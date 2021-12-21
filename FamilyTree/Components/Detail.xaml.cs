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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using FamilyTree.Components;
using FamilyTreeLibrary;

namespace FamilyTree.Components
{
    /// <summary>
    /// Interaction logic for Detail.xaml
    /// </summary>
    public partial class Detail : UserControl, INotifyPropertyChanged
    {
        private string name;
        string birthDay;
        string bithPalce;

        public Detail()
        {
            InitializeComponent();

            this.DataContext = this;

            //PersonBirthDay = CurrentPerson.currentPerson.BirthDate.ToString();
            //PersonName = CurrentPerson.currentPerson.FullName;
            //PersonBirthPlace = CurrentPerson.currentPerson.BirthPlace;

            LoadPerson();
            StarTimer();
        }

        void LoadPerson()
        {
            if (App.Family.Current != null)
            {
                PersonName = App.Family.Current.FullName;
                //PersonBirthDay = App.Family.Current.BirthDate.ToString("MM/dd/yyyy");
                PersonBirthDay = App.Family.Current.BirthDate.ToString();
                PersonBirthPlace = App.Family.Current.BirthPlace;

            }
        }

        void StarTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            LoadPerson();
        }

        public string PersonName {
            get
            {
                return name;
            }

            set
            {
                name = value;
                OnPropertyChanged("PersonName");
            }
        }

        public string PersonBirthDay
        {
            get
            {
                return birthDay;
            }

            set
            {
                birthDay = value;
                OnPropertyChanged("PersonBirthDay");
            }
        }

        public string PersonBirthPlace
        {
            get
            {
                return bithPalce;
            }

            set
            {
                bithPalce = value;
                OnPropertyChanged("PersonBirthPlace");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string newName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(newName));
            }
        }

        private void bnt_view_edit_Click(object sender, RoutedEventArgs e)
        {
            
            MoreDetail moreDetail = new MoreDetail(App.Family.Current);

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

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (cbbAdd == null) return;

            AddRelationship addRelationship;

            switch (cbbAdd.SelectedIndex)
            {
                case 0://add Father
                    addRelationship = new AddRelationship(0,Gender.Male);
                    addRelationship.ShowDialog();
                    break;
                case 1://add mother
                    addRelationship = new AddRelationship(1, Gender.Female);
                    addRelationship.ShowDialog();
                    break;
                case 2://add spause
                    addRelationship = new AddRelationship(2, Gender.Female);
                    addRelationship.ShowDialog();
                    break;
                case 3://add sister
                    addRelationship = new AddRelationship(3, Gender.Female);
                    addRelationship.ShowDialog();
                    break;
                case 4:// add brother
                    addRelationship = new AddRelationship(4, Gender.Male);
                    addRelationship.ShowDialog();
                    break;
                case 5://add daughter
                    addRelationship = new AddRelationship(5, Gender.Female);
                    addRelationship.ShowDialog();
                    break;
                case 6:// add son
                    addRelationship = new AddRelationship(6, Gender.Male);
                    addRelationship.ShowDialog();
                    break;
            }
        }

        private void DeletePerson_Click(object sender, RoutedEventArgs e)
        {
            RelationshipHelper.DeletePerson(App.Family, App.Family.Current);

            if (App.Family.Count > 0)
            {
                App.Family.Current = App.Family[0];

                App.Family.OnContentChanged();
            }
        }
    }
}
