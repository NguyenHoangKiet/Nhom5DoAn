using FamilyTree.Components;
using FamilyTreeLibrary;
using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;

namespace FamilyTree
{
    public partial class App : Application
    {
        internal const string ApplicationFolderName = "Family.Tree";

        // Class people chua co 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
        public static People FamilyCollection = new People();
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
        public static PeopleCollection Family = FamilyCollection.PeopleCollection;

        public static MainWindow mainWindow;
        // load khi chuong trinh vua bat dau
        // tao. 2 theme trong skin

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        protected override void OnStartup(StartupEventArgs e)
        {
            mainWindow = new MainWindow();
            mainWindow.DetailsControl.Visibility = Visibility.Hidden;
            mainWindow.Tree.Visibility = Visibility.Hidden;

            WelcomeWindow welcome = new WelcomeWindow();
            welcome.ShowDialog();
            //Tet();
            base.OnStartup(e);
        }

        void Tet()
        {
            Person person = new Person();
            person.FirstName = "Hồng";
            person.MiddleName = "Trường";
            person.LastName = "Vinh";
            person.Gender = Gender.Male;
            person.BirthDate = new DateTime(2002, 6, 6);
            person.BirthPlace = "Nam Xuan, Krong No, Dak Nong";
            Contact x = new Contact();
            x.Phone = "19008198";
            //x.Address.Address1 = "UIT"; có bug
            x.Mail = "hongvinhkrn@gmail.com";
            person.Contact = x;
            Family.Current = person;
        }

        public static TimeSpan GetAnimationDuration(double milliseconds)
        {
            return TimeSpan.FromMilliseconds(Keyboard.IsKeyDown(Key.F12) ? milliseconds * 5 : milliseconds);
        }

        internal static DateTime StringToDate(string dateString)
        {
            if (dateString.Length == 4)
            {
                dateString = "1/1/" + dateString;
            }

            DateTime.TryParse(dateString, out DateTime date);

            return date;
        }
    }
}