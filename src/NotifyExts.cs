using System;
using System.Threading.Tasks;
using Common;
using Common.Primitives;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace UWP.Common.Helpers
{
    /// <summary>
    /// Represents type of notification bubble
    /// </summary>
    public enum NotificationType
    {
        /// <summary>
        /// Information notification.
        /// </summary>
        Info,
        /// <summary>
        /// Success Notification
        /// </summary>
        Success,
        /// <summary>
        /// Warning Notification
        /// </summary>
        Warning,
        /// <summary>
        /// Error Notification
        /// </summary>
        Error
    }
    public enum NotificationPosition
    {
        TopRight = 1,
        CenterRight = 5,
        BottomRight = 2,
        BottomCenter = 0,
        BottomLeft = 4,
        CenterLeft = 6,
        TopLeft = 3
    }
    public enum AnimationType
    {
        Entry,
        Exit
    }
    public static class NotifyExts
    {
        #region Constants
        public const string InfoGlyph = "\uE890";
        public const string SuccessGlyph = "\uE73E";
        public const string WarningGlyph = "\uE730";
        public const string ErrorGlyph = "\uE711";
        #endregion

        #region Color Variables
        public static Color InfoForeground = Colors.White;
        public static Color InfoBackground = "#025FA4".HexToColor();
        public static Color SuccessForeground = Colors.White;
        public static Color SuccessBackground = "#10A717".HexToColor();
        public static Color WarningForeground = Colors.Black;
        public static Color WarningBackground = "#FDA502".HexToColor();
        public static Color ErrorForeground = Colors.White;
        public static Color ErrorBackground = Colors.Red;
        #endregion

        #region Custom Icon Elements
        /// <summary>
        /// Set icon to use for <see cref="NotificationType.Info"/>
        /// </summary>
        //private static FrameworkElement InfoIcon = null;
        /// <summary>
        /// Set icon to use for <see cref="NotificationType.Error"/>
        /// </summary>
        //private static FrameworkElement ErrorIcon = null;
        /// <summary>
        /// Set icon to use for <see cref="NotificationType.Success"/>
        /// </summary>
        //private static FrameworkElement SuccessIcon;
        /// <summary>
        /// Set icon to use for <see cref="NotificationType.Warning"/>
        /// </summary>
        //private static FrameworkElement WarningIcon = null;
        #endregion

        #region Global Notification Settings
        /// <summary>
        /// Timeout Duration for each notification. Default is 2000(ms)
        /// </summary>
        public static int TimeoutDuration = 2000;
        /// <summary>
        /// Animation duration for entry & exit of notifications. Default is 400(ms)
        /// </summary>
        public static double AnimationDuration = 400;
        /// <summary>
        /// FontSize of notification text.
        /// </summary>
        public static int FontSize = 15;
        /// <summary>
        /// Corner Radius of the notification bubble.
        /// </summary>
        public static int CornerRadius = 2;
        /// <summary>
        /// Notification bubble inner padding.
        /// </summary>
        public static Thickness ContainerPadding = new Thickness(15, 10, 15, 10);
        /// <summary>
        /// Offset distance for a notification on the Y-Axis. Default is 150
        /// </summary>
        public static int YOffset = 100;
        /// <summary>
        /// Offset distance for a notification on the X-Axis. Default is 100
        /// </summary>
        public static int XOffset = 100;
        #endregion


        /// <summary>
        /// Display a notification bubble message to the user.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="msg">Information Message.</param>
        /// <param name="pos">Location to show/place the notification.</param>
        public static void Info(this Page page, string msg = null, NotificationPosition pos = NotificationPosition.BottomCenter) 
            => page.ToggleNotification(NotificationType.Info, msg, pos);
        /// <summary>
        /// Display a success notification bubble message to the user.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="msg">Success Message.</param>
        /// <param name="pos">Location to show/place the notification.</param>
        public static void Success(this Page page, string msg = null, NotificationPosition pos = NotificationPosition.BottomCenter) 
            => page.ToggleNotification(NotificationType.Success, msg, pos);
        /// <summary>
        /// Display a warning notification bubble message to the user.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="msg">Warning Message.</param>
        /// <param name="pos">Location to show/place the notification.</param>
        public static void Warning(this Page page, string msg = null, NotificationPosition pos = NotificationPosition.BottomCenter)
            => page.ToggleNotification(NotificationType.Warning, msg, pos);
        /// <summary>
        /// Display an error notification bubble message to the user.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="msg">Error Message.</param>
        /// <param name="pos">Location to show/place the notification.</param>
        public static void Error(this Page page, string msg = null, NotificationPosition pos = NotificationPosition.BottomCenter) 
            => page.ToggleNotification(NotificationType.Error, msg, pos);
        private static void ToggleNotification(this Page page, NotificationType type = NotificationType.Info, string msg = null, NotificationPosition pos = NotificationPosition.BottomCenter) 
            => page.GenerateNotification(type, msg, pos);
        private static void GenerateNotification(this Page pg, NotificationType nt = NotificationType.Info, string msg = null, NotificationPosition pos = NotificationPosition.BottomCenter) 
            => pg.ProcessNotification(nt, msg, pos, $"NTF_KEY_{Constants.random.Next()}");
        private static void ProcessNotification(this Page pg, NotificationType nt, string msg, NotificationPosition pos, string NotificationKey)
        {
            var panel = (Panel)pg.Content;
            var item = panel.GetElem<Grid>(NotificationKey);
            if(item != null)
            {
                bool b = panel.Children.Remove(item);
                return;
            }
            else
            {
                var b = GetGrid(NotificationKey, pos, nt, msg);
                var a = b.GetStoryboards(pos);
                panel.Children.Add(a.grid);
                var entry = a.entry;
                var exit = a.exit;
                exit.Completed += (s, e) =>
                {
                    bool c = panel.Children.Remove(a.grid);
                    entry.Stop();
                    exit.Stop();
                    exit.Children.Clear();
                    entry.Children.Clear();
                    a.grid.Children.Clear();
                    a.grid.RenderTransform = null;
                    return;
                };
                entry.Completed += EntryAnimation_Completed;
                entry.Begin();
                async void EntryAnimation_Completed(object sender, object e)
                {
                    await Task.Delay(TimeoutDuration).ContinueWith((t) =>
                    {
                        pg.LoadOnUI(() =>
                        {
                            exit.Begin();
                        });
                    });
                }
                return;
            }
        }
        private static Grid GetGrid(string name, NotificationPosition pos, NotificationType type, string txt)
        {
            var grid = new Grid
            {
                Name = name,
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0),
                Visibility = Visibility.Visible
            }.ModifyGridByPosition(pos);
            grid.Children.Add(GetStackPanel(type, txt));
            return grid;
        }
        private static (Grid grid, Storyboard entry, Storyboard exit) GetStoryboards(this Grid grid, NotificationPosition pos = NotificationPosition.BottomCenter)
        {
            grid.RenderTransform = new TranslateTransform();
            Storyboard entry = new Storyboard();
            Storyboard exit = new Storyboard();
            double e = 1000;
            DoubleAnimation d1_entry = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(AnimationDuration)),
                EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut }
            }.ModifyAnimationBasedOnPosition(AnimationType.Entry, pos);
            DoubleAnimation d2_exit = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(e)),
                EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut, }
            }.ModifyAnimationBasedOnPosition(AnimationType.Exit, pos);
            FadeInThemeAnimation fade_in = new FadeInThemeAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(AnimationDuration))
            };
            FadeOutThemeAnimation fade_out = new FadeOutThemeAnimation
            {
                BeginTime = TimeSpan.FromMilliseconds(e/3),
                Duration = new Duration(TimeSpan.FromMilliseconds(e))
            };
            Storyboard.SetTarget(fade_in, grid);
            Storyboard.SetTarget(fade_out, grid);
            Storyboard.SetTarget(d1_entry, grid.RenderTransform);
            Storyboard.SetTarget(d2_exit, grid.RenderTransform);
            Storyboard.SetTargetProperty(d1_entry, pos.GetTargetPropFromPosition());
            Storyboard.SetTargetProperty(d2_exit, pos.GetTargetPropFromPosition());
            entry.Children.Add(d1_entry);
            entry.Children.Add(fade_in);
            exit.Children.Add(d2_exit);
            exit.Children.Add(fade_out);
            return (grid, entry, exit);
        }
        private static string GetTargetPropFromPosition(this NotificationPosition pos)
        {
            switch (pos)
            {
                case NotificationPosition.TopRight:
                    return "Y";
                case NotificationPosition.CenterRight:
                    return "X";
                case NotificationPosition.BottomRight:
                    return "Y";
                case NotificationPosition.BottomCenter:
                    return "Y";
                case NotificationPosition.BottomLeft:
                    return "Y";
                case NotificationPosition.CenterLeft:
                    return "X";
                case NotificationPosition.TopLeft:
                    return "Y";
                default:
                    return "Y";
            }
        }
        private static DoubleAnimation ModifyAnimationBasedOnPosition(this DoubleAnimation da, AnimationType type, NotificationPosition pos = NotificationPosition.BottomCenter)
        {
            double from = 0, to = 0;
            if (pos == NotificationPosition.TopRight)
            {
                from = type == AnimationType.Entry ? YOffset.Negate() : YOffset;
                to = type == AnimationType.Entry ? YOffset : (YOffset * 2);
            }
            else if (pos == NotificationPosition.BottomCenter)
            {
                from = type == AnimationType.Entry ? 0 : YOffset.Negate();
                to = type == AnimationType.Entry ? YOffset.Negate() : (YOffset * 2).Negate();
            }
            else if (pos == NotificationPosition.BottomRight)
            {
                from = type == AnimationType.Entry ? 0 : YOffset.Negate();
                to = type == AnimationType.Entry ? YOffset.Negate() : (YOffset * 2).Negate();
            }
            else if (pos == NotificationPosition.BottomLeft)
            {
                from = type == AnimationType.Entry ? 0 : YOffset.Negate();
                to = type == AnimationType.Entry ? YOffset.Negate() : (YOffset * 2).Negate();
            }
            else if (pos == NotificationPosition.TopLeft)
            {
                from = type == AnimationType.Entry ? YOffset.Negate() : YOffset;
                to = type == AnimationType.Entry ? YOffset : (YOffset * 2);
            }
            da.From = from;
            da.To = to;
            return da;
        }
        private static Grid ModifyGridByPosition(this Grid grid, NotificationPosition pos)
        {
            int pad = 10;
            switch (pos)
            {
                case NotificationPosition.TopRight:
                    grid.VerticalAlignment = VerticalAlignment.Top;
                    grid.HorizontalAlignment = HorizontalAlignment.Right;
                    grid.Margin = new Thickness(0, YOffset.Negate(), pad, 0);
                    break;
                case NotificationPosition.CenterRight:
                    grid.VerticalAlignment = VerticalAlignment.Center;
                    grid.HorizontalAlignment = HorizontalAlignment.Right;
                    break;
                case NotificationPosition.BottomRight:
                    grid.VerticalAlignment = VerticalAlignment.Bottom;
                    grid.HorizontalAlignment = HorizontalAlignment.Right;
                    grid.Margin = new Thickness(0, 0, pad, YOffset.Negate());
                    break;
                case NotificationPosition.BottomCenter:
                    grid.VerticalAlignment = VerticalAlignment.Bottom;
                    grid.HorizontalAlignment = HorizontalAlignment.Center;
                    grid.Margin = new Thickness(0, 0, 0, YOffset.Negate());
                    break;
                case NotificationPosition.BottomLeft:
                    grid.VerticalAlignment = VerticalAlignment.Bottom;
                    grid.HorizontalAlignment = HorizontalAlignment.Left;
                    grid.Margin = new Thickness(pad, 0, 0, YOffset.Negate());
                    break;
                case NotificationPosition.CenterLeft:
                    grid.VerticalAlignment = VerticalAlignment.Center;
                    grid.HorizontalAlignment = HorizontalAlignment.Left;
                    grid.Margin = new Thickness(0);
                    break;
                case NotificationPosition.TopLeft:
                    grid.VerticalAlignment = VerticalAlignment.Top;
                    grid.HorizontalAlignment = HorizontalAlignment.Left;
                    grid.Margin = new Thickness(pad, YOffset.Negate(), 0, 0);
                    break;
                default:
                    break;
            }
            return grid;
        }
        private static StackPanel GetStackPanel(NotificationType type, string txt)
        {
            var stack = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
                CornerRadius = new CornerRadius(CornerRadius),
                Padding = ContainerPadding,
            };
            Color bg = InfoBackground;
            Color forecolor = InfoForeground;
            FontIcon icon = null;
            string glyph = InfoGlyph;
            switch (type)
            {
                case NotificationType.Info:
                    glyph = InfoGlyph;
                    bg = InfoBackground;
                    forecolor = InfoForeground;
                    //icon = InfoIcon ?? icon; 
                    break;
                case NotificationType.Success:
                    glyph = SuccessGlyph;
                    bg = SuccessBackground;
                    forecolor = SuccessForeground;
                    //icon = SuccessIcon ?? icon; 
                    break;
                case NotificationType.Warning:
                    glyph = WarningGlyph;
                    bg = WarningBackground;
                    forecolor = WarningForeground;
                    //icon = WarningIcon ?? icon; 
                    break;
                case NotificationType.Error:
                    glyph = ErrorGlyph;
                    bg = ErrorBackground;
                    forecolor = ErrorForeground;
                    //icon = ErrorIcon ?? icon; 
                    break;
                default: break;
            }
            if (icon == null)
            {
                icon = new FontIcon()
                {
                    Glyph = glyph,
                    FontSize = FontSize,
                    Foreground = forecolor.ToBrush(),
                    FontWeight = new FontWeight() { Weight = 700 },
                };
            }
            icon.Margin = new Thickness(0, 0, 10, 0);
            stack.Background = bg.ToBrush();
            TextBlock tb = new TextBlock
            {
                FontSize = FontSize,
                Text = txt,
                Foreground = forecolor.ToBrush()
            };
            try
            {
                stack.Children.Add(icon);
            }
            catch (Exception)
            { }
            stack.Children.Add(tb);
            return stack;
        }
    }
}