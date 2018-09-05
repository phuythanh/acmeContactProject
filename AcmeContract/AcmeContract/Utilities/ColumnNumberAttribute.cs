using System;

namespace AcmeContract.Utilities.FileReader
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnNumberAttribute : Attribute
    {
        public int ColumnNumber { get; set; }
        public ColumnNumberAttribute(int columnNumber)
        {
            ColumnNumber = columnNumber;
        }
    }
}