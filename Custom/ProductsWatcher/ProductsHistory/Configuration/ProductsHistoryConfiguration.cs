using System.Collections.Generic;
using Radiant.Custom.ProductsWatcher.ProductsHistory.Parsers;
using Radiant.Custom.ProductsWatcher.ProductsHistory.Scraper;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.Configuration;

namespace Radiant.Custom.ProductsWatcher.ProductsHistory.Configuration
{
    public class ProductsHistoryConfiguration
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public List<ProductDOMParserItem> DOMParserItems { get; set; }
            //= new()
            //{
            //    // Default parsers from most common shops
            //    new ProductDOMParserItem
            //    {
            //        IfUrlContains = "amazon.c",
            //        RegexPattern = @"priceblock_ourprice(.*?)<\/span>",
            //        Target = DOMParserItem.DOMParserItemResultTarget.Value,
            //        SubParserToExecuteOnTargetValue = new DOMParserItem
            //        {
            //            IfUrlContains = "amazon.c",
            //            RegexPattern = @"[\d,]+\.\d+",
            //            Target = DOMParserItem.DOMParserItemResultTarget.Value
            //        }
            //    },
            //    new ProductDOMParserItem
            //    {
            //        IfUrlContains = "newegg.c",
            //        RegexPattern = @"\""price\"":\""(.*?)[\""|,]",
            //        Target = DOMParserItem.DOMParserItemResultTarget.Value,
            //        SubParserToExecuteOnTargetValue = new DOMParserItem
            //        {
            //            IfUrlContains = "newegg.c",
            //            RegexPattern = @"[\d,]+\.\d+",
            //            Target = DOMParserItem.DOMParserItemResultTarget.Value
            //        }
            //    },
            //    new ProductDOMParserItem
            //    {
            //        IfUrlContains = "bestbuy.c",
            //        RegexPattern = @"priceblock_ourprice(.*?)<\/span>",
            //        Target = DOMParserItem.DOMParserItemResultTarget.Value,
            //        SubParserToExecuteOnTargetValue = new DOMParserItem
            //        {
            //            IfUrlContains = "bestbuy.c",
            //            RegexPattern = @"[\d,]+\.\d+",
            //            Target = DOMParserItem.DOMParserItemResultTarget.Value
            //        }
            //    }
            //};
        public List<ManualScraperProductParser> ManualScraperSequenceItems { get; set; }
            //= new()
            //{
            //    new ManualScraperProductParser
            //    {
            //        Target = ProductParserItemTarget.Price,
            //        ManualScraperSequenceItems = new List<ManualScraperSequenceItem>
            //    {
            //        new ManualScraperSequenceItemByInput()
            //        {
            //            InputType = InputsManager.InputType.Keyboard,
            //            InputParam = new KeyboardKeyStrokeActionInputParam
            //            {
            //                Delay = 89,
            //                KeyStrokeCodes = new [] { Keycode.CtrlL, Keycode.XK_f }
            //            },
            //            WaitMsOnEnd = 450
            //        }, new ManualScraperSequenceItemByInput()
            //        {
            //            InputType = InputsManager.InputType.Keyboard,
            //            InputParam = new KeyboardTypeActionInputParam
            //            {
            //                Delay = 115,
            //                ValueToType = "price:"
            //            },
            //            WaitMsOnEnd = 1525
            //        }, new ManualScraperSequenceItemByInput()
            //        {
            //            InputType = InputsManager.InputType.Keyboard,
            //            InputParam = new KeyboardKeyStrokeActionInputParam
            //            {
            //                Delay = 290,
            //                KeyStrokeCodes = new [] { Keycode.XK_Escape }
            //            },
            //            WaitMsOnEnd = 625
            //        }, new ManualScraperSequenceItemByInput()
            //        {
            //            InputType = InputsManager.InputType.Keyboard,
            //            InputParam = new KeyboardKeyStrokeActionInputParam
            //            {
            //                Delay = 321,
            //                KeyStrokeCodes = new [] { Keycode.XK_Shift_L, Keycode.XK_Right }
            //            },
            //            WaitMsOnEnd = 422
            //        }, new ManualScraperSequenceItemByInput()
            //        {
            //            InputType = InputsManager.InputType.Keyboard,
            //            InputParam = new KeyboardKeyStrokeActionInputParam
            //            {
            //                Delay = 159,
            //                KeyStrokeCodes = new [] { Keycode.XK_Shift_L, Keycode.XK_End }
            //            },
            //            WaitMsOnEnd = 522
            //        }, new ManualScraperSequenceItemByClipboard()
            //        {
            //            Operation = ManualScraperSequenceItemByClipboard.ClipboardOperation.Set,
            //            Value = "",
            //            WaitMsOnEnd = 625
            //        }, new ManualScraperSequenceItemByInput()
            //        {
            //            InputType = InputsManager.InputType.Keyboard,
            //            InputParam = new KeyboardKeyStrokeActionInputParam
            //            {
            //                Delay = 273,
            //                KeyStrokeCodes = new [] { Keycode.CtrlL, Keycode.XK_c }
            //            },
            //            WaitMsOnEnd = 1714
            //        }, new ManualScraperSequenceItemByClipboard()
            //        {
            //            Operation = ManualScraperSequenceItemByClipboard.ClipboardOperation.Get,
            //            WaitMsOnEnd = 1314
            //        }
            //    },
            //        ValueParser = new ProductDOMParserItem()
            //        {
            //            RegexPattern = @"[\d,]+\.\d+",
            //            Target = DOMParserItem.DOMParserItemResultTarget.Group1Value,
            //            ParserItemTarget = ProductParserItemTarget.Price,// optional as the container is of type Price anyway
            //        }
            //    }
            //};

            public GoogleDriveAPIConfig GoogleDriveAPIConfig { get; set; } = new();

            public GoogleSheetAPIConfig GoogleSheetAPIConfig { get; set; } = new();

            public GoogleSheetProductsExportData GoogleSheetProductsExportData { get; set; } = new();
    }
}
