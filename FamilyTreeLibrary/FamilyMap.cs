using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FamilyTreeLibrary
{
    class FamilyMap : Dictionary<string, Family>
    {
        public void Create(PeopleCollection people)
        {
            Clear();
            foreach (Person person in people)
            {
                Collection<Person> parents = person.Parents;
                if (parents.Count > 0)
                {
                    Person parentLeft = parents[0];
                    Person parentRight = (parents.Count > 1) ? parents[1] : null;
                    string key = GetKey(parentLeft, parentRight);
                    if (!ContainsKey(key))
                    {
                        Family details = new Family(parentLeft, parentRight);
                        details.Relationship = parentLeft.GetSpouseRelationship(parentRight);
                        this[key] = details;
                    }
                    this[key].Children.Add(person);
                }
            }
            foreach (Person person in people)
            {
                Collection<Person> spouses = person.Spouses;
                foreach (Person spouse in spouses)
                {
                    string key = GetKey(person, spouse);
                    if (!ContainsKey(key))
                    {
                        Family details = new Family(person, spouse);
                        details.Relationship = person.GetSpouseRelationship(spouse);
                        this[key] = details;
                    }
                }
            }
        }
        private static string GetKey(Person partnerLeft, Person partnerRight)
        {

            string key = partnerLeft.Id;
            if (partnerRight != null)
            {
                if (partnerLeft.Id.CompareTo(partnerRight.Id) < 0)
                    key = partnerLeft.Id + partnerRight.Id;
                else
                    key = partnerRight.Id + partnerLeft.Id;
            }

            return key;
        }
    }
}