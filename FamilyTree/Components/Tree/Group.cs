using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace FamilyTree.Components.Tree
{
    public class Group : FrameworkElement
    {
        #region fields

        private const double NodeSpace = 10;

        private Point location = new Point();

        private List<Node> nodes = new List<Node>();

        #endregion

        #region properties

        public Point Location
        {
            get { return location; }
            set { location = value; }
        }

        public ReadOnlyCollection<Node> Nodes
        {
            get { return new ReadOnlyCollection<Node>(nodes); }
        }

        #endregion

        #region overrides

        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (Node node in nodes)
                node.Measure(size);

            return ArrangeNodes(false);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return ArrangeNodes(true);
        }

        protected override int VisualChildrenCount
        {
            get { return nodes.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            return nodes[index];
        }

        #endregion

        public void Add(Node node)
        {
            nodes.Add(node);
            AddVisualChild(node);
        }

        public void Clear()
        {
            foreach (Node node in nodes)
                RemoveVisualChild(node);

            nodes.Clear();
        }

        public void Reverse()
        {
            nodes.Reverse();
        }

        private Size ArrangeNodes(bool arrange)
        {
            double pos = 0;

            Rect bounds = new Rect();

            Size totalSize = new Size(0, 0);

            foreach (Node node in nodes)
            {
                bounds.X = pos;
                bounds.Y = 0;

                bounds.Width = node.DesiredSize.Width;
                bounds.Height = node.DesiredSize.Height;

                if (arrange)
                {
                    node.Arrange(bounds);
                    node.Location = bounds.TopLeft;
                }

                totalSize.Width = pos + node.DesiredSize.Width;
                totalSize.Height = Math.Max(totalSize.Height, node.DesiredSize.Height);

                pos += (bounds.Width + NodeSpace);
            }

            return totalSize;
        }
    }
}
