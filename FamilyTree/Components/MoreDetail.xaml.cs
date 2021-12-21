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
using System.Windows.Shapes;
using FamilyTreeLibrary;
using System.IO;
using Microsoft;
using MaterialDesignThemes.Wpf;
using FamilyTree.Components;

namespace FamilyTree
{
    /// <summary>
    /// Interaction logic for MoDetail.xaml
    /// </summary>
    public partial class MoreDetail : Window
    {
        public Person personobj { get; set; }
        public MoreDetail()
        {
            InitializeComponent();
            personobj = new Person
            {
                FirstName = "Nguyễn",
                MiddleName = "Hoàng",
                LastName = "Kiệt",
                Gender = Gender.Male,
                BirthPlace = "Cao lãnh",
                BirthDate = DateTime.Now,
                IsLiving = false,
                DeathPlace = "Cao lANH",
                DeathDate = DateTime.Now,
            };
            personobj.Contact = new Contact();
            personobj.Contact.Phone = "03220322";
            personobj.Contact.Mail = "skill1sp2@gmail.com";

            //this.DataContext = personobj;

            #region Load property of person into UI

            // Load Name
            textboxFirstName.Text = personobj.FirstName;
            textboxMiddleName.Text = personobj.MiddleName;
            textboxLastName.Text = personobj.LastName;

            // Load Gender
            if (personobj.Gender == Gender.Male)
            {
                genderToggleButton.IsChecked = true;
            }
            else
            {
                genderToggleButton.IsChecked = false;
            }
            // Load birth place
            textboxPlaceofBirth.Text = personobj.BirthPlace;
            // Load birth date
            DatePickerofBirth.Text = personobj.BirthDate.ToString();
            // Load Contact
            if (personobj.Contact != null)
            {
                if (personobj.Contact.Mail != null)
                {
                    textboxGmail.Text = personobj.Contact.Mail;
                }
                if (personobj.Contact.Phone != null)
                {
                    textboxPhoneNum.Text = personobj.Contact.Phone;
                }
                if (personobj.Contact.Address != null)
                {
                    textboxAddress.Text = personobj.Contact.Address.Address1;
                }
            }

            // Load Death or not
            if (personobj.IsLiving == true)
            {
                deathToggleButton.IsChecked = false;
            }
            else
            {
                deathToggleButton.IsChecked = true;
            }
            // Load Death place
            textboxPlaceofDeath.Text = personobj.DeathPlace;
            // Load Death date
            DatePickerDeath.Text = personobj.DeathDate.ToString();

            // Load Photo
            if (personobj.Photos != null)
            {
                foreach (Photo image in personobj.Photos)
                {
                    photoBox.Source = new BitmapImage(new Uri(image.RelativePath));
                }
            }
            #endregion
        }

        public MoreDetail(Person currentPerson)
        {
            InitializeComponent();

            if (currentPerson == null)
            {
                currentPerson = new Person();
            }

            if (currentPerson != null)
            {
                personobj = currentPerson;

                if (currentPerson.FirstName != null)
                {
                    textboxFirstName.Text = currentPerson.FirstName;
                }

                if (currentPerson.MiddleName != null)
                {
                    textboxMiddleName.Text = currentPerson.MiddleName;
                }

                if (currentPerson.LastName != null)
                {
                    textboxLastName.Text = currentPerson.LastName;
                }

                if (currentPerson.Gender == Gender.Male)
                {
                    genderToggleButton.IsChecked = true;
                }
                else
                {
                    genderToggleButton.IsChecked = false;
                }

                if (currentPerson.BirthPlace != null)
                {
                    textboxPlaceofBirth.Text = currentPerson.BirthPlace;
                }

                if (currentPerson.BirthDate != null)
                {
                    DatePickerofBirth.Text = currentPerson.BirthDate.ToString();
                }

                if (currentPerson.Contact != null)
                {
                    if (currentPerson.Contact.Mail != null)
                    {
                        textboxGmail.Text = currentPerson.Contact.Mail;
                    }
                    if (currentPerson.Contact.Phone != null)
                    {
                        textboxPhoneNum.Text = currentPerson.Contact.Phone;
                    }
                    if (currentPerson.Contact.Address.Address1 != null)
                    {
                        textboxAddress.Text = currentPerson.Contact.Address.Address1;
                    }
                }

                if (currentPerson.IsLiving == true)
                {
                    deathToggleButton.IsChecked = false;
                }
                else
                {
                    deathToggleButton.IsChecked = true;
                }
                // Load Death place
                textboxPlaceofDeath.Text = currentPerson.DeathPlace;
                // Load Death date
                DatePickerDeath.Text = currentPerson.DeathDate.ToString();

                // Load Photo
                if (currentPerson.Photos != null)
                {
                    foreach (Photo image in currentPerson.Photos)
                    {
                        photoBox.Source = new BitmapImage(new Uri(image.RelativePath));
                    }
                }
            }
        }


        #region Event update property of person

        // Update Name
        private void textboxFirstName_TextChanged(object sender, TextChangedEventArgs e)
        {
            App.Family.Current.FirstName = textboxFirstName.Text;
        }
        private void textboxMiddleName_TextChanged(object sender, TextChangedEventArgs e)
        {
            App.Family.Current.MiddleName = textboxMiddleName.Text;
        }
        private void textboxLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            App.Family.Current.LastName = textboxLastName.Text;
        }

        // Update gender
        private void genderToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.Family.Current.Gender == Gender.Male)
            {
                App.Family.Current.Gender = Gender.Female;
            }
            else
            {
                App.Family.Current.Gender = Gender.Male;
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
            }
        }
        // Update place of birth
        private void textboxPlaceofBirth_TextChanged(object sender, TextChangedEventArgs e)
        {
            App.Family.Current.BirthPlace = textboxPlaceofBirth.Text;
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
                
            }
            else
            {
                personobj.DeathDate = date;
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
            App.Family.Current = personobj;

            this.Close();
        }
    }
}
