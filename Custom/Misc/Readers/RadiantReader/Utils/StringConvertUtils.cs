using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Xml;

namespace RadiantReader.Utils
{
    public static class StringConvertUtils
    {
        // ********************************************************************
        //                            Nested
        // ********************************************************************
        private enum HtmlElementName
        {
            strong,
            em,
            p
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static List<Inline> GetInlinesFromString(string aChapterContent)
        {
            string[] _Lines = aChapterContent.Split(new[]
            {
                Environment.NewLine,
            }, StringSplitOptions.RemoveEmptyEntries);

            List<Inline> _Inlines = new();

            foreach (string _Line in _Lines)
            {
                var _WrappedLine = $"<Body>{_Line}</Body>";
                var _XmlDocument = new XmlDocument();
                _XmlDocument.LoadXml(_WrappedLine);

                _Inlines.AddRange(ConvertStringToInline(_XmlDocument));
                _Inlines.Add(new LineBreak());
            }

            return _Inlines;
        }

        public static string GetStringFromInlines(List<Inline> aInlines)
        {
            var _StringBuilder = new StringBuilder();

            foreach (Inline _Inline in aInlines)
            {
                if (_Inline is LineBreak)
                {
                    _StringBuilder.Append(Environment.NewLine);
                    continue;
                }

                _StringBuilder.Append(_Inline);
            }

            return _StringBuilder.ToString();
        }

        private static List<Inline> ConvertStringToInline(XmlNode aNode)
        {
            List<Inline> _ChildInlines = new();

            for (int i = 0; i < aNode.ChildNodes.Count; i++)
            {
                var _CurrentChildNode = aNode.ChildNodes[i];
                _ChildInlines.AddRange(ConvertStringToInline(_CurrentChildNode));
            }

            List<Inline> _Inlines = new();
            if (_ChildInlines.Any())
            {
                HtmlElementName? _ElementName = null;
                if (Enum.TryParse(aNode.Name.ToLowerInvariant(), out HtmlElementName _Element))
                    _ElementName = _Element;

                if (_ElementName == HtmlElementName.p)
                    _Inlines.Add(new LineBreak());// Handle <p>

                foreach (Inline _ChildInline in _ChildInlines)
                {
                    if (_ElementName != null)
                    {
                        switch (_ElementName)
                        {
                            case HtmlElementName.strong:
                                _Inlines.Add(new Bold(_ChildInline));
                                break;
                            case HtmlElementName.em:
                                _Inlines.Add(new Italic(_ChildInline));
                                break;
                            case HtmlElementName.p:
                                _Inlines.Add(_ChildInline);// Just add the child as is

                                if (_ChildInline == _ChildInlines.Last())
                                    _Inlines.Add(new LineBreak());// Handle </p>

                                break;// P element is adding a linebreak before the loop as the operation isn't applied on each child
                            default:
                                throw new ArgumentOutOfRangeException($"Unhandled element [{_ElementName}].");
                        }
                    }
                    else
                        _Inlines.Add(_ChildInline);
                }
            }
            else
            {
                if (Enum.TryParse(aNode.Name.ToLowerInvariant(), out HtmlElementName _Element))
                {
                    switch (_Element)
                    {
                        case HtmlElementName.strong:
                            _Inlines.Add(new Bold(new Run(aNode.InnerText)));
                            break;
                        case HtmlElementName.em:
                            _Inlines.Add(new Italic(new Run(aNode.InnerText)));
                            break;
                        case HtmlElementName.p:
                            _Inlines.Add(new LineBreak());
                            _Inlines.Add(new Run(aNode.InnerText));// Just add the child as is

                            break;// P element is adding a linebreak before the loop as the operation isn't applied on each child
                        default:
                            throw new ArgumentOutOfRangeException($"Unhandled element [{_Element}].");
                    }
                }
                else
                    _Inlines.Add(new Run(aNode.InnerText));
            }


            return _Inlines;
        }
    }
}


