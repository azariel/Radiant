using System;
using System.Windows;
using Radiant.Common.Diagnostics;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryClient
{
    public static class ClientException
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static void ThrowNewException(string aUID, string aMessage, Exception aException = null)
        {
            LoggingManager.LogToFile(aUID, aMessage, aException);

            MessageBox.Show(aMessage);
        }
    }
}
