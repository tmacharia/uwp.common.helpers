using System;
using Common;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace UWP.Common.Helpers.Controls
{
    public class GaugeControl : Grid
    {
        private const double MinValue = 0.0;
        private const double DefaultMaxValue = 100.0;
        private const double MaxAngle = 359.999;
        private const double RADIANS = Math.PI / 180;
        private const double DefaultFontSize = 14.0;
        private const double DefaultRadius = 50.0;
        private const double DefaultThickness = 2.0;

        private static readonly SolidColorBrush DefaultPathBrush = new SolidColorBrush(Colors.Green);
        private static readonly SolidColorBrush DefaultInnerPathBrush = new SolidColorBrush(Colors.Gray);

        public GaugeControl()
        {
            this.Background = new SolidColorBrush(Colors.Transparent);
            this.DrawGauge();
        }

        private double FullSize => (Radius * DefaultThickness) + Thickness;
        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }
        public double Thickness
        {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }
        public SolidColorBrush PathBrush
        {
            get { return (SolidColorBrush)GetValue(PathBrushProperty); }
            set { SetValue(PathBrushProperty, value); }
        }
        public SolidColorBrush InnerPathBrush
        {
            get { return (SolidColorBrush)GetValue(InnerPathBrushProperty); }
            set { SetValue(InnerPathBrushProperty, value); }
        }
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }
        public FrameworkElement InnerPanel
        {
            get { return (Panel)GetValue(InnerPanelProperty); }
            set { SetValue(InnerPanelProperty, value); }
        }
        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }
        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }


        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register(nameof(MaxValue), typeof(double), typeof(GaugeControl), new PropertyMetadata(DefaultMaxValue, (s, e) => OnPropertyChanged(s)));
        public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register(nameof(FontWeight), typeof(FontWeight), typeof(GaugeControl), new PropertyMetadata(FontWeights.Normal, (s, e) => OnPropertyChanged(s)));
        public static readonly DependencyProperty InnerPanelProperty = DependencyProperty.Register(nameof(InnerPanel), typeof(FrameworkElement), typeof(GaugeControl), new PropertyMetadata(null, (s, e) => OnPropertyChanged(s)));
        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(nameof(FontSize), typeof(double), typeof(GaugeControl), new PropertyMetadata(DefaultFontSize, (s, e) => OnPropertyChanged(s)));
        public static readonly DependencyProperty InnerPathBrushProperty = DependencyProperty.Register(nameof(InnerPathBrush), typeof(SolidColorBrush), typeof(GaugeControl), new PropertyMetadata(DefaultInnerPathBrush, (s, e) => OnPropertyChanged(s)));
        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(nameof(Radius), typeof(double), typeof(GaugeControl), new PropertyMetadata(DefaultRadius, (s, e) => OnPropertyChanged(s)));
        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register(nameof(Thickness), typeof(double), typeof(GaugeControl), new PropertyMetadata(DefaultThickness, (s, e) => OnPropertyChanged(s)));
        public static readonly DependencyProperty PathBrushProperty = DependencyProperty.Register(nameof(PathBrush), typeof(SolidColorBrush), typeof(GaugeControl), new PropertyMetadata(DefaultPathBrush, (s, e) => OnPropertyChanged(s)));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(double), typeof(GaugeControl), new PropertyMetadata(MinValue, (s, e) => OnPropertyChanged(s)));



        #region Event Callback Methods
        private static void OnPropertyChanged(DependencyObject d) => ((GaugeControl)d).DrawGauge();
        private void DrawGauge(double? r1 = null, double? r2 = null)
        {
            this.Children.Clear();
            this.Width = FullSize;
            this.Height = FullSize;

            r1 = r1.HasValue ? r1 : Radius;
            r2 = r2.HasValue ? r2 : Radius;

            Path innerPath = CreateArc(GetCenterPoint(), r1.Value, MaxAngle);
            innerPath.Stroke = InnerPathBrush;
            innerPath.StrokeThickness = Thickness <= 2 ? Thickness - 1 : Thickness - 2;

            Path path = CreateArc(GetCenterPoint(), r2.Value, ConvertPercentToAngle());
            path.Stroke = PathBrush;
            path.StrokeThickness = Thickness;
            path.PointerEntered += (s, e) => DrawGauge(null, Radius - 12);
            path.PointerExited += (s, e) => DrawGauge();

            FrameworkElement inner = InnerPanel ?? GetInnerTxt();
            inner.VerticalAlignment = VerticalAlignment.Center;
            inner.HorizontalAlignment = HorizontalAlignment.Center;

            Children.Add(innerPath);
            Children.Add(path);
            Children.Add(inner);
        }
        #endregion

        private TextBlock GetInnerTxt()
        {
            return new TextBlock
            {
                FontSize = FontSize,
                Text = Value.ToHuman(0) + "%",
                FontWeight = FontWeight
            };
        }


        #region Helper Functions
        private Point GetCenterPoint() => new Point(Radius + (Thickness / 2), Radius + (Thickness / 2));
        private Point ScaleUnitCirclePoint(Point origin, double angle, double radius)
            => new Point(origin.X + Math.Sin(RADIANS * angle) * radius, origin.Y - Math.Cos(RADIANS * angle) * radius);
        private double ConvertPercentToAngle()
        {
            double angle = Value * 360 / MaxValue;
            return angle >= 360 ? MaxAngle : angle;
        }
        public Path CreateArc(Point center, double radius, double angle)
        {
            var figure = new PathFigure
            {
                StartPoint = new Point(center.X, center.Y - radius),
                IsClosed = false
            };
            var arc = new ArcSegment
            {
                IsLargeArc = angle > 180.0,
                Point = ScaleUnitCirclePoint(center, angle, radius),
                Size = new Size(radius, radius),
                SweepDirection = SweepDirection.Clockwise
            };
            figure.Segments.Add(arc);
            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);
            return new Path
            {
                Data = geometry
            };
        }
        #endregion
    }
}