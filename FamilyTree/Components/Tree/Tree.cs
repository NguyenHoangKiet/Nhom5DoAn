using FamilyTreeLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace FamilyTree.Components.Tree
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    public class Tree : FrameworkElement
    {
        #region fields

        private static class Const
        {
            public static double AnimationPauseDuration = 600;

            public static double NodeFadeInDuration = 500;

            public static double NewPersonAnimationDuration = 250;

            public static int MaximumNodes = 50;

            public static double PrimaryRowGroupSpace = 20;
            public static double ChildRowGroupSpace = 20;
            public static double ParentRowGroupSpace = 40;

            public static double RowSpace = 40;

            public static double RelatedMultiplier = 0.8;

            public static double GenerationMultiplier = 0.9;
        }

        private List<Row> rows = new List<Row>();

        private Logic logic;

        private Size totalSize = new Size(0, 0);

        private double scale = 1.0;

        private Rect selectedNodeBounds = Rect.Empty;

        private bool populating;

        private Person newPerson;

        private DispatcherTimer animationTimer = new DispatcherTimer();

#if DEBUG
        bool displayBorder;
#endif

        #endregion

        #region events

        public event EventHandler TreeUpdated;
        private void OnTreeUpdated()
        {
            if (TreeUpdated != null)
            {
                TreeUpdated(this, EventArgs.Empty);
            }
        }

        public event EventHandler TreePopulated;
        private void OnTreePopulated()
        {
            if (TreePopulated != null)
            {
                TreePopulated(this, EventArgs.Empty);
            }
        }

        #endregion

        #region properties

        public double Scale
        {
            get { return scale; }
            set
            {
                if (scale != value)
                {
                    scale = value;
                    LayoutTransform = new ScaleTransform(scale, scale);
                }
            }
        }

        public double DisplayYear
        {
            set
            {
                logic.DisplayYear = value;
                InvalidateVisual();
            }
        }

        public double MinimumYear
        {
            get { return logic.MinimumYear; }
        }

        public Rect PrimaryNodeBounds
        {
            get { return logic.GetNodeBounds(logic.Family.Current); }
        }

        public Rect SelectedNodeBounds
        {
            get { return selectedNodeBounds; }
        }

        public int NodeCount
        {
            get { return logic.PersonLookup.Count; }
        }

        #endregion

        public Tree()
        {
            logic = new Logic();
            logic.NodeClickHandler = new RoutedEventHandler(OnNodeClick);

            if (logic.Family != null)
            {
                logic.Family.ContentChanged += new EventHandler<ContentChangedEventArgs>(OnFamilyContentChanged);
                logic.Family.CurrentChanged += new EventHandler(OnFamilyCurrentChanged);
            }
        }

        #region layout

        protected override void OnInitialized(EventArgs e)
        {
#if DEBUG
            ContextMenu = new ContextMenu();
            MenuItem item = new MenuItem();
            ContextMenu.Items.Add(item);
            item.Header = "Show Tree Outline";
            item.Click += new RoutedEventHandler(OnToggleBorderClick);
            item.Foreground = SystemColors.MenuTextBrush;
            item.Background = SystemColors.MenuBrush;
#endif

            UpdateTree();
            base.OnInitialized(e);
        }

        protected override int VisualChildrenCount
        {
            get { return rows.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            return rows[index];
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (Row row in rows)
            {
                row.Measure(size);
            }

            return ArrangeRows(false);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return ArrangeRows(true);
        }

        private Size ArrangeRows(bool arrange)
        {
            double pos = 0;

            Rect bounds = new Rect();

            Size size = new Size(0, 0);

            foreach (Row row in rows)
            {
                bounds.Y = pos;
                bounds.X = (totalSize.Width == 0) ? 0 :
                    bounds.X = (totalSize.Width - row.DesiredSize.Width) / 2;

                bounds.Width = row.DesiredSize.Width;
                bounds.Height = row.DesiredSize.Height;

                if (arrange)
                {
                    row.Arrange(bounds);
                    row.Location = bounds.TopLeft;
                }

                size.Width = Math.Max(size.Width, bounds.Width);
                size.Height = pos + row.DesiredSize.Height;

                pos += bounds.Height;
            }

            totalSize = size;
            return size;
        }

        #endregion

        protected override void OnRender(DrawingContext drawingContext)
        {
#if DEBUG
            if (displayBorder)
            {
                foreach (Row row in rows)
                {
                    Rect bounds = new Rect(row.Location, row.DesiredSize);
                    drawingContext.DrawRectangle(null, new Pen(Brushes.DarkKhaki, 1), bounds);

                    foreach (Group group in row.Groups)
                    {
                        bounds = new Rect(group.Location, group.DesiredSize);
                        bounds.Offset(row.Location.X, row.Location.Y);
                        bounds.Inflate(-1, -1);
                        drawingContext.DrawRectangle(null, new Pen(Brushes.Gray, 1), bounds);
                    }
                }
            }
#endif

            foreach (Connector connector in logic.Connections)
            {
                if (connector.IsChildConnector)
                {
                    connector.Draw(drawingContext);
                }
            }

            foreach (Connector connector in logic.Connections)
            {
                if (!connector.IsChildConnector)
                {
                    connector.Draw(drawingContext);
                }
            }
        }


#if DEBUG
        void OnToggleBorderClick(object sender, RoutedEventArgs e)
        {
            displayBorder = !displayBorder;

            MenuItem menuItem = ContextMenu.Items[0] as MenuItem;
            menuItem.IsChecked = displayBorder;

            InvalidateVisual();
        }
#endif

        #region tree updates

        private void Clear()
        {
            foreach (Row row in rows)
            {
                row.Clear();
                RemoveVisualChild(row);
            }

            rows.Clear();
            logic.Clear();
        }

        private void Populate()
        {
            populating = true;

            UpdateTree();

            foreach (ConnectorNode connector in logic.PersonLookup.Values)
            {
                if (connector.Node.Person != logic.Family.Current)
                    connector.Node.Visibility = Visibility.Hidden;
            }


            InvalidateVisual();
            InvalidateArrange();
            InvalidateMeasure();

            animationTimer.Interval = App.GetAnimationDuration(Const.AnimationPauseDuration);
            animationTimer.Tick += new EventHandler(OnAnimationTimer);
            animationTimer.IsEnabled = true;

            OnTreePopulated();
        }

        void OnAnimationTimer(object sender, EventArgs e)
        {
            animationTimer.IsEnabled = false;

            foreach (ConnectorNode connector in logic.PersonLookup.Values)
            {
                if (connector.Node.Visibility != Visibility.Visible)
                {
                    connector.Node.Visibility = Visibility.Visible;
                    connector.Node.BeginAnimation(OpacityProperty,
                        new DoubleAnimation(0, 1,
                        App.GetAnimationDuration(Const.NodeFadeInDuration)));
                }
            }

            InvalidateVisual();

            populating = false;
        }
        private void UpdateTree()
        {
            if (logic.Family == null)
                return;

            Clear();

            if (logic.Family.Current == null)
                return;

            Person primaryPerson = logic.Family.Current;
            Row primaryRow = logic.CreatePrimaryRow(primaryPerson, 1.0, Const.RelatedMultiplier);
            primaryRow.GroupSpace = Const.PrimaryRowGroupSpace;
            AddRow(primaryRow);

            int nodeCount = NodeCount;

            double nodeScale = 1.0;

            Row childRow = primaryRow;
            Row parentRow = primaryRow;

            while (nodeCount < Const.MaximumNodes && (childRow != null || parentRow != null))
            {
                if (childRow != null)
                    childRow = AddChildRow(childRow);

                if (parentRow != null)
                {
                    nodeScale *= Const.GenerationMultiplier;
                    parentRow = AddParentRow(parentRow, nodeScale);
                }

                nodeCount = NodeCount;
            }

            OnTreeUpdated();

            AnimateNewPerson();
        }

        private Row AddChildRow(Row row)
        {
            List<Person> children = Logic.GetChildren(row);
            if (children.Count == 0)
                return null;

            row.Margin = new Thickness(0, 0, 0, Const.RowSpace);

            Row childRow = logic.CreateChildrenRow(children, 1.0, Const.RelatedMultiplier);
            childRow.GroupSpace = Const.ChildRowGroupSpace;
            AddRow(childRow);
            return childRow;
        }

        private Row AddParentRow(Row row, double nodeScale)
        {
            Collection<Person> parents = Logic.GetParents(row);
            if (parents.Count == 0)
                return null;

            Row parentRow = logic.CreateParentRow(parents, nodeScale, nodeScale * Const.RelatedMultiplier);
            parentRow.Margin = new Thickness(0, 0, 0, Const.RowSpace);
            parentRow.GroupSpace = Const.ParentRowGroupSpace;
            InsertRow(parentRow);
            return parentRow;
        }

        private void AddRow(Row row)
        {
            if (row != null && row.NodeCount > 0)
            {
                AddVisualChild(row);
                rows.Add(row);
            }
        }

        private void InsertRow(Row row)
        {
            if (row != null && row.NodeCount > 0)
            {
                AddVisualChild(row);
                rows.Insert(0, row);
            }
        }

        #endregion
        private void OnFamilyCurrentChanged(object sender, EventArgs e)
        {
            selectedNodeBounds = logic.GetNodeBounds(logic.Family.Current);

            Populate();
        }

        private void OnFamilyContentChanged(object sender, ContentChangedEventArgs e)
        {
            if (populating)
            {
                return;
            }

            newPerson = e.NewPerson;

            UpdateTree();
            InvalidateMeasure();
            InvalidateArrange();
            InvalidateVisual();
        }


        private void OnNodeClick(object sender, RoutedEventArgs e)
        {
            Node node = sender as Node;
            if (node != null)
            {
                logic.Family.Current = node.Person;
            }
        }

        private void AnimateNewPerson()
        {
            if (newPerson == null)
            {
                return;
            }

            Node node = logic.GetNode(newPerson);
            if (node != null)
            {
                DoubleAnimation anim = new DoubleAnimation(0, 1, App.GetAnimationDuration(Const.NewPersonAnimationDuration));

                ScaleTransform transform = new ScaleTransform();
                transform.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
                transform.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
                node.RenderTransform = transform;
            }

            newPerson = null;
        }
    }
}
