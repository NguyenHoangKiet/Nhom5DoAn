using FamilyTreeLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace FamilyTree.Components
{
    /// <summary>
    /// Interaction logic for FamilyData.xaml
    /// </summary>
    public partial class FamilyData : Window
    {
        public PeopleCollection peopleCollection;
        public FamilyData()
        {
            InitializeComponent();
        }
        public FamilyData(PeopleCollection people)
        {
            InitializeComponent();

            peopleCollection = new PeopleCollection();

            peopleCollection = people;

            listviewMember.ItemsSource = peopleCollection;

            foreach (Person person in peopleCollection)
            {
                if (person.Gender == Gender.Male)
                {
                    person.Gender = Gender.Nam;
                }
                else if (person.Gender == Gender.Female)
                {
                    person.Gender = Gender.Nữ;
                }
                else
                {

                }
            }

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listviewMember.ItemsSource);
            view.Filter = UserFilter;
        }

        private bool UserFilter(object obj)
        {
            if (String.IsNullOrEmpty(txtFilter.Text))
                return true;
            else
                return ((obj as Person).FullName.IndexOf(txtFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void PackIcon_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(listviewMember.ItemsSource).Refresh();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listviewMember.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            CollectionViewSource.GetDefaultView(listviewMember.ItemsSource).Refresh();
        }
    }
}
