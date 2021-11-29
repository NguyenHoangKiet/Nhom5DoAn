using System;
using System.Collections.Generic;

namespace FamilyTreeLibrary
{
    public static class RelationshipHelper
    {
        public static void AddChild(PeopleCollection family, Person person, Person child)
        {
            foreach (Person existingSibling in person.Children)
            {
                family.AddSibling(existingSibling, child);
            }

            switch (person.Spouses.Count)
            {
                case 0:
                    family.AddChild(person, child, ParentChildModifier.Natural);
                    break;
                case 1:
                    family.AddChild(person, child, ParentChildModifier.Natural);
                    family.AddChild(person.Spouses[0], child, ParentChildModifier.Natural);
                    break;
            }
        }
        public static void AddParent(PeopleCollection family, Person person, Person parent)
        {
            if (person.Parents.Count == 2)
            {
                return;
            }
            family.Add(parent);

            switch (person.Parents.Count)
            {
                case 0:
                    family.AddChild(parent, person, ParentChildModifier.Natural);
                    break;
                case 1:
                    family.AddChild(parent, person, ParentChildModifier.Natural);
                    family.AddSpouse(parent, person.Parents[0], SpouseModifier.Current);
                    break;
            }
            if (person.Siblings.Count > 0)
            {
                foreach (Person sibling in person.Siblings)
                {
                    family.AddChild(parent, sibling, ParentChildModifier.Natural);
                }
            }
            person.HasParents = true;
        }
        public static void AddParent(PeopleCollection family, Person person, ParentSet parentSet)
        {
            family.AddChild(parentSet.FirstParent, person, ParentChildModifier.Natural);
            family.AddChild(parentSet.SecondParent, person, ParentChildModifier.Natural);
            List<Person> siblings = GetChildren(parentSet);
            foreach (Person sibling in siblings)
            {
                if (sibling != person)
                {
                    family.AddSibling(person, sibling);
                }
            }
        }
        private static List<Person> GetChildren(ParentSet parentSet)
        {
            List<Person> firstParentChildren = new List<Person>(parentSet.FirstParent.Children);
            List<Person> secondParentChildren = new List<Person>(parentSet.SecondParent.Children);
            List<Person> children = new List<Person>();
            foreach (Person child in firstParentChildren)
            {
                if (secondParentChildren.Contains(child))
                {
                    children.Add(child);
                }
            }

            return children;
        }
        public static void AddSpouse(PeopleCollection family, Person person, Person spouse, SpouseModifier modifier)
        {
            if (person.Gender == Gender.Male)
            {
                spouse.Gender = Gender.Female;
            }
            else
            {
                spouse.Gender = Gender.Male;
            }

            if (person.Spouses != null)
            {
                switch (person.Spouses.Count)
                {
                    case 0:
                        family.AddSpouse(person, spouse, modifier);
                        if (person.Children != null || person.Children.Count > 0)
                        {
                            foreach (Person child in person.Children)
                            {
                                family.AddChild(spouse, child, ParentChildModifier.Natural);
                            }
                        }
                        break;
                    default:
                        if (modifier == SpouseModifier.Current)
                        {
                            foreach (Relationship relationship in person.Relationships)
                            {
                                if (relationship.RelationshipType == RelationshipType.Spouse)
                                {
                                    ((SpouseRelationship)relationship).SpouseModifier = SpouseModifier.Former;
                                }
                            }
                        }

                        family.AddSpouse(person, spouse, modifier);
                        break;
                }
                person.HasSpouse = true;
            }
        }
        public static void AddSibling(PeopleCollection family, Person person, Person sibling)
        {
            if (person.Siblings.Count > 0)
            {
                foreach (Person existingSibling in person.Siblings)
                {
                    family.AddSibling(existingSibling, sibling);
                }
            }

            if (person.Parents != null)
            {
                switch (person.Parents.Count)
                {
                    case 0:
                        family.AddSibling(person, sibling);
                        break;
                    case 1:
                        family.AddSibling(person, sibling);
                        family.AddChild(person.Parents[0], sibling, ParentChildModifier.Natural);
                        break;
                    case 2:
                        foreach (Person parent in person.Parents)
                        {
                            family.AddChild(parent, sibling, ParentChildModifier.Natural);
                        }

                        family.AddSibling(person, sibling);
                        break;

                    default:
                        family.AddSibling(person, sibling);
                        break;
                }
            }
        }
        public static void UpdateSpouseStatus(Person person, Person spouse, SpouseModifier modifier)
        {
            foreach (Relationship relationship in person.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Spouse && relationship.RelationTo.Equals(spouse))
                {
                    ((SpouseRelationship)relationship).SpouseModifier = modifier;
                    break;
                }
            }

            foreach (Relationship relationship in spouse.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Spouse && relationship.RelationTo.Equals(person))
                {
                    ((SpouseRelationship)relationship).SpouseModifier = modifier;
                    break;
                }
            }
        }
        public static void UpdateMarriageDate(Person person, Person spouse, DateTime? dateTime)
        {
            foreach (Relationship relationship in person.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Spouse && relationship.RelationTo.Equals(spouse))
                {
                    ((SpouseRelationship)relationship).MarriageDate = dateTime;
                    break;
                }
            }

            foreach (Relationship relationship in spouse.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Spouse && relationship.RelationTo.Equals(person))
                {
                    ((SpouseRelationship)relationship).MarriageDate = dateTime;
                    break;
                }
            }
        }
        public static void UpdateDivorceDate(Person person, Person spouse, DateTime? dateTime)
        {
            foreach (Relationship relationship in person.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Spouse && relationship.RelationTo.Equals(spouse))
                {
                    ((SpouseRelationship)relationship).DivorceDate = dateTime;
                    break;
                }
            }

            foreach (Relationship relationship in spouse.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Spouse && relationship.RelationTo.Equals(person))
                {
                    ((SpouseRelationship)relationship).DivorceDate = dateTime;
                    break;
                }
            }
        }
        public static void ChangeParents(PeopleCollection family, Person person, ParentSet newParentSet)
        {
            if (person.ParentSet == null || newParentSet == null || person.ParentSet.Equals(newParentSet))
                return;
            ParentSet formerParentSet = person.ParentSet;
            RemoveParentChildRelationship(person, formerParentSet.FirstParent);
            RemoveParentChildRelationship(person, formerParentSet.SecondParent);
            RemoveSiblingRelationships(person);
            AddParent(family, person, newParentSet);
        }
        private static void RemoveSiblingRelationships(Person person)
        {
            for (int i = person.Relationships.Count - 1; i >= 0; i--)
            {
                if (person.Relationships[i].RelationshipType == RelationshipType.Sibling)
                {
                    person.Relationships.RemoveAt(i);
                }
            }
        }
        private static void RemoveParentChildRelationship(Person person, Person parent)
        {
            foreach (Relationship relationship in person.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Parent && relationship.RelationTo.Equals(parent))
                {
                    person.Relationships.Remove(relationship);
                    break;
                }
            }

            foreach (Relationship relationship in parent.Relationships)
            {
                if (relationship.RelationshipType == RelationshipType.Child && relationship.RelationTo.Equals(person))
                {
                    parent.Relationships.Remove(relationship);
                    break;
                }
            }
        }
        public static void DeletePerson(PeopleCollection family, Person personToDelete)
        {
            if (!personToDelete.IsDeletable)
            {
                return;
            }
            foreach (Relationship relationship in personToDelete.Relationships)
            {
                foreach (Relationship rel in relationship.RelationTo.Relationships)
                {
                    if (rel.RelationTo.Equals(personToDelete))
                    {
                        relationship.RelationTo.Relationships.Remove(rel);
                        break;
                    }
                }
            }
            personToDelete.DeletePhotos();
            personToDelete.DeleteStory();

            family.Remove(personToDelete);
        }
    }
}
