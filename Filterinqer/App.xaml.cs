using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace Filterinqer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender,
                       System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //Handling the exception within the UnhandledException handler.
            MessageBox.Show(e.Exception.Message, "Ошибка!",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}
