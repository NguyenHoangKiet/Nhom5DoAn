using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace FamilyTreeLibrary
{
    public class GedcomImport
    {
        #region fields

        private PeopleCollection people;

        private XmlDocument doc;

        #endregion

        public void Import(PeopleCollection peopleCollection, string gedcomFilePath)
        {
            peopleCollection.Clear();

            string xmlFilePath = Path.GetTempFileName();

            try
            {
                people = peopleCollection;

                GedcomConvert.ConvertToXml(gedcomFilePath, xmlFilePath, true);
                doc = new XmlDocument();
                doc.Load(xmlFilePath);

                ImportPeople();
                ImportFamilies();

                if (peopleCollection.Count > 0)
                {
                    peopleCollection.Current = peopleCollection[0];
                }
            }
            finally
            {
                File.Delete(xmlFilePath);
            }
        }

        private void ImportPeople()
        {
            XmlNodeList list = doc.SelectNodes("/root/INDI");

            foreach (XmlNode node in list)
            {
                Person person = new Person();

                person.FirstName = GetFirstName(node);
                person.LastName = GetLastName(node);
                person.NickName = GetNickName(node);
                person.Suffix = GetSuffix(node);
                person.MarriedName = GetMarriedName(node);

                person.Id = GetId(node);
                person.Gender = GetGender(node);

                ImportBirth(person, node);
                ImportDeath(person, node);
                ImportResidence(person, node);
                ImportEventBaptism(person, node);
                ImportPhotos(person, node);
                ImportNote(person, node);

                people.Add(person);
            }
        }

        private void ImportFamilies()
        {
            XmlNodeList list = doc.SelectNodes("/root/FAM");
            foreach (XmlNode node in list)
            {
                string husband = GetHusbandID(node);
                string wife = GetWifeID(node);
                string[] children = GetChildrenIDs(node);

                Person husbandPerson = people.Find(husband);
                Person wifePerson = people.Find(wife);

                ImportMarriage(husbandPerson, wifePerson, node);

                foreach (string child in children)
                {
                    Person childPerson = people.Find(child);

                    if (husbandPerson != null && childPerson != null)
                    {
                        RelationshipHelper.AddChild(people, husbandPerson, childPerson);
                    }

                    if (husbandPerson == null && wifePerson != null & childPerson != null)
                    {
                        RelationshipHelper.AddChild(people, wifePerson, childPerson);
                    }
                }
            }
        }

        private static void ImportMarriage(Person husband, Person wife, XmlNode node)
        {
            if (husband == null || wife == null)
            {
                return;
            }

            if (node.SelectSingleNode("MARR") != null || node.SelectSingleNode("DIV") != null)
            {
                DateTime? marriageDate = GetValueDate(node, "MARR/DATE");
                DateTime? divorceDate = GetValueDate(node, "DIV/DATE");
                SpouseModifier modifier = GetDivorced(node) ? SpouseModifier.Former : SpouseModifier.Current;
                string Marriageplace = GetValue(node, "MARR/PLAC");

                if (husband.GetSpouseRelationship(wife) == null)
                {
                    SpouseRelationship husbandMarriage = new SpouseRelationship(wife, modifier);
                    husbandMarriage.MarriageDate = marriageDate;
                    husbandMarriage.DivorceDate = divorceDate;
                    husbandMarriage.MarriagePlace = Marriageplace;
                    husband.Relationships.Add(husbandMarriage);
                }

                if (wife.GetSpouseRelationship(husband) == null)
                {
                    SpouseRelationship wifeMarriage = new SpouseRelationship(husband, modifier);
                    wifeMarriage.MarriageDate = marriageDate;
                    wifeMarriage.DivorceDate = divorceDate;
                    wifeMarriage.MarriagePlace = Marriageplace;
                    wife.Relationships.Add(wifeMarriage);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static void ImportResidence(Person person, XmlNode node)
        {
            try
            {

                XmlNodeList list = node.SelectNodes("RESI");
                if (list == null || list.Count == 0)
                    return;

                Contact contact = new Contact();

                for (int i = 0; i < list.Count; i++)
                    FillContact(contact, list[i]);

                person.Contact = contact;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static void FillContact(Contact contact, XmlNode node)
        {
            string valeur;

            //Get mail info
            valeur = GetValue(node, "EMAIL");
            if (!string.IsNullOrEmpty(valeur))
            {
                contact.Mail = valeur.Replace("@@", "@");
                return;
            }

            // Get Address info
            XmlNode addrNode = node.SelectSingleNode("ADDR");
            if (addrNode != null)
            {
                if (contact.Address == null)
                    contact.Address = new Address();

                contact.Address.Address1 = GetValue(addrNode, "ADR1");
                contact.Address.Address2 = GetValue(addrNode, "ADR2");
                contact.Address.City = GetValue(addrNode, "CITY");
                contact.Address.ZipCode = GetValue(addrNode, "POST");
                contact.Address.Country = GetValue(addrNode, "CTRY");
            }

            // Get phone number
            valeur = GetValue(node, "PHON");
            if (!String.IsNullOrEmpty(valeur))
            {
                contact.Phone = valeur;
            }
        }

        private void ImportEventBaptism(Person person, XmlNode node)
        {
            try
            {
                XmlNodeList list = node.SelectNodes("BAPM");
                if (list == null || list.Count == 0)
                    return;

                EventBaptism bapt = new EventBaptism();
                XmlNode baptNode = list[0];

                bapt.BaptismDate = GetValueDate(baptNode, "DATE");
                bapt.BaptismPlace = GetValue(baptNode, "PLAC");

                person.Baptism = bapt;
            }
            catch (Exception)
            {

                throw;
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static void ImportPhotos(Person person, XmlNode node)
        {
            try
            {
                string[] photos = GetPhotos(node);
                if (photos == null || photos.Length == 0)
                {
                    return;
                }

                for (int i = 0; i < photos.Length; i++)
                {
                    Photo photo = new Photo(photos[i]);
                    photo.IsAvatar = (i == 0) ? true : false;
                    person.Photos.Add(photo);
                }
            }
            catch
            {
            }
        }


        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static void ImportNote(Person person, XmlNode node)
        {
            try
            {
                string value = GetValue(node, "NOTE");
                if (!string.IsNullOrEmpty(value))
                {
                    person.Story = new Story();
                    string storyFileName = new StringBuilder(person.Name).Append(".rtf").ToString();
                    person.Story.Save(value, storyFileName);
                }
            }
            catch
            {

            }
        }


        private static void ImportBirth(Person person, XmlNode node)
        {
            person.BirthDate = GetValueDate(node, "BIRT/DATE");
            person.BirthPlace = GetValue(node, "BIRT/PLAC");
        }


        private static void ImportDeath(Person person, XmlNode node)
        {
            person.IsLiving = (node.SelectSingleNode("DEAT") == null) ? true : false;
            person.DeathDate = GetValueDate(node, "DEAT/DATE");
            person.DeathPlace = GetValue(node, "DEAT/PLAC");
        }


        private static string[] GetPhotos(XmlNode node)
        {
            string[] photos;
            XmlNodeList list = node.SelectNodes("OBJE");
            photos = new string[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                photos[i] = GetFile(list[i]);
            }

            return photos;
        }

        private static string GetSuffix(XmlNode node)
        {
            return GetValue(node, "NAME/NPFX");
        }

        private static string GetMarriedName(XmlNode node)
        {
            return GetValue(node, "NAME/_MARNM");
        }

        private static string GetNickName(XmlNode node)
        {
            return GetValue(node, "NAME/NICK");
        }

        private static string GetHusbandID(XmlNode node)
        {
            return GetValueId(node, "HUSB");
        }

        private static string GetWifeID(XmlNode node)
        {
            return GetValueId(node, "WIFE");
        }

        private static Gender GetGender(XmlNode node)
        {
            string value = GetValue(node, "SEX");
            if (string.Compare(value, "f", true, CultureInfo.InvariantCulture) == 0)
            {
                return Gender.Female;
            }

            return Gender.Male;
        }

        private static bool GetDivorced(XmlNode node)
        {
            string value = GetValue(node, "DIV");
            if (string.Compare(value, "n", true, CultureInfo.InvariantCulture) == 0)
            {
                return false;
            }

            // Divorced if the tag exists.
            return node.SelectSingleNode("DIV") != null ? true : false;
        }

        private static string GetFile(XmlNode node)
        {
            return GetValue(node, "FILE");
        }

        private static string[] GetChildrenIDs(XmlNode node)
        {
            string[] children;
            XmlNodeList list = node.SelectNodes("CHIL");
            children = new string[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                children[i] = GetId(list[i]);
            }

            return children;
        }

        private static string GetFirstName(XmlNode node)
        {
            string name = GetValue(node, "NAME");
            string[] parts = name.Split('/');
            if (parts.Length > 0)
            {
                return parts[0].Trim();
            }

            return string.Empty;
        }

        private static string GetLastName(XmlNode node)
        {
            string name = GetValue(node, "NAME");
            string[] parts = name.Split('/');
            if (parts.Length > 1)
            {
                return parts[1].Trim();
            }

            return string.Empty;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static DateTime? GetValueDate(XmlNode node, string xpath)
        {
            DateTime? result = null;

            try
            {
                string value = GetValue(node, xpath);
                if (!string.IsNullOrEmpty(value))
                {
                    result = DateTime.Parse(value, CultureInfo.InvariantCulture);
                }
            }
            catch
            {
            }

            return result;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static string GetId(XmlNode node)
        {
            try
            {
                return node.Attributes["Value"].Value.Replace('@', ' ').Trim();
            }
            catch
            {
                return string.Empty;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static string GetValueId(XmlNode node, string xpath)
        {
            string result = string.Empty;
            try
            {
                XmlNode valueNode = node.SelectSingleNode(xpath);
                if (valueNode != null)
                {
                    result = valueNode.Attributes["Value"].Value.Replace('@', ' ').Trim();
                }
            }
            catch
            {
            }
            return result;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static string GetValue(XmlNode node, string xpath)
        {
            string result = string.Empty;
            try
            {
                XmlNode valueNode = node.SelectSingleNode(xpath);
                if (valueNode != null)
                {
                    result = valueNode.Attributes["Value"].Value.Trim();
                }
            }
            catch
            {
            }
            return result;
        }
    }
}