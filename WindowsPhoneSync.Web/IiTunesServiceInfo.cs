﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WindowsPhoneSync.DataContracts;

namespace WindowsPhoneSync.Web
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IiTunesInfoService" in both code and config file together.
    [ServiceContract]
    public interface IiTunesInfoService
    {
        /// <summary>
        /// Get a single song's metadata
        /// </summary>
        [OperationContract]
        SongMetadata GetMetaData(string artist, string album, string trackName);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        // TODO: Add your service operations here
    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    // You can add XSD files into the project. After building the project, you can directly use the data types defined there, with the namespace "WindowsPhoneSync.Web.ContractType".
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
