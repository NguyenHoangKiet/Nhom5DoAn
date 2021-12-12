using FamilyTree;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using FamilyTreeLibrary;

namespace FamilyTree.Components.Tree
{
    class Logic
    {
        #region fields

        private List<Connector> connections = new List<Connector>();

        private Dictionary<Person, ConnectorNode> personLookup =
            new Dictionary<Person, ConnectorNode>();

        private PeopleCollection family;

        private RoutedEventHandler nodeClickHandler;

        private double displayYear;

        #endregion

        #region properties

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public RoutedEventHandler NodeClickHandler
        {
            set { nodeClickHandler = value; }
        }

        public PeopleCollection Family
        {
            get { return family; }
        }

        public List<Connector> Connections
        {
            get { return connections; }
        }

        public Dictionary<Person, ConnectorNode> PersonLookup
        {
            get { return personLookup; }
        }

        public double DisplayYear
        {
            set
            {
                if (displayYear != value)
                {
                    displayYear = value;
                    foreach (ConnectorNode connectorNode in personLookup.Values)
                        connectorNode.Node.DisplayYear = displayYear;
                }
            }
        }

        public double MinimumYear
        {
            get
            {
                double minimumYear = DateTime.Now.Year;

                foreach (ConnectorNode connectorNode in personLookup.Values)
                {
                    DateTime? date = connectorNode.Node.Person.BirthDate;
                    if (date != null)
                        minimumYear = Math.Min(minimumYear, date.Value.Year);
                }

                foreach (Connector connector in connections)
                {
                    DateTime? date = connector.MarriedDate;
                    if (date != null)
                        minimumYear = Math.Min(minimumYear, date.Value.Year);

                    date = connector.PreviousMarriedDate;
                    if (date != null)
                        minimumYear = Math.Min(minimumYear, date.Value.Year);
                }

                return minimumYear;
            }
        }

        #endregion

        public Logic()
        {
            family = App.Family;

            Clear();
        }

        #region get people

        public static Collection<Person> GetParents(Row row)
        {
            Collection<Person> list = new Collection<Person>();

            List<Person> rowList = GetPrimaryAndRelatedPeople(row);

            foreach (Person person in rowList)
            {
                foreach (Person parent in person.Parents)
                {
                    if (!list.Contains(parent))
                        list.Add(parent);
                }
            }

            return list;
        }

        public static List<Person> GetChildren(Row row)
        {
            List<Person> list = new List<Person>();

            List<Person> rowList = GetPrimaryAndRelatedPeople(row);

            foreach (Person person in rowList)
            {
                foreach (Person child in person.Children)
                {
                    if (!list.Contains(child))
                        list.Add(child);
                }
            }

            return list;
        }
        private static List<Person> GetPrimaryAndRelatedPeople(Row row)
        {
            List<Person> list = new List<Person>();
            foreach (Group group in row.Groups)
            {
                foreach (Node node in group.Nodes)
                {
                    if (node.Type == NodeType.Related || node.Type == NodeType.Primary)
                        list.Add(node.Person);
                }
            }

            return list;
        }

        private static void RemoveDuplicates(Collection<Person> people, Collection<Person> other)
        {
            foreach (Person person in other)
                people.Remove(person);
        }

        #endregion

        #region create nodes

        private Node CreateNode(Person person, NodeType type, bool clickEvent, double scale)
        {
            Node node = CreateNode(person, type, clickEvent);
            node.Scale = scale;
            return node;
        }
        private Node CreateNode(Person person, NodeType type, bool clickEvent)
        {
            Node node = new Node();
            node.Person = person;
            node.Type = type;
            if (clickEvent)
                node.Click += nodeClickHandler;

            return node;
        }

        private void AddSiblingNodes(Row row, Group group,
            Collection<Person> siblings, NodeType nodeType, double scale)
        {
            foreach (Person sibling in siblings)
            {
                if (!personLookup.ContainsKey(sibling))
                {
                    Node node = CreateNode(sibling, nodeType, true, scale);
                    group.Add(node);
                    personLookup.Add(node.Person, new ConnectorNode(node, group, row));
                }
            }
        }

        private void AddSpouseNodes(Person person, Row row,
            Group group, Collection<Person> spouses,
            NodeType nodeType, double scale, bool married)
        {
            foreach (Person spouse in spouses)
            {
                if (!personLookup.ContainsKey(spouse))
                {
                    Node node = CreateNode(spouse, nodeType, true, scale);
                    group.Add(node);

                    ConnectorNode connectorNode = new ConnectorNode(node, group, row);
                    personLookup.Add(node.Person, connectorNode);
                    connections.Add(new MarriedConnector(married, personLookup[person], connectorNode));
                }
            }
        }

        #endregion

        #region create rows

        public Row CreatePrimaryRow(Person person, double scale, double scaleRelated)
        {
            Group primaryGroup = new Group();
            Group leftGroup = new Group();

            Row row = new Row();

            Node node = CreateNode(person, NodeType.Primary, false, scale);
            primaryGroup.Add(node);
            personLookup.Add(node.Person, new ConnectorNode(node, primaryGroup, row));

            Collection<Person> currentSpouses = person.CurrentSpouses;
            AddSpouseNodes(person, row, leftGroup, currentSpouses,
                NodeType.Spouse, scaleRelated, true);

            Collection<Person> previousSpouses = person.PreviousSpouses;
            AddSpouseNodes(person, row, leftGroup, previousSpouses,
                NodeType.Spouse, scaleRelated, false);

            Collection<Person> siblings = person.Siblings;
            AddSiblingNodes(row, leftGroup, siblings, NodeType.Sibling, scaleRelated);

            Collection<Person> halfSiblings = person.HalfSiblings;
            AddSiblingNodes(row, leftGroup, halfSiblings, NodeType.SiblingLeft, scaleRelated);

            if (leftGroup.Nodes.Count > 0)
            {
                leftGroup.Reverse();
                row.Add(leftGroup);
            }

            row.Add(primaryGroup);

            return row;
        }

        public Row CreateChildrenRow(List<Person> children, double scale, double scaleRelated)
        {
            Row row = new Row();

            foreach (Person child in children)
            {
                Group group = new Group();
                row.Add(group);

                if (!personLookup.ContainsKey(child))
                {
                    Node node = CreateNode(child, NodeType.Related, true, scale);
                    group.Add(node);
                    personLookup.Add(node.Person, new ConnectorNode(node, group, row));
                }

                Collection<Person> currentSpouses = child.CurrentSpouses;
                AddSpouseNodes(child, row, group, currentSpouses,
                    NodeType.Spouse, scaleRelated, true);

                Collection<Person> previousSpouses = child.PreviousSpouses;
                AddSpouseNodes(child, row, group, previousSpouses,
                    NodeType.Spouse, scaleRelated, false);

                AddParentConnections(child);

                group.Reverse();
            }

            return row;
        }

        public Row CreateParentRow(Collection<Person> parents, double scale, double scaleRelated)
        {
            Row row = new Row();

            int groupCount = 0;

            foreach (Person person in parents)
            {
                Group group = new Group();
                row.Add(group);

                bool left = (groupCount++ % 2 == 0) ? true : false;

                if (!personLookup.ContainsKey(person))
                {
                    Node node = CreateNode(person, NodeType.Related, true, scale);
                    group.Add(node);
                    personLookup.Add(node.Person, new ConnectorNode(node, group, row));
                }

                Collection<Person> currentSpouses = person.CurrentSpouses;
                RemoveDuplicates(currentSpouses, parents);
                AddSpouseNodes(person, row, group, currentSpouses,
                    NodeType.Spouse, scaleRelated, true);

                Collection<Person> previousSpouses = person.PreviousSpouses;
                RemoveDuplicates(previousSpouses, parents);
                AddSpouseNodes(person, row, group, previousSpouses,
                    NodeType.Spouse, scaleRelated, false);

                Collection<Person> siblings = person.Siblings;
                AddSiblingNodes(row, group, siblings, NodeType.Sibling, scaleRelated);

                Collection<Person> halfSiblings = person.HalfSiblings;
                AddSiblingNodes(row, group, halfSiblings, left ?
                    NodeType.SiblingLeft : NodeType.SiblingRight, scaleRelated);

                AddChildConnections(person);
                AddChildConnections(currentSpouses);
                AddChildConnections(previousSpouses);

                if (left)
                    group.Reverse();
            }

            AddSpouseConnections(parents);

            return row;
        }

        #endregion

        #region connections

        private void AddChildConnections(Collection<Person> parents)
        {
            foreach (Person person in parents)
                AddChildConnections(person);
        }

        private void AddParentConnections(Person child)
        {
            foreach (Person parent in child.Parents)
            {
                if (personLookup.ContainsKey(parent) &&
                    personLookup.ContainsKey(child))
                {
                    connections.Add(new ChildConnector(
                        personLookup[parent], personLookup[child]));
                }
            }
        }

        private void AddChildConnections(Person parent)
        {
            foreach (Person child in parent.Children)
            {
                if (personLookup.ContainsKey(parent) &&
                    personLookup.ContainsKey(child))
                {
                    connections.Add(new ChildConnector(
                        personLookup[parent], personLookup[child]));
                }
            }
        }

        private void AddSpouseConnections(Collection<Person> list)
        {
            for (int current = 0; current < list.Count; current++)
            {
                Person person = list[current];

                for (int i = current + 1; i < list.Count; i++)
                {
                    Person spouse = list[i];
                    SpouseRelationship rel = person.GetSpouseRelationship(spouse);

                    if (rel != null && rel.SpouseModifier == SpouseModifier.Current)
                    {
                        if (personLookup.ContainsKey(person) &&
                            personLookup.ContainsKey(spouse))
                        {
                            connections.Add(new MarriedConnector(true,
                                personLookup[person], personLookup[spouse]));
                        }
                    }

                    if (rel != null && rel.SpouseModifier == SpouseModifier.Former)
                    {
                        if (personLookup.ContainsKey(person) &&
                            personLookup.ContainsKey(spouse))
                        {
                            connections.Add(new MarriedConnector(false,
                                personLookup[person], personLookup[spouse]));
                        }
                    }
                }
            }
        }

        #endregion
        public void Clear()
        {
            foreach (ConnectorNode node in personLookup.Values)
                node.Node.Click -= nodeClickHandler;

            connections.Clear();
            personLookup.Clear();

            displayYear = DateTime.Now.Year;
        }

        public Node GetNode(Person person)
        {
            if (person == null)
                return null;

            if (!personLookup.ContainsKey(person))
                return null;

            return personLookup[person].Node;
        }

        public Rect GetNodeBounds(Person person)
        {
            Rect bounds = Rect.Empty;
            if (person != null && personLookup.ContainsKey(person))
            {
                ConnectorNode connector = personLookup[person];
                bounds = new Rect(connector.TopLeft.X, connector.TopLeft.Y,
                    connector.Node.ActualWidth, connector.Node.ActualHeight);
            }

            return bounds;
        }
    }
}
