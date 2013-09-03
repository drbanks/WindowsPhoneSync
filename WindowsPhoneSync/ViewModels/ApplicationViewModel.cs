using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using WindowsPhoneSync.Utilities;

namespace WindowsPhoneSync.ViewModels
{
    /// <summary>
    /// The main instance view model.  Contains plumbing for saving/restoring parameters
    /// </summary>
    public class ApplicationViewModel : ViewModelBase
    {
        #region Private Static Fields

        /// <summary>
        /// A table of all known parsers, indexed by property name
        /// </summary>
        private static Dictionary<string, Dictionary<string, MethodInfo>> parsers;

        /// <summary>
        /// The list of known viewmodels
        /// </summary>
        private static Dictionary<ViewModelBase, ViewModelBase> registeredViewModels = new Dictionary<ViewModelBase, ViewModelBase>();

        /// <summary>
        /// The contents of the saved value file
        /// </summary>
        private static XDocument savedValues;

        /// <summary>
        /// Generic class-level synchronization object
        /// </summary>
        private static object syncRoot = new object();

        #endregion

        #region Static Properties

        /// <summary>
        /// Gets the list of registered viewmodels
        /// </summary>
        public static Dictionary<ViewModelBase, ViewModelBase> RegisteredViewModels { get { return registeredViewModels; } }

        #endregion


        #region Public Static Methods

        /// <summary>
        /// De-register an active viewmodel
        /// </summary>
        /// <param name="vm">The viewmodel to de-register</param>
        public static void DeRegister(ViewModelBase vm)
        {
            if (RegisteredViewModels.ContainsKey(vm))
                RegisteredViewModels.Remove(vm);
        }

        /// <summary>
        /// Gets the saved field values XDocument
        /// </summary>
        /// <returns></returns>
        public static XDocument GetSavedFieldsDocument()
        {
            // Load the xml file if not already done:

            if (savedValues == null)
            {
                string saveFile = ConfigurationManager.AppSettings["saveFileName"];
                if (string.IsNullOrWhiteSpace(saveFile))
                    return null;

                string saveFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                                   saveFile);

                try
                {
                    savedValues = XDocument.Load(saveFileName);
                }
                catch (IOException)
                {
                    return null;
                }
            }

            return savedValues;
        }

        /// <summary>
        /// Load the ui related properties for a specific view model
        /// </summary>
        /// <param name="instance">The ViewModel instance we're initializing</param>
        public static void LoadFieldValues(ViewModelBase instance)
        {
            // Get the saved values XML document

            XDocument savedValues = GetSavedFieldsDocument();
            if (savedValues == null)
                return;

            // Pick up all the property settings relevant to this view model:

            var props = from prop in savedValues.Descendants("Property")
                        let className = prop.Element("Class").Value
                        let propertyName = prop.Element("PropertyName").Value
                        let propertyValue = prop.Element("Value").Value
                        where !string.IsNullOrWhiteSpace(className) &&
                              className == instance.InstanceName
                        select new { Class = className, Property = propertyName, Value = propertyValue };

            // Set the values:

            foreach (var property in props)
                SetPropertyValue(instance, property.Property, property.Value);

            // Now fetch any lists that apply:

            var lists = from list in savedValues.Descendants("SavedList")
                        let className = list.Attribute("Class").Value
                        let listName = list.Attribute("List").Value
                        let propInfo = instance.GetType().GetProperty(listName, BindingFlags.Public | BindingFlags.Instance)
                        where !string.IsNullOrWhiteSpace(className) &&
                              className == instance.InstanceName &&
                              propInfo != null
                        let listPointer = propInfo.GetValue(instance, null)
                        select new { Class = className, List = listPointer, ListName = listName, Values = list };

            var tupleLists = from list in savedValues.Descendants("SavedTupleList")
                             let className = list.Attribute("Class").Value
                             let listName = list.Attribute("List").Value
                             let propInfo = instance.GetType().GetProperty(listName, BindingFlags.Public | BindingFlags.Instance)
                             where !string.IsNullOrWhiteSpace(className) &&
                                   className == instance.InstanceName &&
                                   propInfo != null
                             let listPointer = propInfo.GetValue(instance, null)
                             select new { Class = className, List = listPointer, ListName = listName, Values = list };

            // For each List<string> collection we've found...

            foreach (var list in lists)
            {
                ObservableCollection<string> collection = list.List as ObservableCollection<string>;
                if (collection == null)
                    continue;

                // Add each string item in the list:

                foreach (var node in list.Values.Descendants("Item"))
                    collection.Add(node.Value);
            }

            // For each Tuple<,> collection we've found...

            foreach (var list in tupleLists)
            {
                // Get the underlying types of the tuples:

                Type tupleType = list.List.GetType().GetGenericArguments().First();
                Type[] types = tupleType.GetGenericArguments();

                var newTupleMethod = tupleType.GetConstructor(types);
                var addMethod = list.List.GetType().GetMethod("Add");
                foreach (var node in list.Values.Descendants("Items"))
                {
                    // Parse out the values for this list item:

                    object[] args = new object[types.Length];
                    int i = 0;
                    foreach (var type in types)
                    {
                        args[i] = ParseString(node.Descendants("Item" + (i + 1).ToString()).First().Value, type);
                        i++;
                    };

                    var item = newTupleMethod.Invoke(args);
                    addMethod.Invoke(list.List, new object[] { item });
                }
            }
        }

        /// <summary>
        /// Register a viewmodel as active
        /// </summary>
        /// <param name="vm">The ViewModel to register</param>
        public static void Register(ViewModelBase vm)
        {
            if (!RegisteredViewModels.ContainsKey(vm))
                RegisteredViewModels[vm] = vm;
        }

        /// <summary>
        /// Reload the saved value for a field, long after initialization
        /// </summary>
        /// <param name="instance">The instance that we're reloading into</param>
        /// <param name="field">The property to reload</param>
        public static void ReloadFieldValue(ViewModelBase instance, PropertyInfo field)
        {
            // Get the saved values XML document

            XDocument savedValues = GetSavedFieldsDocument();
            if (savedValues == null)
                return;

            // Pick up all the property settings relevant to this view model:

            var props = from prop in savedValues.Descendants("Property")
                        let className = prop.Element("Class").Value
                        let propertyName = prop.Element("PropertyName").Value
                        let propertyValue = prop.Element("Value").Value
                        where !string.IsNullOrWhiteSpace(className) &&
                              className == instance.InstanceName &&
                              propertyName == field.Name
                        select new { Class = className, Property = propertyName, Value = propertyValue };

            // Set the values:

            foreach (var property in props)
                SetPropertyValue(instance, property.Property, property.Value);
        }

        /// <summary>
        /// Save the value of any ui related fields tagged with the SavedDataField or SavedListProperty attribute
        /// </summary>
        public static void SaveFields()
        {
            // Get the save file name.  If there isn't one, don't bother with the rest:

            string saveFileName = ConfigurationManager.AppSettings["saveFileName"];
            if (string.IsNullOrWhiteSpace(saveFileName))
                return;

            var saveDoc = new XmlDocument();
            var fieldsNode = saveDoc.CreateElement("SavedData");
            saveDoc.AppendChild(fieldsNode);
            SavePropertyValues(fieldsNode, saveDoc);
            SaveListValues(fieldsNode, saveDoc);

            // Save the property values:

            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                           saveFileName);
            saveDoc.Save(filePath);
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Gets the type appropriate Parse method for a given property
        /// </summary>
        /// <param name="typeName">The type name that we want to parse</param>
        /// <param name="property">The property to parse into</param>
        /// <returns></returns>
        private static MethodInfo GetParser(string typeName, PropertyInfo property)
        {
            parsers = GetParserCache();
            lock (parsers)
            {
                if (parsers.ContainsKey(typeName) &&
                    parsers[typeName].ContainsKey(property.Name))
                    return parsers[typeName][property.Name];

                MethodInfo parser = GetParser(property.PropertyType, property);

                // If the dictionary for this tpe doesn't exist, create it:

                if (!parsers.ContainsKey(typeName))
                    parsers[typeName] = new Dictionary<string, MethodInfo>();

                parsers[typeName][property.Name] = parser;
                return parser;
            }
        }

        /// <summary>
        /// Get the parser for a specific type
        /// </summary>
        /// <param name="type">The type we need the parser for</param>
        /// <returns></returns>
        private static MethodInfo GetParser(Type type)
        {
            parsers = GetParserCache();
            lock (parsers)
            {
                if (parsers.ContainsKey(type.Name) &&
                    parsers[type.Name].ContainsKey("Parser"))
                    return parsers[type.Name]["Parser"];

                // Not in cache: get it manually:

                var parser = GetParser(type, null);

                if (!parsers.ContainsKey(type.Name))
                    parsers[type.Name] = new Dictionary<string, MethodInfo>();
                parsers[type.Name]["Parser"] = parser;
                return parser;
            }
        }

        /// <summary>
        /// Get a parser for a given property
        /// </summary>
        /// <param name="propertyType">The type of the property that we want to parse</param>
        /// <param name="property">The property to parse into</param>
        /// <returns></returns>
        private static MethodInfo GetParser(Type propertyType, PropertyInfo property = null)
        {
            // Handle nullable types:  Try to find the parser for the nullable's
            // underlying type:

            if (propertyType.Name.Contains("Nullable"))
            {
                propertyType = Nullable.GetUnderlyingType(propertyType);
            }

            // Grab the parser, if any, exception out if none.

            MethodInfo parser = propertyType.GetMethod("Parse", new Type[] { typeof(string) });
            if (parser == null)
            {
                //throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                //                                          "Unsupported value type {0}{1}",
                //                                          propertyType.Name,
                //                                          property == null ? "" : " for property " + property.Name),
                //                            "property");
            }
            return parser;
        }

        /// <summary>
        /// Gets the parser cache
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, Dictionary<string, MethodInfo>> GetParserCache()
        {
            if (parsers != null &&
                parsers.Count > 0)
                return parsers;

            // If the cache doesn't exist, create one

            if (parsers == null)
                parsers = new Dictionary<string, Dictionary<string, MethodInfo>>();
            return parsers;
        }

        /// <summary>
        /// Parse some arbitrary value type, returning the parsed value
        /// </summary>
        /// <param name="value">The string representation of the value</param>
        /// <param name="type">The type of the result we're looking for</param>
        /// <returns>Parsed value</returns>
        private static object ParseString(string value, Type type)
        {
            // If the property is string, just store the value and move on:

            if (type == typeof(string))
            {
                return value;
            }

            // If it's null/empty, just leave the default alone:

            if (string.IsNullOrEmpty(value) && !type.IsValueType)
                return null;
            else if (value == null)
                value = string.Empty;

            // Otherwise, look for a parse method:

            MethodInfo parser = GetParser(type);

            // Parse the value:

            try
            {
                object parsedValue = parser.Invoke(null, new object[] { value });
                return parsedValue;
            }
            catch (TargetInvocationException ex)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                                                          "Unable to parse {0} as a {1}",
                                                          value,
                                                          type.Name),
                                            "value",
                                            ex);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                                                          "Unable to parse {0} as a {1}",
                                                          value,
                                                          type.Name),
                                            "value",
                                            ex);
            }
        }

        /// <summary>
        /// Save the contents of all ObservableCollections of type string flagged with the 
        /// SavedListProperty attribute
        /// </summary>
        /// <param name="root">The root node to append the lists</param>
        /// <param name="saveDoc">The document we're appending to</param>
        private static void SaveListValues(XmlElement root, XmlDocument saveDoc)
        {
            // Get a list of all the lists flagged for saving:

            var lists = (from vm in RegisteredViewModels.Values
                         from prop in vm.GetType().GetProperties(BindingFlags.Instance |
                                                                 BindingFlags.Public)
                         let attribs = prop.GetCustomAttributes(typeof(SaveListPropertyAttribute), true)
                         let list = prop.GetValue(vm, null)
                         where attribs != null &&
                               attribs.Length > 0 &&
                               list != null &&
                               list is IEnumerable
                         select new
                         {
                             Attrib = attribs[0],
                             Instance = vm,
                             List = list,
                             Property = prop,
                         });

            // Write out the list and its values:

            foreach (var list in lists)
            {
                var attribute = list.Attrib;
                var instance = list.Instance;
                var property = list.Property;
                var col = list.List;

                if (col is IEnumerable<string>)
                    SaveStringListValues(root, saveDoc, instance, property, col as IEnumerable<string>);
                else if (col is IEnumerable &&
                         col.GetType().FullName.Contains("Tuple") &&
                         col.GetType().IsGenericType)
                {
                    // Get the types of the tuple members:

                    Type[] args = col.GetType().GetGenericArguments();
                    if (args == null ||
                        args.Length != 1 ||
                        args[0] == null ||
                        !args[0].IsGenericType ||
                        !args[0].FullName.Contains("Tuple"))
                        continue;

                    // Pick out the types that underly the tuple:

                    Type[] types = args[0].GetGenericArguments();
                    if (types.Length != 2)
                        continue;

                    var tupleType = typeof(Tuple<,>).MakeGenericType(types);
                    var collectionType = typeof(IEnumerable<>).MakeGenericType(new Type[] { tupleType });
                    var method = typeof(ApplicationViewModel).GetMethod("SaveTupleListValues",
                                                                       BindingFlags.Static | BindingFlags.NonPublic);
                    if (method == null)
                        continue;

                    var genericMethod = method.MakeGenericMethod(types);
                    if (genericMethod == null)
                        continue;

                    genericMethod.Invoke(null, new object[] { root, saveDoc, instance, property, col });
                }
            }
        }

        /// <summary>
        /// Save the values from a list of strings
        /// </summary>
        /// <param name="root">The root of the XML document</param>
        /// <param name="saveDoc">The XML document being built</param>
        /// <param name="instance">The view model instance to save from</param>
        /// <param name="property">The property to save from</param>
        /// <param name="collection">The actual collection to save</param>
        private static void SaveStringListValues(XmlElement root,
                                                 XmlDocument saveDoc,
                                                 ViewModelBase instance,
                                                 PropertyInfo property,
                                                 IEnumerable<string> collection)
        {
            if (collection == null ||
                collection.Count() == 0)
                return;
            var outerNode = saveDoc.CreateElement("SavedList");
            outerNode.SetAttribute("Class", instance.InstanceName);
            outerNode.SetAttribute("List", property.Name);
            root.AppendChild(outerNode);

            // Add all the list elements to the node:

            foreach (string item in collection)
            {
                var listNode = saveDoc.CreateElement("Item");
                listNode.InnerText = item;
                outerNode.AppendChild(listNode);
            }
        }

        /// <summary>
        /// Save the values from a list of two item tuples
        /// </summary>
        /// <param name="root">The root of the XML document</param>
        /// <param name="saveDoc">The XML document being built</param>
        /// <param name="instance">The view model instance to save from</param>
        /// <param name="property">The property to save from</param>
        /// <param name="collection">The actual collection to save</param>
        private static void SaveTupleListValues<T1, T2>(XmlElement root,
                                                XmlDocument saveDoc,
                                                ViewModelBase instance,
                                                PropertyInfo property,
                                                IEnumerable<Tuple<T1, T2>> collection)
        {
            if (collection == null ||
                collection.Count() == 0)
                return;
            var outerNode = saveDoc.CreateElement("SavedTupleList");
            outerNode.SetAttribute("Class", instance.InstanceName);
            outerNode.SetAttribute("List", property.Name);
            root.AppendChild(outerNode);

            // Add all the list elements to the node:

            foreach (Tuple<T1, T2> item in collection)
            {
                var listNode = saveDoc.CreateElement("Items");
                var item1Node = saveDoc.CreateElement("Item1");
                item1Node.InnerText = typeof(T1).IsValueType || item.Item1 != null ? item.Item1.ToString() : "";
                var item2Node = saveDoc.CreateElement("Item2");
                item2Node.InnerText = typeof(T2).IsValueType || item.Item2 != null ? item.Item2.ToString() : "";

                outerNode.AppendChild(listNode);
                listNode.AppendChild(item1Node);
                listNode.AppendChild(item2Node);
            }
        }

        /// <summary>
        /// Save all the single UI properties that have been flagged for saving
        /// </summary>
        /// <param name="root">The root node to append the values to</param>
        /// <param name="saveDoc">The document we're adding the properties to</param>
        private static void SavePropertyValues(XmlElement root, XmlDocument saveDoc)
        {
            // Get a list of all properties flagged for saving

            var props = (from vm in RegisteredViewModels.Values
                         from prop in vm.GetType().GetProperties(BindingFlags.Instance |
                                                                 BindingFlags.Public)
                         let attribs = prop.GetCustomAttributes(typeof(SavedDataPropertyAttribute), true)
                         where attribs != null && attribs.Length > 0
                         select new
                         {
                             Attribs = attribs,
                             Instance = vm,
                             InstanceName = vm.InstanceName,
                             Property = prop,
                             Value = prop.GetValue(vm, null)
                         });

            // Build the collection of actual values to be saved.  Done this way to handle
            // multiple instances of the same ViewModel type (the last one found "wins")

            var saveValues = new Dictionary<string, Dictionary<string, string>>();
            foreach (var property in props)
            {
                string className = property.InstanceName;
                string propertyName = property.Property.Name;
                string propertyValue = property.Value == null ? (string)null : property.Value.ToString();
                if (!saveValues.ContainsKey(className))
                    saveValues[className] = new Dictionary<string, string>();

                // For each attribute attached to this property, try to save something:

                foreach (SavedDataPropertyAttribute attribute in property.Attribs)
                {
                    if (!string.IsNullOrWhiteSpace(attribute.PropertyName))
                    {
                        // If this attribute has a property name attached, look for that
                        // property as a member of the property we're looking at:

                        var subPropertyInfo = property.Value.GetType().GetProperty(attribute.PropertyName);
                        if (subPropertyInfo == null)
                            continue;
                        propertyName = property.Property.Name + "." + attribute.PropertyName;
                        object value = subPropertyInfo.GetValue(property.Value, new object[0]);
                        propertyValue = value == null ? (string)null : value.ToString();
                    }
                    saveValues[className][propertyName] = propertyValue;
                }
            }

            var outerElement = saveDoc.CreateElement("SavedValues");
            root.AppendChild(outerElement);
            var elements = (from className in saveValues
                            from propertyName in className.Value
                            orderby className.Key, propertyName.Key
                            select new Tuple<string, string, string>(className.Key, propertyName.Key, propertyName.Value));
            foreach (var element in elements)
            {
                var innerElement = saveDoc.CreateElement("Property");
                outerElement.AppendChild(innerElement);
                var className = saveDoc.CreateElement("Class");
                className.InnerText = element.Item1;
                var propertyName = saveDoc.CreateElement("PropertyName");
                propertyName.InnerText = element.Item2;
                var propertyValue = saveDoc.CreateElement("Value");
                propertyValue.InnerText = element.Item3;

                innerElement.AppendChild(className);
                innerElement.AppendChild(propertyName);
                innerElement.AppendChild(propertyValue);
            }
        }

        /// <summary>
        /// Parse a field according to its datatype, and save it as the field's
        /// value
        /// </summary>
        /// <param name="property"></param>
        /// <param name="instance">The instance owning the property to be initialized</param>
        /// <param name="propertyName">The name of the property to set</param>
        /// <param name="value">The property's intended value, in string form</param>
        private static void SetPropertyValue(object instance, string propertyName, string value)
        {
            // Recurse down the dotted elements of the saved property name to get the
            // destination property:

            if (string.IsNullOrWhiteSpace(propertyName))
                return;

            string[] path = propertyName.Split('.');

            PropertyInfo property = null;
            for (int i = 0; i < path.Length; i++)
            {
                // Loop here for each of the properties in the path:

                property = instance.GetType().GetProperty(path[i],
                                                          BindingFlags.Public | BindingFlags.Instance);
                if (property == null)
                    return;

                // On all but the last, advance the instance down to the just-found
                // path element:

                if (i < path.Length - 1)
                {
                    instance = property.GetValue(instance, new object[0]);
                    if (instance == null)
                        return;
                }
            }

            // Get the property's datatype:

            Type propertyType = property.PropertyType;

            // If the property is string, just store the value and move on:

            if (propertyType == typeof(string))
            {
                property.SetValue(instance, value, null);
                return;
            }

            // If it's null/empty, just leave the default alone:

            if (string.IsNullOrEmpty(value))
                return;

            // Otherwise, look for a parse method:

            MethodInfo parser = GetParser(instance.GetType().Name, property);

            // If no parser, punt it:

            if (parser == null)
                return;

            // Parse the value:

            try
            {
                // Hack: Boolean types seem to get written out with a capitalized "true".
                // If the property type is Boolean, force the value to lower case:

                if (property.PropertyType.Name == typeof(Boolean).Name)
                    value = value == null ? "false" : value.ToLower();

                object parsedValue = parser.Invoke(instance, new object[] { value });

                // Having parsed the value, store it in the object:

                property.SetValue(instance, parsedValue, null);
            }
            catch (TargetInvocationException ex)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                                                          "Unable to parse {0} as a {1} for property {2}.",
                                                          value,
                                                          propertyType.Name,
                                                          property.Name),
                                            "value",
                                            ex);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                                                          "Unable to parse {0} as a {1} for property {2}.",
                                                          value,
                                                          propertyType.Name,
                                                          property.Name),
                                            "value",
                                            ex);
            }
        }

        #endregion
    }
}
