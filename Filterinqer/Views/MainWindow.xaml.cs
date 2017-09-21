using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Filterinqer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            var vm = (MainViewModel)DataContext;
            if (e.Key == Key.Z && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && vm.UndoCommand.CanExecute(null))
                vm.UndoCommand.Execute(null);
        }

        private void DoubleAnimation_Completed(object sender, EventArgs e)
        {
            var vm = (MainViewModel)DataContext;
            vm.IsAfterAction = false;
        }
    }
}
