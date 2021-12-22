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
        public NewUser()
        {
            InitializeComponent();
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

            Person newPerson = new Person(tbFirstname.Text != null ? tbFirstname.Text : null, tbLastName.Text != null ? tbLastName.Text : null, gender);

            if (tbBirthDay.SelectedDate != null)
            {
                newPerson.BirthDate = tbBirthDay.SelectedDate;
            }

            if (tbPlaceOfBirth.Text != null)
            {
                newPerson.BirthPlace = tbPlaceOfBirth.Text;
            }


            try
            {
                if (photoBox.Source != null)
                {
                    Photo photo = new Photo(photoBox.Source.ToString());

                    photo.IsAvatar = true;

                    if (photo != null)
                    {
                        newPerson.Photos.Add(photo);
                    }
                }
            }
            catch
            {

            }
            FamilyTree.App.Family.Add(newPerson);
            FamilyTree.App.Family.Current = newPerson;
            FamilyTree.App.Family.OnContentChanged();

            App.mainWindow.DetailsControl.Visibility = Visibility.Visible;
            App.mainWindow.Tree.Visibility = Visibility.Visible;
            this.Close();
        }
        string fileAvatar;
        private void btnGetPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    PackIcon getIcon = icon;
                    if (getIcon != null)
                    {
                        icon.Visibility = Visibility.Collapsed;
                    }
                }
                catch
                {

                }
                photoBox.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                fileAvatar = openFileDialog.FileName;
            }
        }
    }
}
