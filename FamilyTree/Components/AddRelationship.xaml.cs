﻿using System;
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
using FamilyTree;

namespace FamilyTree.Components
{
    /// <summary>
    /// Interaction logic for AddRelationship.xaml
    /// </summary>
    public partial class AddRelationship : Window
    {
        Person newPerson;

        Gender _gender;

        int _index;

        public AddRelationship()
        {
            InitializeComponent();
        }

        public AddRelationship(int index, Gender gender)
        {
            InitializeComponent();

            _gender = gender;
            _index = index;

            if(index == 1 || index == 2)
            { 
                tbFirstname.Text = null;
            }
            else
            {
                tbFirstname.Text = App.Family.Current.FirstName;
            }

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void addPerson_Click(object sender, RoutedEventArgs e)
        {
            newPerson = new Person(tbFirstname.Text != null ? tbFirstname.Text : null, tbLastName.Text != null ? tbLastName.Text : null, _gender);

            if (tbBirthDay.SelectedDate != null)
            {
                newPerson.BirthDate = tbBirthDay.SelectedDate;
            }

            if (tbPlaceOfBirth.Text != null)
            {
                newPerson.BirthPlace = tbPlaceOfBirth.Text;
            }

            if (_index == 0)
            {
                // add father
                RelationshipHelper.AddParent(FamilyTree.App.Family, FamilyTree.App.Family.Current, newPerson);
            }
            else if(_index == 1)
            {
                // add mother
                RelationshipHelper.AddParent(FamilyTree.App.Family, FamilyTree.App.Family.Current, newPerson);
            }
            else if (_index == 2)
            {
                // add spause
                RelationshipHelper.AddSpouse(FamilyTree.App.Family, FamilyTree.App.Family.Current, newPerson, SpouseModifier.Current);
            }
            else if (_index == 3)
            {
                // add sister
                RelationshipHelper.AddSibling(FamilyTree.App.Family, FamilyTree.App.Family.Current, newPerson);
            }
            else if (_index == 4)
            {
                // add brother
                RelationshipHelper.AddSibling(FamilyTree.App.Family, FamilyTree.App.Family.Current, newPerson);
            }
            else if (_index == 5)
            {
                // add daught
                RelationshipHelper.AddChild(FamilyTree.App.Family, FamilyTree.App.Family.Current, newPerson);
            }
            else if (_index == 6)
            {
                // add son
                RelationshipHelper.AddChild(FamilyTree.App.Family, FamilyTree.App.Family.Current, newPerson);
            }
            else
            {
                MessageBox.Show("error");return;
            }


        }
    }
}