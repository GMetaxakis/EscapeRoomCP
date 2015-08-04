using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;

namespace EscapeRoomCP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void HandleDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string message = e.Exception.Message;
            if (e.Exception.InnerException != null)
            {
                message += Environment.NewLine + string.Format("[{0}]", e.Exception.InnerException.Message);
            }

            MessageBox.Show(message, "Exception!", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}
