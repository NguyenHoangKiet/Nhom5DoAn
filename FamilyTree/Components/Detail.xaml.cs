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
using FamilyTree.Components;

namespace FamilyTree.Components
{
    /// <summary>
    /// Interaction logic for Detail.xaml
    /// </summary>
    public partial class Detail : UserControl
    {

        public Detail()
        {
            InitializeComponent();

        }

        private void bnt_view_edit_Click(object sender, RoutedEventArgs e)
        {
            MoreDetail moreDetail = new MoreDetail();
            moreDetail.Width = 470;
            moreDetail.Height = 700;
            moreDetail.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            moreDetail.ShowDialog();
        }
    }
}
