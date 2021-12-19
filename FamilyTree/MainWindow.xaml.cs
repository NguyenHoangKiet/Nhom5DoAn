using FamilyTree.Components;
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

namespace FamilyTree
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        private void ThemeDefaul_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ResourceDictionary dict = new ResourceDictionary();
                dict.Source = new Uri("..\\Resource\\ResourceTheme.default.xaml", UriKind.Relative);
                Application.Current.Resources.MergedDictionaries.Add(dict);
            }
            catch
            {

            }
        }

        private void ThemeLight_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ResourceDictionary dict = new ResourceDictionary();
                dict.Source = new Uri("..\\Resource\\ResourceTheme.light.xaml", UriKind.Relative);
                Application.Current.Resources.MergedDictionaries.Add(dict);
            }
            catch
            {

            }
        }

        private void MenuItem_About_Click(object sender, RoutedEventArgs e)
        {
            string text = "Xin chào!\n" +
                "Team phát triển phần mềm gia phả này bao gồm:\n" +
                "   -Trần Quốc Thắng\n" +
                "   -Nguyễn Hoàng Kiệt\n" +
                "   -Ngô Quang Vũ\n" +
                "   -Lê Minh Quân\n" +
                "   -Hồng Trường Vinh\n" +
                "\n Cảm ơn các bạn rất nhiều vì đã sử sụng phần mềm của chúng tôi.\n" +
                "\n\n Phiên bản 1.0.0";

            MessageBox.Show(text);
           
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}