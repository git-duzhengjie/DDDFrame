﻿
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Infra.Consul.Configuration
{
    /// <summary>
    /// Json配置文件转换器
    /// </summary>
    internal class JsonConfigurationFileParser
    {
        #region <变量>

        private readonly IDictionary<string, string> _data = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly Stack<string> _context = new Stack<string>();
        private string _currentPath;

        #endregion <变量>

        #region <构造方法和析构方法>

        /// <summary>
        /// 私有构造
        /// </summary>
        private JsonConfigurationFileParser()
        {
        }

        #endregion <构造方法和析构方法>

        #region <方法>

        public static IDictionary<string, string> Parse(Stream input)
           => new JsonConfigurationFileParser().ParseStream(input);

        private IDictionary<string, string> ParseStream(Stream input)
        {
            _data.Clear();

            var jsonDocumentOptions = new JsonDocumentOptions
            {
                CommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
            };

            using (var reader = new StreamReader(input))
            using (JsonDocument doc = JsonDocument.Parse(reader.ReadToEnd(), jsonDocumentOptions))
            {
                if (doc.RootElement.ValueKind != JsonValueKind.Object)
                {
                    throw new FormatException("数据格式不正确");
                }
                VisitElement(doc.RootElement);
            }

            return _data;
        }

        private void VisitElement(JsonElement element)
        {
            foreach (var property in element.EnumerateObject())
            {
                EnterContext(property.Name);
                VisitValue(property.Value);
                ExitContext();
            }
        }

        private void VisitValue(JsonElement value)
        {
            switch (value.ValueKind)
            {
                case JsonValueKind.Object:
                    VisitElement(value);
                    break;

                case JsonValueKind.Array:
                    var index = 0;
                    foreach (var arrayElement in value.EnumerateArray())
                    {
                        EnterContext(index.ToString());
                        VisitValue(arrayElement);
                        ExitContext();
                        index++;
                    }
                    break;

                case JsonValueKind.Number:
                case JsonValueKind.String:
                case JsonValueKind.True:
                case JsonValueKind.False:
                case JsonValueKind.Null:
                    var key = _currentPath;
                    if (_data.ContainsKey(key))
                    {
                        throw new FormatException($"键{key}重复定义");
                    }
                    _data[key] = value.ToString();
                    break;

                default:
                    throw new FormatException("数据格式不正确");
            }
        }

        private void EnterContext(string context)
        {
            _context.Push(context);
            _currentPath = ConfigurationPath.Combine(_context.Reverse());
        }

        private void ExitContext()
        {
            _context.Pop();
            _currentPath = ConfigurationPath.Combine(_context.Reverse());
        }

        #endregion <方法>
    }
}
