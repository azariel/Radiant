using Radiant.Common.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
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
        //                            Private
        // ********************************************************************
        private static Regex HrFinderRegex = new Regex("<hr.*?>");

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static List<Inline> GetInlinesFromString(string aChapterContent)
        {
            List<Inline> _Inlines = new();
            string _ChapterContent = aChapterContent
                                     .Replace("<br>", "<br />")
                                     .Replace("<hr>", "<hr />")
                                     .Replace("&", "and")
                                     .Replace("<center>", "<center />")
                                     .Replace("amp;", "")
                                     .Replace("nbsp;", " ");

            _ChapterContent = HrFinderRegex.Replace(_ChapterContent, "");

            // Replace tags by regex
            // ex: <hr size="1" noshade=""> should become <hr />
            //foreach (var _RegexMatch in HrFinderRegex.Matches(_ChapterContent))
            //{

            //}

            try
            {
                var _WrappedLine = $"<RadiantReader>{_ChapterContent}</RadiantReader>";
                var _XmlDocument = new XmlDocument();
                _XmlDocument.LoadXml(_WrappedLine);

                _Inlines.AddRange(ConvertStringToInline(_XmlDocument));
                _Inlines.Add(new LineBreak());

                return _Inlines;
            }
            catch (Exception _Exception)
            {
                LoggingManager.LogToFile("e4234d71-d663-4eda-9448-58deab1cb3ac", $"Couldn't convert string to XML object. Full Text [{_ChapterContent}].", _Exception);
                return new List<Inline>()
                {
                    new Run("[Couldn't load XML object. Check logs for more infos.]")
                };
            }
        }

        public static string GetStringFromInlines(List<Inline> aInlines)
        {
            var _StringBuilder = new StringBuilder();

            Type _LastInlineType = null;
            foreach (Inline _Inline in aInlines)
            {
                if (_Inline is LineBreak && _LastInlineType != typeof(LineBreak))
                {
                    if (_StringBuilder.Length > 0)// We want to avoid spacing up anything before start of text values
                        _StringBuilder.Append(Environment.NewLine);

                    _LastInlineType = _Inline.GetType();
                    continue;
                }

                if (_Inline is Run _InlineRun)
                    _StringBuilder.Append(_InlineRun.Text);

                _LastInlineType = _Inline.GetType();
            }

            return _StringBuilder.ToString();
        }

        private static List<Inline> ConvertStringToInline(XmlNode aNode)
        {
            List<Inline> _ChildInlines = new();

            for (int i = 0; i < aNode.ChildNodes.Count; i++)
            {
                var _CurrentChildNode = aNode.ChildNodes[i];

                // Ignore head content
                if (_CurrentChildNode.Name.ToLowerInvariant() == "head")
                {
                    continue;
                }

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


