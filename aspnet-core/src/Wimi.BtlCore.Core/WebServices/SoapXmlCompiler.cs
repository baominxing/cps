using NCalc2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Wimi.BtlCore.WebServices
{
    public class SoapXmlCompiler
    {
        public SoapXmlCompiler()
        {
            Scope = new Dictionary<string, dynamic>();
            XmlNs = new List<XmlNameSpace>();
        }

        static SoapXmlCompiler()
        {
            RepeaterKey = "sxRepeat";
            IfKey = "sxIf";
            TemplateKey = "sxRun";
            Filters = new Dictionary<string, Func<object, string>>
            {
                ["currency"] = x =>
                {
                    //this is simple and dirty to support all numeric types.
                    var s = x.ToString();
                    double d;
                    double.TryParse(s, out d);
                    return d.ToString("C");
                },
                ["stringFormat"] = x =>
                {
                    var result = ((dynamic)x).Result;
                    var format = ((dynamic)x).Format;

                    return result.ToString(format);
                }
            };
            IsExpressionRegex = new Regex("(?<={{).*?(?=}})");
            ForEachRegex = new Regex(@"^\s*([a-zA-Z_]+[\w]*)\s+in\s+(([a-zA-Z][\w]*(\.[a-zA-Z][\w]*)*)|\[(.+)(,\s*.+)*\])\s*$", RegexOptions.Singleline);
            StringFormatRegex = new Regex(@"(?<=\("").+?(?=""\))");
        }

        public static string RepeaterKey { get; set; }

        public static string IfKey { get; set; }

        public static string TemplateKey { get; set; }

        public static dynamic OnNullOrNotFound { get; set; } = false;

        public static Dictionary<string, Func<object, string>> Filters { get; }

        public List<XmlNameSpace> XmlNs { get; }

        private static readonly Regex IsExpressionRegex;
        private static readonly Regex ForEachRegex;
        private static readonly Regex StringFormatRegex;

        private static readonly char[] ValidStartName =
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            '_', '$'
        };

        private static readonly char[] ValidContentName =
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            '_', '$', '1','2','3','4','5','6','7','8','9','0', '.', '[', ']'
        };

        private static readonly string[] KeyWords = { "if" };

        public XmlWriterSettings XmlWriterSettings { get; set; }

        public Dictionary<string, dynamic> Scope { get; set; }

        public SoapXmlCompiler SetScope(Dictionary<string, dynamic> scope)
        {
            Scope = scope;
            return this;
        }

        public SoapXmlCompiler AddKey(string key, dynamic value)
        {
            Scope[key] = value;
            return this;
        }

        /// <summary>
        /// Compiles a string template
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string CompileString(string input)
        {
            var template = "<sx.string>" + input + "</sx.string>";
            using (var reader = XmlReader.Create(new StringReader(template)))
            {
                var output = new StringBuilder();
                var ws = XmlWriterSettings ?? new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8, OmitXmlDeclaration = true };
                using (var writer = XmlWriter.Create(output, ws))
                {
                    var compiled = _readXml(reader);
                    compiled.Run(writer, this.XmlNs);
                }
                return output.ToString().Replace("<sx.string>", "").Replace("</sx.string>", "");
            }
        }


        /// <summary>
        /// Compiles a Xml template with specified URI.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="root">
        ///     Set the root to compile, to improve performance. 
        ///     example: x => x.Children.First(y => x.Name == "MyElement")
        /// </param>
        /// <returns></returns>
        public string CompileXml(string uri, Func<XmlElement, XmlElement> root = null)
        {
            using (var reader = XmlReader.Create(uri))
            {
                var output = new StringBuilder();
                var ws = XmlWriterSettings ?? new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8, OmitXmlDeclaration = true };
                using (var writer = XmlWriter.Create(output, ws))
                {
                    var compiled = _readXml(reader);
                    if (root != null)
                    {
                        compiled = root(compiled);
                    }
                    compiled.Run(writer, this.XmlNs);
                }
                return output.ToString();
            }
        }


        /// <summary>
        /// Compiles a Xml template using the specified stream with default settings. 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="root">
        ///     Set the root to compile, to improve performance. 
        ///     example: x => x.Children.First(y => x.Name == "MyElement")
        /// </param>
        /// <returns></returns>
        public string CompileXml(Stream stream, Func<XmlElement, XmlElement> root = null)
        {
            using (var reader = XmlReader.Create(stream))
            {
                var output = new StringBuilder();
                var ws = XmlWriterSettings ?? new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8, OmitXmlDeclaration = true };
                using (var writer = XmlWriter.Create(output, ws))
                {
                    var compiled = _readXml(reader);
                    if (root != null)
                    {
                        compiled = root(compiled);
                    }
                    compiled.Run(writer, this.XmlNs);
                }
                return output.ToString();
            }
        }


        /// <summary>
        /// Compiles a Xml template by using the specified text reader. 
        /// </summary>
        /// <param name="textReader"></param>
        /// <param name="root">
        ///     Set the root to compile, to improve performance. 
        ///     example: x => x.Children.First(y => x.Name == "MyElement")
        /// </param>
        /// <returns></returns>
        public string CompileXml(TextReader textReader, Func<XmlElement, XmlElement> root = null)
        {
            using (var reader = XmlReader.Create(textReader))
            {
                var output = new StringBuilder();
                var ws = XmlWriterSettings ?? new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8, OmitXmlDeclaration = true };
                using (var writer = XmlWriter.Create(output, ws))
                {
                    var compiled = _readXml(reader);
                    if (root != null)
                    {
                        compiled = root(compiled);
                    }
                    compiled.Run(writer, this.XmlNs);
                }
                return output.ToString();
            }
        }


        /// <summary>
        /// Compiles a Xml template with a specified XmlReader
        /// </summary>
        /// <param name="xmlReader"></param>
        /// <param name="root">
        ///     Set the root to compile, to improve performance. 
        ///     example: x => x.Children.First(y => x.Name == "MyElement")
        /// </param>
        /// <returns></returns>
        public string CompileXml(XmlReader xmlReader, Func<XmlElement, XmlElement> root = null)
        {
            var output = new StringBuilder();
            var ws = XmlWriterSettings ?? new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8, OmitXmlDeclaration = true };
            using (var writer = XmlWriter.Create(output, ws))
            {
                var compiled = _readXml(xmlReader);
                if (root != null)
                {
                    compiled = root(compiled);
                }
                compiled.Run(writer, this.XmlNs);
            }
            return output.ToString();
        }

        private XmlElement _readXml(XmlReader reader)
        {
            var element = new XmlElement(BufferCommands.NewDocument) { Scope = Scope };
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        element = new XmlElement(BufferCommands.NewElement)
                        {
                            Name = reader.Name,
                            Parent = element
                        };
                        for (var i = 0; i < reader.AttributeCount; i++)
                        {
                            reader.MoveToAttribute(i);
                            if (reader.Name.StartsWith("xmlns"))
                            {
                                XmlNameSpace Ns = new XmlNameSpace();
                                Ns.Value = reader.Value;
                                if (reader.Name.Contains(':'))
                                {
                                    Ns.Prefix = "xmlns";
                                    Ns.Name = reader.Name.Substring(reader.Name.IndexOf(":") + 1);
                                }
                                else
                                {
                                    Ns.Prefix = "";
                                    Ns.Name = reader.Name;
                                }
                                if (!element.NsElement)
                                {
                                    element.NsElement = true;
                                    element.NameSpaces = new List<XmlNameSpace>();
                                }
                                XmlNs.Add(Ns);
                                element.NameSpaces.Add(Ns);
                            }
                            else
                            {
                                element.Attributes.Add(new XmlAttribute
                                {
                                    Name = reader.Name,
                                    Value = reader.Value
                                });
                            }
                        }
                        if (reader.AttributeCount > 0) reader.MoveToElement();
                        if (reader.IsEmptyElement) goto case XmlNodeType.EndElement;
                        break;
                    case XmlNodeType.Text:
                        new XmlElement(BufferCommands.StringContent)
                        {
                            Value = reader.Value,
                            Parent = element
                        };
                        break;
                    case XmlNodeType.EndElement:
                        element = element.Parent;
                        break;
                    case XmlNodeType.XmlDeclaration:
                    case XmlNodeType.ProcessingInstruction:
                    case XmlNodeType.Comment:
                        //ignored
                        break;
                }
            }
            var root = element.Children.First();
            return root;
        }

        public class XmlElement
        {
            private XmlElement _parent;
            public XmlElement(BufferCommands type)
            {
                Attributes = new List<XmlAttribute>();
                Children = new List<XmlElement>();
                Type = type;
            }

            private BufferCommands Type { get; }

            /// <summary>
            /// Element whether has namespace
            /// </summary>
            public bool NsElement { get; set; }

            public List<XmlNameSpace> NameSpaces { get; set; }
            /// <summary>
            /// Name of the Element
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Content of the Element
            /// </summary>
            public string Value { get; set; }
            /// <summary>
            /// Sets Name space to xml Element
            /// </summary>
            /// <summary>
            /// Attributes in the Element
            /// </summary>
            public List<XmlAttribute> Attributes { get; }

            public XmlElement Parent
            {
                get { return _parent; }
                set
                {
                    _parent = value;
                    _parent?.Children.Add(this);
                }
            }
            /// <summary>
            /// Gets the children of this element
            /// </summary>
            public List<XmlElement> Children { get; }
            /// <summary>
            /// Scope of current Element.
            /// </summary>
            public Dictionary<string, dynamic> Scope { get; set; }
            public dynamic GetValueFromScope(string propertyName)
            {
                try
                {
                    var keys = propertyName.Split('.');
                    var digTo = keys.Count(x => x == "$parent");
                    var d = 0;
                    var p = this;

                    while (digTo > 0 && digTo + 1 > d)
                    {
                        p = p.Parent;
                        d += p.Scope != null ? 1 : 0;
                    }
                    if (digTo > 0) keys = keys.Where(x => x != "$parent").ToArray();

                    var property = new PropertyAccess(keys[0]);
                    var scope = p.Scope;
                    var parent = this;
                    while (scope == null || !scope.ContainsKey(property.Name))
                    {
                        parent = parent.Parent;
                        scope = parent.Scope;
                    }

                    var obj = scope[property.Name];
                    var level = 1;

                    while (level < keys.Length)
                    {
                        obj = property.GetValue(obj);
                        property = new PropertyAccess(keys[level]);
                        var t = obj.GetType();
                        obj = t == typeof(Dictionary<string, dynamic>) || t.IsArray
                            ? obj[property.Name]
                            : t.GetProperty(property.Name).GetValue(obj, null);
                        level++;
                    }

                    return property.GetValue(obj);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            private IEnumerable<Dictionary<string, dynamic>> Repeater()
            {
                var repeaterAttribute = Attributes.FirstOrDefault(x => x.Name == RepeaterKey);
                if (repeaterAttribute == null)
                {
                    yield return null;
                    yield break;
                }

                var expression = repeaterAttribute.Value;

                if (!ForEachRegex.IsMatch(expression))
                    throw new FormatException(
                        "Compilation Error: ForEach was expecting an expression like " +
                        "'varName in [value1, value2, value3..., valueN]'");

                var match = ForEachRegex.Match(expression);
                var repeater = match.Groups[1].ToString();
                var scopeName = match.Groups[3].ToString();
                var items = GetValueFromScope(scopeName);

                var i = 0;

                foreach (var item in items ?? new List<int>())
                {
                    var even = i % 2 == 0;
                    yield return new Dictionary<string, dynamic>
                    {
                        [repeater] = item,
                        ["$index"] = i++,
                        ["$odd"] = !even,
                        ["$even"] = even
                    };
                }
            }

            private Dictionary<string, CExpression> _cache = new Dictionary<string, CExpression>();
            private CExpression _ifCache;

            private bool If()
            {
                var at = Attributes.FirstOrDefault(x => x.Name == IfKey);
                var expression = at?.Value;
                if (string.IsNullOrEmpty(expression)) return true;
                if (_ifCache == null) _ifCache = new CExpression(expression, this);
                var e = _ifCache.Evaluate();
                bool res;
                var couldConvert = bool.TryParse(e, out res);
                return couldConvert && res;
            }

            private string Inject(string expression)
            {
                foreach (var v in IsExpressionRegex.Matches(expression).Cast<Match>()
                            .GroupBy(x => x.Value).Select(varGroup => varGroup.First().Value))
                {
                    if (!_cache.ContainsKey(v)) _cache.Add(v, new CExpression(v, this));
                    expression = expression.Replace("{{" + v + "}}", _cache[v].Evaluate());
                }
                return expression;
            }

            public void Run(XmlWriter writer, List<XmlNameSpace> XmlNs)
            {
                switch (Type)
                {
                    case BufferCommands.NewElement:
                        var isTemplate = Name == TemplateKey;
                        foreach (var scope in Repeater())
                        {
                            Scope = scope;
                            if (!If()) continue;
                            if (!isTemplate)
                                if (XmlNs.Count > 0)
                                {
                                    string[] EleNames = Name.Split(':');
                                    if (EleNames.Length > 1)
                                    {
                                        string RealName = EleNames.Last();
                                        if (NsElement)
                                        {
                                            var NameSpace = NameSpaces[0];
                                            writer.WriteStartElement(EleNames[0], RealName, NameSpace.Value);
                                            for (int i = 0; i < NameSpaces.Count(); i++)
                                            {
                                                if (NameSpaces[i].Name != NameSpace.Name)
                                                {
                                                    writer.WriteAttributeString(NameSpaces[i].Prefix, NameSpaces[i].Name, null, NameSpaces[i].Value);
                                                }
                                            }
                                        }
                                        else //Children use ns
                                        {
                                            var NameSpace = XmlNs.FirstOrDefault(x => x.Name == EleNames[0]);
                                            writer.WriteStartElement(RealName, NameSpace.Value);
                                        }
                                    }
                                    else
                                    {
                                        if (NsElement)
                                        {
                                            writer.WriteStartElement(Name, NameSpaces[0].Value);
                                            for (int i = 0; i < NameSpaces.Count(); i++)
                                            {
                                                if (NameSpaces[i].Name != NameSpaces[0].Name)
                                                {
                                                    writer.WriteAttributeString(NameSpaces[i].Prefix, NameSpaces[i].Name, null, NameSpaces[i].Value);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            writer.WriteStartElement(Name);
                                        }
                                    }
                                }
                                else
                                {
                                    writer.WriteStartElement(Name);
                                }

                            foreach (var attribute in Attributes.Where(attribute => attribute.Name != RepeaterKey
                                                                                    && attribute.Name != IfKey))
                            {
                                if (attribute.Name.Contains(':'))
                                {
                                    string[] AttNames = Name.Split(':');
                                    writer.WriteAttributeString(AttNames[0], AttNames[1], null, Inject(attribute.Value));
                                }
                                else
                                {
                                    writer.WriteAttributeString(attribute.Name, Inject(attribute.Value));
                                }
                            }
                            foreach (var child in Children)
                            {
                                child.Run(writer, XmlNs);
                            }
                            if (!isTemplate) writer.WriteEndElement();
                        }
                        break;
                    case BufferCommands.StringContent:
                        writer.WriteString(Inject(Value));
                        break;
                }
            }

            private class CExpression
            {
                public CExpression(string expression, XmlElement parent)
                {
                    Items = new List<CExpressionItem>();
                    Parent = parent;
                    OriginalExpression = expression;

                    var read = new List<char>();
                    var type = LectureType.Unknow;

                    foreach (var c in expression)
                    {
                        switch (type)
                        {
                            case LectureType.Variable:
                                if (!ValidContentName.Contains(c))
                                {
                                    var l = new string(read.ToArray());
                                    Items.Add(new CExpressionItem
                                    {
                                        FromScope = !KeyWords.Contains(l),
                                        Value = l
                                    });
                                    read.Clear();
                                    type = LectureType.Unknow;
                                }
                                break;
                            case LectureType.String:
                                if (c == '\'')
                                {
                                    read.Add('\'');
                                    Items.Add(new CExpressionItem
                                    {
                                        FromScope = false,
                                        Value = new string(read.ToArray())
                                    });
                                    read.Clear();
                                    type = LectureType.Unknow;
                                    continue;
                                }
                                break;
                            case LectureType.Constant:
                                if (ValidStartName.Contains(c) || c == '\'' || c == '|')
                                {
                                    Items.Add(new CExpressionItem
                                    {
                                        FromScope = false,
                                        Value = new string(read.ToArray())
                                    });
                                    read.Clear();
                                    type = LectureType.Unknow;
                                }
                                break;
                        }
                        if (type == LectureType.Unknow)
                        {
                            if (ValidStartName.Contains(c))
                            {
                                type = LectureType.Variable;
                            }
                            else
                                switch (c)
                                {
                                    case '\'':
                                        type = LectureType.String;
                                        break;
                                    case '|':
                                        type = LectureType.Filter;
                                        break;
                                    default:
                                        type = LectureType.Constant;
                                        break;
                                }
                        }
                        read.Add(c);
                    }
                    if (type != LectureType.Filter)
                    {
                        Items.Add(new CExpressionItem
                        {
                            FromScope = type == LectureType.Variable,
                            Value = new string(read.ToArray())
                        });
                    }
                    else
                    {
                        Filter = new string(read.ToArray()).Replace("|", "").Trim();
                    }
                }

                private List<CExpressionItem> Items { get; }
                private XmlElement Parent { get; }
                private string OriginalExpression { get; }
                private string Filter { get; }

                public string Evaluate()
                {
                    var sb = new StringBuilder();
                    var p = 0;
                    var parameters = new Dictionary<string, object>();
                    foreach (var i in Items)
                    {
                        if (i.FromScope)
                        {
                            sb.Append("[p");
                            sb.Append(p);
                            sb.Append("]");
                            parameters.Add("p" + p, Parent.GetValueFromScope(i.Value) ?? OnNullOrNotFound);
                            p++;
                        }
                        else
                        {
                            sb.Append(i.Value);
                        }
                    }

                    var s = sb.ToString();
                    if (string.IsNullOrWhiteSpace(s)) return "";
                    var e = new Expression(s.Replace("&gt;", ">").Replace("&lt;", "<"), EvaluateOptions.NoCache)
                    {
                        Parameters = parameters
                    };

                    try
                    {
                        var result = e.Evaluate();
                        if (Filter != null)
                        {
                            if (Filter.StartsWith("stringFormat"))
                            {
                                if (!string.IsNullOrEmpty(result.ToString()))
                                {
                                    string format = StringFormatRegex.Match(Filter).Value;
                                    return Filters["stringFormat"](new { Result = result, Format = format });
                                }
                            }
                            else
                            {
                                return Filters[Filter](result);
                            }
                        }

                        return result.ToString();
                    }
                    catch
                    {
                        return "{{ " + OriginalExpression + " }}";
                    }
                }
            }

            private class CExpressionItem
            {
                public bool FromScope { get; set; }
                public string Value { get; set; }
            }
        }

        public class XmlAttribute
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public class XmlNameSpace
        {
            public string Prefix { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
        }

        private class PropertyAccess
        {
            public PropertyAccess(string propertyName)
            {
                var ar = propertyName.Split('[', ']');
                Name = ar[0];
                Children = new List<string>();
                for (var i = 1; i < ar.Length - 1; i++)
                {
                    Children.Add(ar[i]);
                }
            }

            public string Name { get; }
            public List<string> Children { get; }

            public dynamic GetValue(dynamic obj)
            {
                dynamic r = obj;
                foreach (var child in Children)
                {
                    var t = r.GetType();
                    if (t.IsArray || obj is IEnumerable)
                    {
                        int index;
                        int.TryParse(child, out index);
                        r = r[index];
                    }
                    else
                    {
                        r = r[child];
                    }
                }
                return r;
            }
        }

        public enum BufferCommands
        {
            NewElement,
            StringContent,
            NewDocument
        }

        private enum LectureType
        {
            Variable, String, Filter, Unknow, Constant
        }
    }
}
