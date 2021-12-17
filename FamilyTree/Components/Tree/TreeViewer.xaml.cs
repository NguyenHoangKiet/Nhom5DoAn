using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace FamilyTree.Components.Tree
{
    public partial class TreeViewer : UserControl
    {

        #region fields

        private static class Const
        {
            public static double PanMargin = 50;
            public static double AutoCenterAnimationPauseDuration = 1000;
            public static double AutoCenterAnimationDuration = 600;
        }
        private Point scrollStartPoint;
        private Point scrollStartOffset;
        private Point previousTopLeftOffset;
        DispatcherTimer autoCenterTimer;

        #endregion

        #region properties
        public double Zoom
        {
            get { return ZoomSlider.Value; }
            set
            {
                if (value >= ZoomSlider.Minimum && value <= ZoomSlider.Maximum)
                {
                    Tree.Scale = value;
                    ZoomSlider.Value = value;
                    UpdateScrollSize();
                }
            }
        }

        #endregion

        public TreeViewer()
        {
            InitializeComponent();
            Zoom = 1;
            UpdateTimeSlider();
            TimeSlider.Value = TimeSlider.Maximum;
        }

        #region event handlers

        protected override void OnInitialized(EventArgs e)
        {
            autoCenterTimer = new DispatcherTimer();
            Tree.Loaded += new RoutedEventHandler(Tree_Loaded);
            Tree.SizeChanged += new SizeChangedEventHandler(Tree_SizeChanged);
            Tree.TreeUpdated += new EventHandler(Tree_TreeUpdated);
            Tree.TreePopulated += new EventHandler(Tree_TreePopulated);

            ZoomSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(ZoomSlider_ValueChanged);
            ZoomSlider.MouseDoubleClick += new MouseButtonEventHandler(ZoomSlider_MouseDoubleClick);

            TimeSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(TimeSlider_ValueChanged);
            TimeSlider.MouseDoubleClick += new MouseButtonEventHandler(TimeSlider_MouseDoubleClick);

            SizeChanged += new SizeChangedEventHandler(Tree_SizeChanged);
            ScrollViewer.ScrollChanged += new ScrollChangedEventHandler(ScrollViewer_ScrollChanged);

            base.OnInitialized(e);
        }

        private void Tree_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateScrollSize();
            AutoScrollToSelected();
        }

        private void Tree_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateScrollSize();
        }

        private void Tree_TreeUpdated(object sender, EventArgs e)
        {
            UpdateTimeSlider();
        }

        private void Tree_TreePopulated(object sender, EventArgs e)
        {
            Point offset = GetTopLeftScrollOffset();
            previousTopLeftOffset = new Point(
                Grid.ActualWidth - ScrollViewer.HorizontalOffset - offset.X,
                Grid.ActualHeight - ScrollViewer.VerticalOffset - offset.Y);
            UpdateLayout();
            AutoScrollToSelected();
            TimeSlider.Value = TimeSlider.Maximum;
        }

        private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Zoom = e.NewValue;
        }

        private void ZoomSlider_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ZoomSlider.Value = 1.0;
        }

        private void TimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Tree.DisplayYear = e.NewValue;
        }

        private void TimeSlider_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TimeSlider.Value = TimeSlider.Maximum;
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

            if (e.ExtentWidthChange != 0 && e.ExtentWidthChange != e.ExtentWidth)
            {
                double percent = e.ExtentWidthChange / (e.ExtentWidth - e.ExtentWidthChange);
                double middle = e.HorizontalOffset + (e.ViewportWidth / 2);
                ScrollViewer.ScrollToHorizontalOffset(e.HorizontalOffset + (middle * percent));
            }

            if (e.ExtentHeightChange != 0 && e.ExtentHeightChange != e.ExtentHeight)
            {
                double percent = e.ExtentHeightChange / (e.ExtentHeight - e.ExtentHeightChange);
                double middle = e.VerticalOffset + (e.ViewportHeight / 2);
                ScrollViewer.ScrollToVerticalOffset(e.VerticalOffset + (middle * percent));
            }
        }

        #endregion
        private Point GetTopLeftScrollOffset()
        {
            Point offset = new Point();
            if (Tree.ActualWidth == 0 || Tree.ActualHeight == 0)
                return offset;
            Size TreeSize = new Size(
                Tree.ActualWidth * Zoom,
                Tree.ActualHeight * Zoom);
            offset.X = ActualWidth + TreeSize.Width - (Const.PanMargin / 2);
            offset.Y = ActualHeight + TreeSize.Height - (Const.PanMargin / 2);

            return offset;
        }
        private void UpdateScrollSize()
        {
            if (ActualWidth == 0 || ActualHeight == 0)
                return;

            Size TreeSize = new Size(
                Tree.ActualWidth * Zoom,
                Tree.ActualHeight * Zoom);
            Grid.Width = Math.Max(0, (ActualWidth * 2) + TreeSize.Width - Const.PanMargin);
            Grid.Height = Math.Max(0, (ActualHeight * 2) + TreeSize.Height - Const.PanMargin);
        }
        private void AutoScrollToSelected()
        {
            if (Tree.ActualWidth == 0 || Tree.ActualHeight == 0)
                return;
            Point offset = GetTopLeftScrollOffset();
            Rect selectedBounds = Tree.SelectedNodeBounds;
            if (selectedBounds.IsEmpty)
            {
                offset.X += ((ActualWidth - (Tree.ActualWidth * Zoom)) / 2);
                offset.Y += ((ActualHeight - (Tree.ActualHeight * Zoom)) / 2);
            }
            else
            {
                offset.X += previousTopLeftOffset.X;
                offset.Y += previousTopLeftOffset.Y;
                Rect primaryBounds = Tree.PrimaryNodeBounds;
                Point nodeDelta = new Point();
                nodeDelta.X = (primaryBounds.Left + (primaryBounds.Width / 2)) -
                    (selectedBounds.Left + (selectedBounds.Width / 2));
                nodeDelta.Y = (primaryBounds.Top + (primaryBounds.Height / 2)) -
                    (selectedBounds.Top + (selectedBounds.Height / 2));
                offset.X -= (nodeDelta.X * Zoom);
                offset.Y -= (nodeDelta.Y * Zoom);
            }
            ScrollViewer.ScrollToHorizontalOffset(Grid.Width - offset.X);
            ScrollViewer.ScrollToVerticalOffset(Grid.Height - offset.Y);
            autoCenterTimer.Interval = App.GetAnimationDuration(Const.AutoCenterAnimationPauseDuration);
            autoCenterTimer.Tick += new EventHandler(OnAutoCenterPauseTimer);
            autoCenterTimer.IsEnabled = true;
        }

        void OnAutoCenterPauseTimer(object sender, EventArgs e)
        {
            autoCenterTimer.IsEnabled = false;
            AutoScrollToCenter();
        }
        private void AutoScrollToCenter()
        {
            Point offset = GetTopLeftScrollOffset();
            offset.X += ((ActualWidth - (Tree.ActualWidth * Zoom)) / 2);
            offset.Y += ((ActualHeight - (Tree.ActualHeight * Zoom)) / 2);
            Point startLocation = new Point(
                ScrollViewer.HorizontalOffset,
                ScrollViewer.VerticalOffset);

            Point endLocation = new Point(
                Grid.Width - offset.X - startLocation.X,
                Grid.Height - offset.Y - startLocation.Y);
            ScrollViewer.ScrollToHorizontalOffset(Grid.Width - offset.X);
            ScrollViewer.ScrollToVerticalOffset(Grid.Height - offset.Y);
            AnimateTree(endLocation);
        }
        private void AnimateTree(Point endLocation)
        {
            DoubleAnimation horzAnim = new DoubleAnimation(endLocation.X, 0,
                App.GetAnimationDuration(Const.AutoCenterAnimationDuration));
            horzAnim.AccelerationRatio = .5;
            horzAnim.DecelerationRatio = .5;

            DoubleAnimation vertAnim = new DoubleAnimation(endLocation.Y, 0,
                App.GetAnimationDuration(Const.AutoCenterAnimationDuration));
            vertAnim.AccelerationRatio = .5;
            vertAnim.DecelerationRatio = .5;
            TranslateTransform transform = new TranslateTransform();
            transform.BeginAnimation(TranslateTransform.XProperty, horzAnim);
            transform.BeginAnimation(TranslateTransform.YProperty, vertAnim);
            Grid.RenderTransform = transform;
        }
        private void UpdateTimeSlider()
        {
            TimeSlider.Maximum = DateTime.Now.Year;
            TimeSlider.Minimum = Math.Min(
                DateTime.Now.Year - 10, Tree.MinimumYear - 1);
            TimeSlider.LargeChange = Math.Max(2,
                (int)((TimeSlider.Maximum - TimeSlider.Minimum) / 10));
        }
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) > 0)
            {
                e.Handled = true;
                Zoom += (e.Delta > 0) ? ZoomSlider.LargeChange : -ZoomSlider.LargeChange;
            }
            if ((Keyboard.Modifiers & ModifierKeys.Shift) > 0)
            {
                e.Handled = true;
                TimeSlider.Value += (e.Delta > 0) ? TimeSlider.LargeChange : -TimeSlider.LargeChange;
            }

            base.OnPreviewMouseWheel(e);
        }

        #region manual scroll

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (ScrollViewer.IsMouseOver && !Tree.IsMouseOver)
            {
                scrollStartPoint = e.GetPosition(this);
                scrollStartOffset.X = ScrollViewer.HorizontalOffset;
                scrollStartOffset.Y = ScrollViewer.VerticalOffset;
                Cursor = (ScrollViewer.ExtentWidth > ScrollViewer.ViewportWidth) ||
                    (ScrollViewer.ExtentHeight > ScrollViewer.ViewportHeight) ?
                    Cursors.ScrollAll : Cursors.Arrow;

                CaptureMouse();
            }

            base.OnPreviewMouseDown(e);
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (IsMouseCaptured)
            {
                Point point = e.GetPosition(this);
                Point delta = new Point(
                    (point.X > scrollStartPoint.X) ?
                        -(point.X - scrollStartPoint.X) : (scrollStartPoint.X - point.X),
                    (point.Y > scrollStartPoint.Y) ?
                        -(point.Y - scrollStartPoint.Y) : (scrollStartPoint.Y - point.Y));
                ScrollViewer.ScrollToHorizontalOffset(scrollStartOffset.X + delta.X);
                ScrollViewer.ScrollToVerticalOffset(scrollStartOffset.Y + delta.Y);
            }

            base.OnPreviewMouseMove(e);
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
            {
                Cursor = Cursors.Arrow;
                ReleaseMouseCapture();
            }

            base.OnPreviewMouseUp(e);
        }

        #endregion

    }
}
