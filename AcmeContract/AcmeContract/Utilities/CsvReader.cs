using AcmeContract.Utilities.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AcmeContract.Utilities.FileReader
{
    public class CsvConfig
    {
        public char Delimiter { get; private set; }
        public string NewLineMark { get; private set; }
        public char QuotationMark { get; private set; }

        public CsvConfig(char delimiter, string newLineMark, char quotationMark)
        {
            Delimiter = delimiter;
            NewLineMark = newLineMark;
            QuotationMark = quotationMark;
        }

        // useful configs

        public static CsvConfig Default
        {
            get { return new CsvConfig(',', "\r\n", '\"'); }
        }

        // etc.
    }
    public class CsvReader : ICsvReader
    {
        private CsvConfig m_config;

        public CsvReader(CsvConfig config = null)
        {
            if (config == null)
                m_config = CsvConfig.Default;
            else
                m_config = config;
        }
        public List<T> ProcessCsvFile<T>(string csvFile) where T : class
        {
            if (!File.Exists(csvFile))
            {
                throw new FileNotFoundException($"{csvFile} does not exist!");
            }

            List<T> list = new List<T>();
            var csvDataLines = ReadCsv(csvFile).ToList();
            if (csvDataLines.Count <= 1) // file is empty
            {
                return list;
            }

            ValidateFileLevelError(csvDataLines);

            string[] headers = csvDataLines[0];
            for (int row = 1; row < csvDataLines.Count; row++)
            {
                string[] values = csvDataLines[row];
                T t = Activator.CreateInstance<T>();
                var properties = t.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var prop in properties)
                {
                    if (!prop.CanWrite || !prop.CanRead) { continue; }
                    int index = -1;
                    var displayAtt = (DisplayNameAttribute)prop.GetCustomAttribute(typeof(DisplayNameAttribute));
                    var columnNumberAtt = (ColumnNumberAttribute)prop.GetCustomAttribute(typeof(ColumnNumberAttribute));
                    index = Array.FindIndex(headers, h => h.Trim().ToLower() == prop.Name.ToLower() || h.Trim().ToLower() == displayAtt?.DisplayName.ToLower());

                    if (index >= 0 && index < values.Length)
                    {
                        prop.SetValue(t, Convert.ChangeType(values[index], prop.PropertyType), null);
                    }
                }

                list.Add(t);
            }
            return list;
        }

        private IEnumerable<string[]> ReadCsv(string fileName)
        {
            var list = new List<string[]>();
            using (var reader = new StreamReader(fileName, Encoding.Default))
            {
                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                        break;
                    yield return ParseLine(line);
                }
            }
        }

        private string[] ParseLine(string line)
        {
            Stack<string> result = new Stack<string>();

            int i = 0;
            while (true)
            {
                string cell = ParseNextCell(line, ref i);
                if (cell == null)
                    break;
                result.Push(cell);
            }

            // remove last elements if they're empty
            while (string.IsNullOrEmpty(result.Peek()))
            {
                result.Pop();
            }

            var resultAsArray = result.ToArray();
            Array.Reverse(resultAsArray);
            return resultAsArray;
        }

        // returns iterator after delimiter or after end of string
        private string ParseNextCell(string line, ref int i)
        {
            if (i >= line.Length)
                return null;

            if (line[i] != m_config.QuotationMark)
                return ParseNotEscapedCell(line, ref i);
            else
                return ParseEscapedCell(line, ref i);
        }

        // returns iterator after delimiter or after end of string
        private string ParseNotEscapedCell(string line, ref int i)
        {
            StringBuilder sb = new StringBuilder();
            while (true)
            {
                if (i >= line.Length) // return iterator after end of string
                    break;
                if (line[i] == m_config.Delimiter)
                {
                    i++; // return iterator after delimiter
                    break;
                }
                sb.Append(line[i]);
                i++;
            }
            return sb.ToString();
        }

        // returns iterator after delimiter or after end of string
        private string ParseEscapedCell(string line, ref int i)
        {
            i++; // omit first character (quotation mark)
            StringBuilder sb = new StringBuilder();
            while (true)
            {
                if (i >= line.Length)
                    break;
                if (line[i] == m_config.QuotationMark)
                {
                    i++; // we're more interested in the next character
                    if (i >= line.Length)
                    {
                        // quotation mark was closing cell;
                        // return iterator after end of string
                        break;
                    }
                    if (line[i] == m_config.Delimiter)
                    {
                        // quotation mark was closing cell;
                        // return iterator after delimiter
                        i++;
                        break;
                    }
                    if (line[i] == m_config.QuotationMark)
                    {
                        // it was doubled (escaped) quotation mark;
                        // do nothing -- we've already skipped first quotation mark
                    }

                }
                sb.Append(line[i]);
                i++;
            }

            return sb.ToString();
        }

        private void ValidateFileLevelError(List<string[]> csvDataLines)
        {
            string[] headers = csvDataLines[0];
            var indexRowsHaveDifferentColumnNumber = csvDataLines.Select((values, row) => new { ValueArray = values, Index = row })
                                                                            .Where(s => s.ValueArray.Length != headers.Length)
                                                                            .Select(s => s.Index + 1).ToList();
            if (indexRowsHaveDifferentColumnNumber.Any())
            {
                throw new IOException($"Number of columns are different between header and data at row {string.Join(",", indexRowsHaveDifferentColumnNumber)}");
            }
        }


    }
}
