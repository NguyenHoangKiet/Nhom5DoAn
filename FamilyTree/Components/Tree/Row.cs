using FamilyTree.Components.Tree;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace FamilyTree.Components.Tree
{
    public class Row : FrameworkElement
    {
        #region fields

        private double groupSpace = 80;

        private Point location = new Point();

        private List<Group> groups = new List<Group>();

        #endregion

        #region properties

        public double GroupSpace
        {
            get { return groupSpace; }
            set { groupSpace = value; }
        }
        public Point Location
        {
            get { return location; }
            set { location = value; }
        }

        public ReadOnlyCollection<Group> Groups
        {
            get { return new ReadOnlyCollection<Group>(groups); }
        }

        public int NodeCount
        {
            get
            {
                int count = 0;
                foreach (Group group in groups)
                    count += group.Nodes.Count;
                return count;
            }
        }

        #endregion

        #region overrides

        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (Group group in groups)
            {
                group.Measure(size);
            }

            return ArrangeGroups(false);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return ArrangeGroups(true);
        }

        protected override int VisualChildrenCount
        {
            get { return groups.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            return groups[index];
        }

        #endregion

        public void Add(Group group)
        {
            groups.Add(group);
            AddVisualChild(group);
        }

        public void Clear()
        {
            foreach (Group group in groups)
            {
                group.Clear();
                RemoveVisualChild(group);
            }

            groups.Clear();
        }

        private Size ArrangeGroups(bool arrange)
        {
            double pos = 0;

            Rect bounds = new Rect();

            Size totalSize = new Size(0, 0);

            foreach (Group group in groups)
            {
                bounds.X = pos;
                bounds.Y = 0;

                bounds.Width = group.DesiredSize.Width;
                bounds.Height = group.DesiredSize.Height;

                if (arrange)
                {
                    group.Arrange(bounds);
                    group.Location = bounds.TopLeft;
                }

                totalSize.Width = pos + group.DesiredSize.Width;
                totalSize.Height = Math.Max(totalSize.Height, group.DesiredSize.Height);

                pos += (bounds.Width + groupSpace);
            }

            return totalSize;
        }
    }
}
