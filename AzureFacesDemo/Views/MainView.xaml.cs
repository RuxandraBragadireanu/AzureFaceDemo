using AzureFacesDemo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AzureFacesDemo.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        MainViewModel vm;

        public MainView()
        {
            InitializeComponent();
        }

        private void ButtonPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (vm != null)
            {
                vm.CurrentImageIndex--;
            }
        }

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            if(vm != null)
            {
                vm.CurrentImageIndex++;
            }
        }

        private void Window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue is MainViewModel)
            {
                vm = (MainViewModel)e.NewValue;
            }
        }

        private void Image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if(vm != null)
            {
                if(e.Delta >= 0)
                {
                    vm.CurrentImageIndex++;
                }
                else
                {
                    vm.CurrentImageIndex--;
                }
            }
        }

    }
}
