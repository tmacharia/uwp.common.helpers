using System.Numerics;
using UWP.Common.Helpers;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Windows.UI.Xaml
{
    public static class XamlExtensions
    {
        public static bool IsVisible(this FrameworkElement elem) => elem.Visibility == Visibility.Visible;
        public static void Hide(this FrameworkElement elem) => elem.Visibility = Visibility.Collapsed;
        public static void UnHide(this FrameworkElement elem) => elem.Visibility = Visibility.Visible;
        public static void FadeIn(this DependencyObject dp, double secs = Animations.FadeDurationSecs)
            => dp.FadeInOrOut(true, secs);
        public static void FadeOut(this DependencyObject dp, double secs = Animations.FadeDurationSecs)
            => dp.FadeInOrOut(false, secs);
        public static void ScaleTo(this FrameworkElement dp, float val)
        {
            Animations.CreateOrUpdateSpringAnimation(val);
            dp.CenterPoint = new Vector3((float)(dp.ActualWidth / 2.0), (float)(dp.ActualHeight / 2.0), 1f);
            dp.StartAnimation(Animations._prdAnimation);
        }
        public static void OpenFlyout(this UIElement s, PointerRoutedEventArgs e)
        {
            if (s != null)
            {
                var pt = e.GetCurrentPoint(s);
                var props = pt.Properties;
                if (props.IsRightButtonPressed)
                {
                    s.ContextFlyout.ShowAt(s, new FlyoutShowOptions() { Position = pt.Position });
                    e.Handled = true;
                }
            }
        }
    }
}