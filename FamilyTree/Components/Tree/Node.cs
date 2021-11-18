using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using FamilyTreeLibrary;

namespace FamilyTree.Components
{
    public enum NodeType
    {
        Primary,
        Related,
        Spouse,
        Sibling,
        SiblingLeft,
        SiblingRight
    }

    public partial class Node : Button
    {
        private static class Const
        {
            public static double OpacityFiltered = 0.15;
            public static double OpacityNormal = 1.0;
            public static double AnimationDuration = 300;
        }

        #region fields

        private Person person;

        private Point location = new Point();

        private NodeType type = NodeType.Related;

        private double scale = 1;

        private double displayYear = DateTime.Now.Year;

        private bool isFiltered;

        #endregion

        #region properties

        public Brush NodeFill
        {
            get { return GetBrushResource("Fill"); }
        }

        public Brush NodeHoverFill
        {
            get { return GetBrushResource("HoverFill"); }
        }

        public Brush NodeStroke
        {
            get { return GetBrushResource("Stroke"); }
        }

        public Brush GroupFill
        {
            get { return GetGroupBrushResource("GroupFill"); }
        }

        public Brush GroupStroke
        {
            get { return GetGroupBrushResource("GroupStroke"); }
        }

        public double DisplayYear
        {
            get { return displayYear; }
            set
            {
                displayYear = value;
                IsFiltered = (person != null && person.BirthDate != null && person.BirthDate.Value.Year > displayYear);

                UpdateBottomLabel();
            }
        }

        public bool IsFiltered
        {
            get { return isFiltered; }
            set
            {
                if (isFiltered != value)
                {
                    isFiltered = value;
                    double newOpacity = isFiltered ? Const.OpacityFiltered : Const.OpacityNormal;
                    BeginAnimation(OpacityProperty,
                        new DoubleAnimation(Opacity, newOpacity,
                        App.GetAnimationDuration(Const.AnimationDuration)));
                }
            }
        }


        private string DateInformation
        {
            get
            {
                if (person.IsLiving)
                {
                    if (person.BirthDate == null)
                    {
                        return string.Empty;
                    }

                    if (!person.Age.HasValue)
                    {
                        return string.Empty;
                    }

                    int age = person.Age.Value - (DateTime.Now.Year - (int)displayYear);
                    return string.Format(CultureInfo.CurrentUICulture, "{0} | {1}", person.BirthDate.Value.Year, Math.Max(0, age));
                }

                if (person.BirthDate != null && person.DeathDate != null)
                {
                    if (!person.Age.HasValue)
                    {
                        return string.Empty;
                    }

                    int age = (displayYear >= person.DeathDate.Value.Year) ?
                        person.Age.Value : person.Age.Value - (person.DeathDate.Value.Year - (int)displayYear);

                    return string.Format(CultureInfo.CurrentUICulture,
                        "{0} - {1} | {2}", person.BirthDate.Value.Year,
                        person.DeathDate.Value.Year, Math.Max(0, age));
                }

                if (person.BirthDate == null && person.DeathDate != null)
                {
                    return string.Format(CultureInfo.CurrentUICulture,
                        "? - {0} | ?", person.DeathDate.Value.Year);
                }

                if (person.BirthDate != null && person.DeathDate == null)
                {
                    return string.Format(CultureInfo.CurrentUICulture,
                        "{0} - ? | ?", person.BirthDate.Value.Year);
                }

                return string.Empty;
            }
        }

        public Person Person
        {
            get { return person; }
            set
            {
                person = value;
                DataContext = this;

                UpdateTemplate();

                UpdateBottomLabel();
            }
        }

        public double Scale
        {
            get { return scale; }
            set
            {
                scale = value;
            }
        }

        public Point Location
        {
            get { return location; }
            set { location = value; }
        }

        public Point Center
        {
            get
            {
                return new Point(location.X + (DesiredSize.Width / 2), location.Y + (DesiredSize.Height / 2));
            }
        }

        public Point TopCenter
        {
            get
            {
                Point point = new Point(location.X + (DesiredSize.Width / 2), location.Y);

                FrameworkElement personElement = Template.FindName("Person", this) as FrameworkElement;
                double offset = (type == NodeType.Primary) ? 12 : 5;
                point.X -= (personElement.ActualWidth / offset);
                return point;
            }
        }

        public Point TopRight
        {
            get { return new Point(location.X + DesiredSize.Width, location.Y); }
        }

        public Point TopLeft
        {
            get { return new Point(location.X, location.Y); }
        }

        public Point BottomCenter
        {
            get
            {
                return new Point(location.X + (DesiredSize.Width / 2), location.Y + DesiredSize.Height);
            }
        }

        public Point LeftCenter
        {
            get { return new Point(location.X, location.Y + (DesiredSize.Height / 2)); }
        }

        public Point RightCenter
        {
            get { return new Point(location.X + DesiredSize.Width, location.Y + (DesiredSize.Height / 2)); }
        }

        public NodeType Type
        {
            get { return type; }
            set
            {
                type = value;
                UpdateTemplate();
            }
        }

        #endregion

        #region dependency properties

        public static readonly DependencyProperty BottomLabelProperty = DependencyProperty.Register("BottomLabel", typeof(string), typeof(Node));

        public string BottomLabel
        {
            get { return (string)GetValue(BottomLabelProperty); }
            set { SetValue(BottomLabelProperty, value); }
        }

        #endregion

        #region overrides

        public override void OnApplyTemplate()
        {
            if (scale != 1)
            {
                FrameworkElement personElement = Template.FindName("Person", this) as FrameworkElement;
                if (personElement != null)
                {
                    ScaleTransform transform = new ScaleTransform(scale, scale);
                    personElement.LayoutTransform = transform;
                }
            }

            UpdateGroupIndicator();

            base.OnApplyTemplate();
        }

        #endregion

        private Brush GetBrushResource(string part)
        {
            string resourceName = string.Format(
                CultureInfo.InvariantCulture, "{0}{1}{2}{3}",
                (person.Gender == Gender.Female) ? "Female" : "Male",
                type.ToString(),
                person.IsLiving ? "Living" : "Deceased",
                part);

            return (Brush)TryFindResource(resourceName);
        }

        private Brush GetGroupBrushResource(string part)
        {
            string resourceName = string.Format(CultureInfo.InvariantCulture, "{0}{1}", type.ToString(), part);

            return (Brush)TryFindResource(resourceName);
        }

        private void UpdateTemplate()
        {
            string template = string.Format(
                CultureInfo.InvariantCulture, "{0}{1}NodeTemplate",
                (person.Gender == Gender.Female) ? "Female" : "Male",
                (type == NodeType.Primary) ? "Primary" : "");

            Template = (ControlTemplate)FindResource(template);
        }

        private void UpdateGroupIndicator()
        {
            if (type == NodeType.Primary)
            {
                return;
            }

            bool isGrouping = ShouldDisplayGroupIndicator();

            FrameworkElement element = Template.FindName("Group", this) as FrameworkElement;
            if (element != null)
            {
                element.Visibility = isGrouping ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private bool ShouldDisplayGroupIndicator()
        {
            if (type == NodeType.Primary || type == NodeType.Related)
            {
                return false;
            }

            bool show = false;
            switch (type)
            {
                case NodeType.Spouse:
                    if (person.Parents.Count > 0 || person.Siblings.Count > 0 || person.PreviousSpouses.Count > 0)
                    {
                        show = true;
                    }

                    break;

                case NodeType.Sibling:
                    if (person.Spouses.Count > 0 || person.Children.Count > 0)
                    {
                        show = true;
                    }

                    break;

                case NodeType.SiblingLeft:
                case NodeType.SiblingRight:
                    if (person.Spouses.Count > 0 || person.Children.Count > 0)
                    {
                        show = true;
                    }

                    break;
            }

            return show;
        }

        private void UpdateBottomLabel()
        {
            string label = string.Format(CultureInfo.CurrentCulture, "{0}\r{1}", person.FullName, DateInformation);
            BottomLabel = label;
        }
    }
}
}