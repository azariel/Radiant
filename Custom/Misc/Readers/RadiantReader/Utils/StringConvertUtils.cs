using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
            em
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static List<Inline> GetInlinesFromString(string aChapterContent)
        {
            string _FilteredContent = aChapterContent.Replace("</p>", "", StringComparison.InvariantCultureIgnoreCase);

            string[] _Lines = _FilteredContent.Split(new[]
            {
                Environment.NewLine,
                "<p>",
                "<P>",
            }, StringSplitOptions.RemoveEmptyEntries);

            List<Inline> _Inlines = new();

            _Lines = new[] { "in<p>this is <strong>strong</strong> hmkay. <em>italic</em> dayum</p>out" };
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
                foreach (Inline _ChildInline in _ChildInlines)
                {
                    if (Enum.TryParse(aNode.Name.ToLowerInvariant(), out HtmlElementName _Element))
                    {
                        switch (_Element)
                        {
                            case HtmlElementName.strong:
                                _Inlines.Add(new Bold(_ChildInline));
                                break;
                            case HtmlElementName.em:
                                _Inlines.Add(new Italic(_ChildInline));
                                break;
                            default:
                                throw new ArgumentOutOfRangeException($"Unhandled element [{_Element}].");
                        }
                    } else
                        _Inlines.Add(_ChildInline);
                }
            } else
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
                        default:
                            throw new ArgumentOutOfRangeException($"Unhandled element [{_Element}].");
                    }
                } else
                    _Inlines.Add(new Run(aNode.InnerText));
            }


            return _Inlines;
        }
    }
}


