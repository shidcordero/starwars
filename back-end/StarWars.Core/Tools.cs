using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;

namespace StarWars.Core
{
    public static class Tools
    {
        private const int BUFFER_SIZE = 64 * 1024; //64kB

        public static Random Random { get; } = new Random();

        public static object ToSqlNullable<T>(this object o) where T : struct
        {
            if (o == null)
                return DBNull.Value;
            else
                return o;
        }

        public static object ToSqlNullable(this string o)
        {
            if (string.IsNullOrWhiteSpace(o))
                return DBNull.Value;
            else
                return o;
        }

        public static object ToSqlNullable(this string o, string nullStart)
        {
            if (string.IsNullOrWhiteSpace(o) || o.StartsWith(nullStart, StringComparison.InvariantCultureIgnoreCase))
                return DBNull.Value;
            else
                return o;
        }

        public static string ToSqlString(this string s)
        {
            return s.Replace("'", "''", StringComparison.OrdinalIgnoreCase);
        }

        public static Nullable<T> ToNullable<T>(this object o) where T : struct
        {
            if (o == DBNull.Value)
                return null;

            if (o is string && string.IsNullOrWhiteSpace((string)o))
                return null;

            var converter = TypeDescriptor.GetConverter(typeof(T?));
            return (T?)converter.ConvertFrom(o);
        }

        public static string ToNullable(this object o)
        {
            if (o == null || o == DBNull.Value)
                return null;
            else
                return (string)o;
        }

        public static int? NullInvalidInt(object input)
        {
            if (input == null || input == DBNull.Value)
                return null;

            var tryit = int.TryParse(input.ToString(), out int result);
            if (tryit)
                return result;
            else
                return null;
        }

        public static int ZeroInvalidInt(object input)
        {
            if (input == null || input == DBNull.Value)
                return 0;

            var tryit = int.TryParse(input.ToString(), out int result);
            if (tryit)
                return result;
            else
                return 0;
        }

        public static double ZeroInvalidDouble(object input)
        {
            if (input == null || input == DBNull.Value)
                return 0;

            var tryit = double.TryParse(input.ToString(), out double result);
            if (tryit)
                return result;
            else
                return 0;
        }

        public static decimal ZeroInvalidDecimal(object input)
        {
            if (input == null || input == DBNull.Value)
                return 0;

            var tryit = decimal.TryParse(input.ToString(), out decimal result);
            if (tryit)
                return result;
            else
                return 0;
        }

        public static long ZeroInvalidLong(object input)
        {
            if (input == null || input == DBNull.Value)
                return 0;

            var tryit = long.TryParse(input.ToString(), out long result);
            if (tryit)
                return result;
            else
                return 0;
        }

        public static DateTime? TryDateTime(string input, string format)
        {
            var isDateTime = DateTime.TryParseExact(input, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime output);

            if (isDateTime)
                return output;
            else
                return null;
        }

        public static int? TryInt(string input)
        {
            if (int.TryParse(input, out int output))
                return output;
            else
                return null;
        }

        public static Guid? TryGuid(string input)
        {
            if (Guid.TryParse(input, out Guid output))
                return output;
            else
                return null;
        }

        public static decimal? TryDecimal(string input)
        {
            if (decimal.TryParse(input, out decimal output))
                return output;
            else
                return null;
        }

        public static byte[] ToByteArray(this Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using (stream)
            {
                using var memStream = new MemoryStream();
                stream.CopyTo(memStream);
                return memStream.ToArray();
            }
        }

        /// <summary>
        /// Given an list of objects, serialises the list into an XML document
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="RootAttribute"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        public static XmlDocument GetEntityXml<T>(string rootAttribute, ICollection<T> objects)
        {
            var xmlDoc = new XmlDocument();
            var nav = xmlDoc.CreateNavigator();

            using (var writer = nav.AppendChild())
            {
                var ser = new XmlSerializer(typeof(List<T>), new XmlRootAttribute(rootAttribute));
                ser.Serialize(writer, objects);
            }
            return xmlDoc;
        }

        public static List<int> MakeListInt(this object o)
        {
            if (!(o is string))
                return new List<int>();

            return Array.ConvertAll<string, int>(((string)o).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), new Converter<string, int>(s => Tools.ZeroInvalidInt(s))).ToList<int>();
        }

        public static List<double> MakeListDouble(this object o)
        {
            if (!(o is string))
                return new List<double>();

            return Array.ConvertAll<string, double>(((string)o).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), new Converter<string, double>(s => Tools.ZeroInvalidDouble(s))).ToList<double>();
        }

        public static List<decimal> MakeListDecimal(this object o)
        {
            if (!(o is string))
                return new List<decimal>();

            return Array.ConvertAll<string, decimal>(((string)o).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), new Converter<string, decimal>(s => Tools.ZeroInvalidDecimal(s))).ToList<decimal>();
        }

        public static List<string> MakeListString(this object o, char separator = ',')
        {
            if (!(o is string))
                return new List<string>();

            return ((string)o).Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
        }

        public static byte[] Compress(byte[] inputData)
        {
            if (inputData == null)
                throw new ArgumentNullException(nameof(inputData));

            using var compressIntoMs = new MemoryStream();
            using var gzs = new BufferedStream(new GZipStream(compressIntoMs, CompressionMode.Compress), BUFFER_SIZE);
            gzs.Write(inputData, 0, inputData.Length);
            return compressIntoMs.ToArray();
        }

        public static byte[] Decompress(byte[] inputData)
        {
            if (inputData == null)
                throw new ArgumentNullException(nameof(inputData));

            using var compressedMs = new MemoryStream(inputData);
            using var decompressedMs = new MemoryStream();
            using var gzs = new BufferedStream(new GZipStream(compressedMs, CompressionMode.Decompress), BUFFER_SIZE);
            gzs.CopyTo(decompressedMs);
            return decompressedMs.ToArray();
        }

        public static decimal? Truncate(this decimal? value, int precision)
        {
            if (!value.HasValue)
                return null;
            decimal step = (decimal)Math.Pow(10, precision);
            decimal tmp = Math.Truncate(step * value.Value);
            return tmp / step;
        }

        public static string GetNumbers(string input)
        {
            return new string(input.Where(c => char.IsDigit(c)).ToArray());
        }

        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string ToJsonString(this JsonDocument jdoc)
        {
            if (jdoc == null)
                return null;

            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });
            jdoc.WriteTo(writer);
            writer.Flush();
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        /// <summary>
        /// Takes a List collection of string and returns a delimited string.  Note that it's easy to create a huge list that won't turn into a huge string because
        /// the string needs contiguous memory.
        /// </summary>
        /// <param name="list">The input List collection of string objects</param>
        /// <param name="qualifier">
        /// The default delimiter. Using a colon in case the List of string are file names,
        /// since it is an illegal file name character on Windows machines and therefore should not be in the file name anywhere.
        /// </param>
        /// <param name="insertSpaces">Whether to insert a space after each separator</param>
        /// <returns>A delimited string</returns>
        /// <remarks>This was implemented pre-linq</remarks>
        public static string ToDelimitedString(this List<string> list, string delimiter = ":", bool insertSpaces = false, string qualifier = "", bool duplicateTicksForSQL = false)
        {
            var result = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                string initialStr = duplicateTicksForSQL ? list[i].ToSqlString() : list[i];
                result.Append(string.IsNullOrEmpty(qualifier) ? initialStr : string.Format(CultureInfo.InvariantCulture, "{1}{0}{1}", initialStr, qualifier));
                if (i < list.Count - 1)
                {
                    result.Append(delimiter);
                    if (insertSpaces)
                    {
                        result.Append(' ');
                    }
                }
            }
            return result.ToString();
        }
    }
}