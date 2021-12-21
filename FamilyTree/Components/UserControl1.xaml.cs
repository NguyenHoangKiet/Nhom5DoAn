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
using FamilyTreeLibrary;
using MaterialDesignThemes.Wpf;

namespace FamilyTree.Components
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();

        }

        #region Detail
        private void bnt_view_edit_Click(object sender, RoutedEventArgs e)
        {
            MoreDetail moreDetail = new MoreDetail();
            moreDetail.Width = 470;
            moreDetail.Height = 700;
            moreDetail.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            moreDetail.ShowDialog();
        }

        private void btn_view_Click(object sender, RoutedEventArgs e)
        {
            FamilyData familyData = new FamilyData(App.Family);
            familyData.ShowDialog();
        }
        #endregion

        #region MoreDetail

        public Person personobj { get; set; }

        #region Event update property of person

        // Update Name
        private void textboxFirstName_TextChanged(object sender, TextChangedEventArgs e)
        {
            personobj.FirstName = textboxFirstName.Text;
        }
        private void textboxMiddleName_TextChanged(object sender, TextChangedEventArgs e)
        {
            personobj.MiddleName = textboxMiddleName.Text;
        }
        private void textboxLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            personobj.LastName = textboxLastName.Text;
        }

        // Update gender
        private void genderToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (personobj.Gender == Gender.Male)
            {
                personobj.Gender = Gender.Female;
            }
            else
            {
                personobj.Gender = Gender.Male;
            }

        }
        // Update Address
        private void textboxAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (personobj.Contact == null)
            {
                personobj.Contact = new Contact();
            }
            if (personobj.Contact.Address == null)
            {
                personobj.Contact.Address = new Address();
            }
            personobj.Contact.Address.Address1 = textboxAddress.Text;
        }
        // Update day of birth
        private void DatePickerofBirth_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var picker = sender as DatePicker;
            DateTime? date = picker.SelectedDate;

            if (date == null)
            {
                //
            }
            else
            {
                personobj.BirthDate = date;
                // this.Title = date.Value.ToShortDateString();
            }
        }
        // Update place of birth
        private void textboxPlaceofBirth_TextChanged(object sender, TextChangedEventArgs e)
        {
            personobj.BirthPlace = textboxPlaceofBirth.Text;
        }
        // Update Contact phone
        private void textboxPhoneNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (personobj.Contact == null)
            {
                personobj.Contact = new Contact();
            }
            personobj.Contact.Phone = textboxPhoneNum.Text;
        }
        // Update Contact mail
        private void textboxGmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (personobj.Contact == null)
            {
                personobj.Contact = new Contact();
            }
            personobj.Contact.Mail = textboxGmail.Text;
        }
        // Update Death or not
        private void deathToggleButton_Click(object sender, RoutedEventArgs e)
        {
            personobj.IsLiving = !personobj.IsLiving;
        }
        // Update day of death
        private void DatePickerDeath_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var picker = sender as DatePicker;
            DateTime? date = picker.SelectedDate;

            if (date == null)
            {
                //this.Title = "No date";
            }
            else
            {
                personobj.DeathDate = date;
                //this.Title = date.Value.ToShortDateString();
            }
        }
        // Update place of death
        private void textboxPlaceofDeath_TextChanged(object sender, TextChangedEventArgs e)
        {
            personobj.DeathPlace = textboxPlaceofDeath.Text;
        }

        // Update Avatar
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

                PackIcon getIcon = icon;
                if (getIcon != null)
                {
                    icon.Visibility = Visibility.Collapsed;
                }
            }
        }
        #endregion

        void Save_Click(object sender, RoutedEventArgs e)
        {
            
        }
        #endregion

    }
}
