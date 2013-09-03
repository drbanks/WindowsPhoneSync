using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WindowsPhoneSync.Utilities
{
    public class ColumnAttribute : Attribute
    {
        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="name">The name/header of the column</param>
        public ColumnAttribute(string name)
        {
            Name = name;
            TextAlignment = string.Empty;
        }

        /// <summary>
        /// True if this column is not to be displayed
        /// </summary>
        public bool Hide { get; set; }

        /// <summary>
        /// True if the cell is to be read-only
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// The column name/header text
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Column sequence number
        /// </summary>
        public int Ordinal { get; set; }

        /// <summary>
        /// Horizontal alignment for the column's cell style
        /// </summary>
        public String TextAlignment { get; set; }
    }
}
