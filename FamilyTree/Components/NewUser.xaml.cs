using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FamilyTreeLibrary;
using MaterialDesignThemes.Wpf;

namespace FamilyTree.Components
{
    
    /// <summary>
    /// Interaction logic for NewUser.xaml
    /// </summary>
    public partial class NewUser : Window
    {
        Person personobj;
        public NewUser()
        {
            InitializeComponent();
            personobj = new Person();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void addNewUser_Click(object sender, RoutedEventArgs e)
        {
            Gender gender;

            if (radiobtnMale.IsChecked == true)
            {
                gender = Gender.Male;
            }
            else if (radiobtnFemale.IsChecked == true)
            {
                gender = Gender.Female;
            }
            else
            {
                gender = Gender.unknown;
            }

            if(tbFirstname.Text == null)
            {
                personobj.FirstName = null;
            }
            if(tbLastName.Text == null)
            {
                personobj.LastName = null;
            }
      
            personobj.Gender = gender;

            if (tbBirthDay.SelectedDate != null)
            {
                personobj.BirthDate = tbBirthDay.SelectedDate;
            }

            if (tbPlaceOfBirth.Text != null)
            {
                personobj.BirthPlace = tbPlaceOfBirth.Text;
            }
            
            FamilyTree.App.Family.Add(personobj);
            FamilyTree.App.Family.Current = personobj;
            FamilyTree.App.Family.OnContentChanged();

            App.mainWindow.DetailsControl.Visibility = Visibility.Visible;
            App.mainWindow.Tree.Visibility = Visibility.Visible;
            this.Close();
        }
        
        private void btnGetPhoto_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog op = new Microsoft.Win32.OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                photoBox.Source = new BitmapImage(new Uri(op.FileName));
                personobj.Photos.Add(new Photo(op.FileName));
                personobj.Avatar = op.FileName;
                PackIcon getIcon = icon;
                if (getIcon != null)
                {
                    icon.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
