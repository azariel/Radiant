using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Radiant.Common.Business;
using Radiant.Common.Diagnostics;
using Radiant.Common.OSDependent.Clipboard;
using Radiant.WebScraper.Helpers;
using Radiant.WebScraper.Scrapers.Manual;
using RadiantInputsManager;

namespace Radiant.WebScraper.Scrapers.Conditions
{
    public class ManualScraperManualCondition : IScraperCondition
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public bool Evaluate(SupportedBrowser? aBrowser)
        {
            if (this.ManualScraperSequenceItems == null || this.ManualScraperSequenceItems.Count <= 0)
            {
                LoggingManager.LogToFile("99BFB517-56F0-4BCF-989F-0DE4F570983C", $"{nameof(ManualScraperManualCondition)} was empty. Defaulting to valid condition.");
                return true;
            }

            try
            {
                foreach (ManualScraperSequenceItem _ManualScraperSequenceItem in this.ManualScraperSequenceItems)
                {
                    switch (_ManualScraperSequenceItem)
                    {
                        case ManualScraperSequenceItemByClipboard _ManualScraperSequenceItemByClipboard:
                            if (_ManualScraperSequenceItemByClipboard.Operation == ManualScraperSequenceItemByClipboard.ClipboardOperation.Get)
                            {
                                string _Value = ClipboardManager.GetClipboardValue();

                                if (_ManualScraperSequenceItemByClipboard.WaitMsOnEnd > 0)
                                    BrowserHelper.WaitForBrowserInputsReadyOrMax(_ManualScraperSequenceItemByClipboard.WaitMsOnEnd, aBrowser);

                                // Override clipboard value
                                ClipboardManager.SetClipboardValue("");

                                Regex _ValueRegex = new Regex(this.RegexPatternToApplyOnValue, RegexOptions.CultureInvariant);
                                Match _Match = _ValueRegex.Match(_Value);

                                if (!_Match.Success)
                                {
                                    LoggingManager.LogToFile("1BD9877F-5CE6-403D-825A-45E0FEB100EF", $"[{nameof(ManualScraperManualCondition)}] couldn't parse found value [{_Value}] with regex pattern [{this.RegexPatternToApplyOnValue}]. The expected value was [{this.ExpectedValue}]. Condition failed.");
                                    return false;
                                }

                                string _ValueFound;
                                switch (this.Target)
                                {
                                    case RegexItemResultTarget.Value:
                                        _ValueFound = _Value;
                                        break;
                                    case RegexItemResultTarget.Group0Value:
                                        if (_Match.Groups.Count < 1)
                                        {
                                            LoggingManager.LogToFile("EEA3F01B-C930-443E-BEC2-DFFD6E4C8570", $"[{nameof(ManualScraperManualCondition)}] couldn't parse found value [{_Value}] with regex pattern [{this.RegexPatternToApplyOnValue}]. The expected value was [{this.ExpectedValue}]. Match didn't contained at least 1 group. Condition failed.");
                                            return false;
                                        }

                                        _ValueFound = _Match.Groups[0].Value;
                                        break;
                                    case RegexItemResultTarget.Group1Value:
                                        if (_Match.Groups.Count < 2)
                                        {
                                            LoggingManager.LogToFile("59EE703F-F9B0-4BD7-AB57-D944BC4235E7", $"[{nameof(ManualScraperManualCondition)}] couldn't parse found value [{_Value}] with regex pattern [{this.RegexPatternToApplyOnValue}]. The expected value was [{this.ExpectedValue}]. Match didn't contained at least 2 groups. Condition failed.");
                                            return false;
                                        }

                                        _ValueFound = _Match.Groups[1].Value;
                                        break;
                                    case RegexItemResultTarget.LastGroupValue:
                                        if (_Match.Groups.Count < 1)
                                        {
                                            LoggingManager.LogToFile("6E6A3798-D0BD-4B51-88C6-8967A1236D5B", $"[{nameof(ManualScraperManualCondition)}] couldn't parse found value [{_Value}] with regex pattern [{this.RegexPatternToApplyOnValue}]. The expected value was [{this.ExpectedValue}]. Match didn't contained at least 1 group. Condition failed.");
                                            return false;
                                        }

                                        _ValueFound = _Match.Groups[^1].Value;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }

                                if (string.Equals(this.ExpectedValue.Trim(), _ValueFound.Trim(), this.ValueStringComparison))
                                    return this.ConditionValidIfValueFound;

                                return !this.ConditionValidIfValueFound;
                            }

                            break;
                        case ManualScraperSequenceItemByInput _ManualScraperSequenceItemByInput:
                            InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(_ManualScraperSequenceItemByInput.InputType, _ManualScraperSequenceItemByInput.InputParam);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(_ManualScraperSequenceItem));
                    }

                    if (_ManualScraperSequenceItem.WaitMsOnEnd > 0)
                        BrowserHelper.WaitForBrowserInputsReadyOrMax(_ManualScraperSequenceItem.WaitMsOnEnd, aBrowser);
                }
            } catch (Exception _Ex)
            {
                LoggingManager.LogToFile("E5966173-8B0E-4D49-9B4A-9347EF5FF18A", $"Couldn't reproduce steps for manual operation in [{nameof(ManualScraperManualCondition)}].", _Ex);
                throw;
            }

            return false;
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public string ExpectedValue { get; set; }
        public List<ManualScraperSequenceItem> ManualScraperSequenceItems { get; set; } = new();
        public string RegexPatternToApplyOnValue { get; set; } = "(.*)";

        [JsonConverter(typeof(StringEnumConverter))]
        public RegexItemResultTarget Target { get; set; } = RegexItemResultTarget.Value;

        public StringComparison ValueStringComparison { get; set; } = StringComparison.InvariantCultureIgnoreCase;

        /// <summary>
        /// If we searched for "Potatoe" and we found it, is it Valid or Invalid
        /// </summary>
        public bool ConditionValidIfValueFound { get; set; } = true;
    }
}
