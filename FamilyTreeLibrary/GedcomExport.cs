using System;
using System.Globalization;
using System.IO;

namespace FamilyTreeLibrary
{
    public class GedcomExport
    {
        #region fields
        private TextWriter writer;
        private GedcomIdMap idMap = new GedcomIdMap();
        private PeopleCollection people;
        private int familyId = 1;

        #endregion
        public void Export(PeopleCollection peopleCollection, string gedcomFilePath)
        {
            people = peopleCollection;

            using (writer = new StreamWriter(gedcomFilePath))
            {
                WriteLine(0, "HEAD", "");
                ExportPeople();
                ExportFamilies();
                WriteLine(0, "TRLR", "");
            }
        }
        private void ExportPeople()
        {
            foreach (Person person in people)
            {
                WriteLine(0, string.Format(CultureInfo.InvariantCulture,
                    "@{0}@", idMap.Get(person.Id)), "INDI");
                ExportName(person);
                if (!string.IsNullOrEmpty(person.NickName))
                {
                    WriteLine(2, "NICK", person.NickName);
                }
                ExportGender(person);
                ExportEvent("BIRT", person.BirthDate, person.BirthPlace);
                ExportEvent("DEAT", person.DeathDate, person.DeathPlace);
                ExportPhotos(person);
            }
        }
        private void ExportFamilies()
        {
            FamilyMap map = new FamilyMap();
            map.Create(people);
            foreach (Family family in map.Values)
            {
                ExportFamily(family);
            }
        }
        private void ExportFamily(Family family)
        {
            if (family.ParentRight == null && family.Children.Count == 0)
            {
                return;
            }
            WriteLine(0, string.Format(CultureInfo.InvariantCulture, "@F{0}@", familyId++), "FAM");
            ExportMarriage(family.ParentLeft, family.ParentRight, family.Relationship);
            foreach (Person child in family.Children)
            {
                WriteLine(1, "CHIL", string.Format(CultureInfo.InvariantCulture, "@{0}@", idMap.Get(child.Id)));
            }
        }
        private void ExportMarriage(Person partnerLeft, Person partnerRight, SpouseRelationship relationship)
        {
            if (partnerLeft != null && partnerLeft.Gender == Gender.Male)
            {
                WriteLine(1, "HUSB", string.Format(CultureInfo.InvariantCulture, "@{0}@", idMap.Get(partnerLeft.Id)));
            }

            if (partnerLeft != null && partnerLeft.Gender == Gender.Female)
            {
                WriteLine(1, "WIFE", string.Format(CultureInfo.InvariantCulture, "@{0}@", idMap.Get(partnerLeft.Id)));
            }
            if (partnerRight != null && partnerRight.Gender == Gender.Male)
            {
                WriteLine(1, "HUSB", string.Format(CultureInfo.InvariantCulture, "@{0}@", idMap.Get(partnerRight.Id)));
            }

            if (partnerRight != null && partnerRight.Gender == Gender.Female)
            {
                WriteLine(1, "WIFE", string.Format(CultureInfo.InvariantCulture, "@{0}@", idMap.Get(partnerRight.Id)));
            }

            if (relationship == null)
            {
                return;
            }
            if (relationship.SpouseModifier == SpouseModifier.Current)
            {
                WriteLine(1, "MARR", "Y");
                if (relationship.MarriageDate != null)
                {
                    WriteLine(2, "DATE", relationship.MarriageDate.Value.ToShortDateString());
                }
            }
            if (relationship.SpouseModifier == SpouseModifier.Former)
            {
                WriteLine(1, "DIV", "Y");
                if (relationship.DivorceDate != null)
                {
                    WriteLine(2, "DATE", relationship.DivorceDate.Value.ToShortDateString());
                }
            }
        }

        private void ExportName(Person person)
        {
            string firstMiddleNameSpace = (!string.IsNullOrEmpty(person.FirstName) && !string.IsNullOrEmpty(person.MiddleName)) ? " " : "";

            string value = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}/{3}/", person.FirstName, firstMiddleNameSpace, person.MiddleName, person.LastName);

            WriteLine(1, "NAME", value);
        }

        private void ExportPhotos(Person person)
        {
            foreach (Photo photo in person.Photos)
            {
                WriteLine(1, "OBJE", "");
                WriteLine(2, "FILE", photo.FullyQualifiedPath);
            }
        }

        private void ExportEvent(string tag, DateTime? date, string place)
        {
            if (date == null && string.IsNullOrEmpty(place))
            {
                return;
            }
            WriteLine(1, tag, "");
            if (date != null)
            {
                WriteLine(2, "DATE", date.Value.ToShortDateString());
            }
            if (!string.IsNullOrEmpty(place))
            {
                WriteLine(2, "PLAC", place);
            }
        }

        private void ExportGender(Person person)
        {
            WriteLine(1, "SEX", (person.Gender == Gender.Female) ? "F" : "M");
        }
        private void WriteLine(int level, string tag, string value)
        {
            const int ValueLimit = 200;
            if (value.Length < ValueLimit && !value.Contains("\r") && !value.Contains("\n"))
            {
                writer.WriteLine(string.Format(CultureInfo.CurrentCulture, "{0} {1} {2}", level, tag, value));
                return;
            }
            value = value.Replace("\r\n", "\n");
            value = value.Replace("\r", "\n");
            string[] lines = value.Split('\n');
            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                string line = lines[lineIndex];
                int chunkCount = (line.Length + ValueLimit - 1) / ValueLimit;

                for (int chunkIndex = 0; chunkIndex < chunkCount; chunkIndex++)
                {
                    int pos = chunkIndex * ValueLimit;
                    string chunk = line.Substring(pos, Math.Min(line.Length - pos, ValueLimit));
                    if (lineIndex == 0 && chunkIndex == 0)
                    {
                        writer.WriteLine(string.Format(CultureInfo.CurrentCulture, "{0} {1} {2}", level, tag, chunk));
                    }
                    else
                    {
                        writer.WriteLine(string.Format(CultureInfo.CurrentCulture, "{0} {1} {2}", level + 1, "CONC", chunk));
                    }
                }
                if (lineIndex < lines.Length - 1)
                {
                    writer.WriteLine(string.Format(CultureInfo.CurrentCulture, "{0} {1}", level + 1, "CONT"));
                }
            }
        }
    }
}