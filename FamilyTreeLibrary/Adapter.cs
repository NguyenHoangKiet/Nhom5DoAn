using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyTreeLibrary
{
    public class Adapter
    {
        #region Make SingleTon
        private static Adapter instance;

        static object key = new object();

        public static Adapter Ins
        {
            get
            {
                if (instance == null)
                {
                    lock (key)
                    {
                        instance = new Adapter();
                    }
                }

                return instance;
            }
        }

        private Adapter() { }
        #endregion

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
        public static People FamilyCollection = new People();
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
        public static PeopleCollection Family = FamilyCollection.PeopleCollection;
        
        //public string fileName;

        #region Convert To file GedCOM

        public void ConvertToGedCOM(string fileName)
        {
            var st = new FileStream(fileName, FileMode.Create);
            st.Close();

            PeopleCollection Family = FamilyCollection.PeopleCollection;

            int number_family = 1;

            foreach (var person in Family)
            {
                AddPersonToGedCOM(fileName, person);
            }

            foreach (var person in Family)
            {
                AddFamilyToGedCOM(fileName, number_family);
                number_family++;
            }
        }

        void AddPersonToGedCOM(string fileName, Person person)
        {
            if (person == null) return;

            //0 @I2@ INDI
            AddNewLine(fileName, "0 @" + person.Id  +"@ INDI");

            //1 NAME Charles Phillip / Ingalls
            AddNewLine(fileName, "1 NAME " + person.FullName);

            //1 SEX M
            if (person.Gender == Gender.Male)
            {
                AddNewLine(fileName, "1 SEX M");
            }
            else if (person.Gender == Gender.Female)
            {
                AddNewLine(fileName, "1 SEX F");
            }
            else
            {
                AddNewLine(fileName, "1 SEX UNKNOW");
            }

            //1 BIRT
            AddNewLine(fileName, "1 BIRT ");

            if (person.BirthDate != null)
            {
                //2 DATE 10 JAN 1836
                AddNewLine(fileName, "2 DATE " + person.BirthDate?.ToString("dd MMM yyyy").ToUpper());

                //2 PLAC Cuba, Allegheny, NY
                AddNewLine(fileName, "2 PLAC " + person.BirthPlace);
            }

            if (person.IsLiving==false)
            {
                //1 DEAT
                AddNewLine(fileName, "DEAT ");

                //2 DATE 08 JUN 1902
                AddNewLine(fileName, "2 DATE " + person.DeathDate?.ToString("dd MMM yyyy").ToUpper());

            }


            //1 FAMC @F2@
            //AddNewLine(fileName, person.Name);

            //1 FAMS @F3@
            //AddNewLine(fileName, person.Name);
        }

        void AddNewLine(string fileName, string text)
        {
            TextWriter tw = new StreamWriter(fileName, true);

            tw.WriteLine(text);

            tw.Close();
        }

        void AddFamilyToGedCOM(string fileName, int number)
        {
            string text = "";

            AddNewLine(fileName, text);
        }

        #endregion
    }
}
