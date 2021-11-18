using FamilyTree;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using FamilyTreeLibrary;

namespace FamilyTree.Components.Tree
{
    public class ConnectorNode
    {
        #region fields

        private Row row;
        private Group group;
        private Node node;

        #endregion

        #region properties

        public Node Node
        {
            get { return node; }
        }

        public Point Center
        {
            get { return GetPoint(node.Center); }
        }

        public Point LeftCenter
        {
            get { return GetPoint(node.LeftCenter); }
        }

        public Point RightCenter
        {
            get { return GetPoint(node.RightCenter); }
        }

        public Point TopCenter
        {
            get { return GetPoint(node.TopCenter); }
        }

        public Point TopRight
        {
            get { return GetPoint(node.TopRight); }
        }

        public Point TopLeft
        {
            get { return GetPoint(node.TopLeft); }
        }

        #endregion

        public ConnectorNode(Node node, Group group, Row row)
        {
            this.node = node;
            this.group = group;
            this.row = row;
        }

        private Point GetPoint(Point point)
        {
            point.Offset(
                row.Location.X + group.Location.X,
                row.Location.Y + group.Location.Y);

            return point;
        }
    }


    public abstract class Connector
    {
        private static class Const
        {
            public static double OpacityFiltered = 0.15;
            public static double OpacityNormal = 1.0;
            public static double AnimationDuration = 300;
        }

        #region fields

        private ConnectorNode start;
        private ConnectorNode end;

        private bool isFiltered;

        private DoubleAnimation animation;

        private Pen resourcePen;

        #endregion

        virtual public bool IsChildConnector
        {
            get { return true; }
        }

        virtual public DateTime? MarriedDate
        {
            get { return null; }
        }

        virtual public DateTime? PreviousMarriedDate
        {
            get { return null; }
        }

        protected ConnectorNode StartNode
        {
            get { return start; }
        }
        protected ConnectorNode EndNode
        {
            get { return end; }
        }

        protected Pen ResourcePen
        {
            get { return resourcePen; }
            set { resourcePen = value; }
        }

        protected Pen Pen
        {
            get
            {
                Pen connectorPen = ResourcePen.Clone();

                connectorPen.Brush.Opacity = (isFiltered) ? Const.OpacityFiltered : Const.OpacityNormal;

                if (animation != null)
                    connectorPen.Brush.BeginAnimation(Brush.OpacityProperty, animation);

                return connectorPen;
            }
        }

        private bool IsFiltered
        {
            set { isFiltered = value; }
            get { return isFiltered; }
        }

        virtual protected bool NewFilteredState
        {
            get
            {
                if (start.Node.IsFiltered || end.Node.IsFiltered)
                    return true;

                return false;
            }
        }

        protected Connector(ConnectorNode startConnector,
            ConnectorNode endConnector)
        {
            start = startConnector;
            end = endConnector;
        }
        virtual public bool Draw(DrawingContext drawingContext)
        {
            if (start.Node.Visibility != Visibility.Visible ||
                end.Node.Visibility != Visibility.Visible)
                return false;

            CheckIfFilteredChanged();

            return true;
        }

        protected SolidColorBrush GetBrush(Color color)
        {
            SolidColorBrush brush = new SolidColorBrush(color);

            brush.Opacity = (isFiltered) ? Const.OpacityFiltered : Const.OpacityNormal;

            if (animation != null)
                brush.BeginAnimation(Brush.OpacityProperty, animation);

            return brush;
        }

        protected void CheckIfFilteredChanged()
        {
            bool newFiltered = NewFilteredState;
            if (newFiltered != IsFiltered)
            {
                IsFiltered = newFiltered;
                animation = new DoubleAnimation();
                animation.From = isFiltered ? Const.OpacityNormal : Const.OpacityFiltered;
                animation.To = isFiltered ? Const.OpacityFiltered : Const.OpacityNormal;
                animation.Duration = App.GetAnimationDuration(Const.AnimationDuration);
            }
            else
            {
                animation = null;
            }
        }
    }


    public class ChildConnector : Connector
    {
        public ChildConnector(ConnectorNode startConnector,
            ConnectorNode endConnector) : base(startConnector, endConnector)
        {
            ResourcePen = (Pen)Application.Current.TryFindResource("ChildConnectionPen");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        override public bool Draw(DrawingContext drawingContext)
        {
            if (!base.Draw(drawingContext))
                return false;

            drawingContext.DrawLine(Pen, StartNode.Center, EndNode.Center);
            return true;
        }
    }


    public class MarriedConnector : Connector
    {
        #region fields

        private double connectionTextSize;
        private Color connectionTextColor;
        private FontFamily connectionTextFont;

        private bool married;

        #endregion

        #region properties
        override public bool IsChildConnector
        {
            get { return false; }
        }

        override public DateTime? MarriedDate
        {
            get
            {
                if (married)
                {
                    SpouseRelationship rel = StartNode.Node.Person.GetSpouseRelationship(EndNode.Node.Person);
                    if (rel != null)
                        return rel.MarriageDate;
                }
                return null;
            }
        }

        override public DateTime? PreviousMarriedDate
        {
            get
            {
                if (!married)
                {
                    SpouseRelationship rel = StartNode.Node.Person.GetSpouseRelationship(EndNode.Node.Person);
                    if (rel != null)
                        return rel.DivorceDate;
                }
                return null;
            }
        }

        override protected bool NewFilteredState
        {
            get
            {
                if (base.NewFilteredState)
                    return true;

                SpouseRelationship rel = StartNode.Node.Person.GetSpouseRelationship(EndNode.Node.Person);
                if (rel != null && rel.MarriageDate != null &&
                    (StartNode.Node.DisplayYear < rel.MarriageDate.Value.Year))
                {
                    return true;
                }

                if (!married && rel != null && rel.DivorceDate != null &&
                    (StartNode.Node.DisplayYear < rel.DivorceDate.Value.Year))
                {
                    return true;
                }

                return false;
            }
        }

        #endregion

        public MarriedConnector(bool isMarried,
            ConnectorNode startConnector, ConnectorNode endConnector) :
            base(startConnector, endConnector)
        {
            married = isMarried;

            connectionTextSize = (double)Application.Current.TryFindResource("ConnectionTextSize");
            connectionTextColor = (Color)Application.Current.TryFindResource("ConnectionTextColor");
            connectionTextFont = (FontFamily)Application.Current.TryFindResource("ConnectionTextFont");

            ResourcePen = (Pen)Application.Current.TryFindResource(
                married ? "MarriedConnectionPen" : "FormerConnectionPen");
        }

        override public bool Draw(DrawingContext drawingContext)
        {
            if (!base.Draw(drawingContext))
                return false;

            DrawMarried(drawingContext);
            return true;
        }

        private void DrawMarried(DrawingContext drawingContext)
        {
            const double TextSpace = 3;

            Point startPoint = (StartNode.TopCenter.X < EndNode.TopCenter.X) ? StartNode.TopCenter : EndNode.TopCenter;
            Point endPoint = (StartNode.TopCenter.X < EndNode.TopCenter.X) ? EndNode.TopCenter : StartNode.TopCenter;

            double arcHeight = (endPoint.X - startPoint.X) / 4;
            Point middlePoint = new Point(startPoint.X + ((endPoint.X - startPoint.X) / 2), startPoint.Y - arcHeight);

            Rect bounds = DrawArc(drawingContext, Pen, startPoint, middlePoint, endPoint);

            SpouseRelationship rel = StartNode.Node.Person.GetSpouseRelationship(EndNode.Node.Person);
            if (rel != null)
            {
                if (rel.MarriageDate != null)
                {
                    string text = rel.MarriageDate.Value.Year.ToString(CultureInfo.CurrentCulture);

                    FormattedText format = new FormattedText(text,
                        CultureInfo.CurrentUICulture,
                        FlowDirection.LeftToRight, new Typeface(connectionTextFont,
                        FontStyles.Normal, FontWeights.Normal, FontStretches.Normal,
                        connectionTextFont), connectionTextSize, GetBrush(connectionTextColor));

                    drawingContext.DrawText(format, new Point(
                        bounds.Left + ((bounds.Width / 2) - (format.Width / 2)),
                        bounds.Top - format.Height - TextSpace));
                }

                if (!married && rel.DivorceDate != null)
                {
                    string text = rel.DivorceDate.Value.Year.ToString(CultureInfo.CurrentCulture);

                    FormattedText format = new FormattedText(text,
                        CultureInfo.CurrentUICulture,
                        FlowDirection.LeftToRight, new Typeface(connectionTextFont,
                        FontStyles.Normal, FontWeights.Normal, FontStretches.Normal,
                        connectionTextFont), connectionTextSize, GetBrush(connectionTextColor));

                    drawingContext.DrawText(format, new Point(
                        bounds.Left + ((bounds.Width / 2) - (format.Width / 2)),
                        bounds.Top + TextSpace));
                }
            }
        }

        private static Rect DrawArc(DrawingContext drawingContext, Pen pen,
            Point startPoint, Point middlePoint, Point endPoint)
        {
            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure();
            figure.StartPoint = startPoint;
            figure.Segments.Add(new QuadraticBezierSegment(middlePoint, endPoint, true));
            geometry.Figures.Add(figure);
            drawingContext.DrawGeometry(null, pen, geometry);
            return geometry.Bounds;
        }
    }

}
