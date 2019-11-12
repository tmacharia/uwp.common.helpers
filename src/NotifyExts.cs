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
    public static class NotifyExts
    {
        static NotifyExts()
        {
            //FontAwesomeIcon.check
        }
        private const string gridKey = "notificationGrid";
        private const string stackKey = "notificationStack";
        private const string iconKey = "notificationIcon";
        private const string textKey = "notificationTxt";

        public const string InfoGlyph = "\uE890";
        public const string SuccessGlyph = "\uE73E";
        public const string WarningGlyph = "\uE730";
        public const string ErrorGlyph = "\uE711";

        public static Color InfoForeground = Colors.White;
        public static Color InfoBackground = "#0382dc".HexToColor();
        public static Color SuccessForeground = Colors.White;
        public static Color SuccessBackground = "#0F821E".HexToColor();
        public static Color WarningForeground = Colors.Black;
        public static Color WarningBackground = Colors.Yellow;
        public static Color ErrorForeground = Colors.White;
        public static Color ErrorBackground = Colors.Red;

        public const int Timeout = 2000;
        public static int FontSize = 15;
        public static int CornerRadius = 15;
        public static int YOffset => 150;
        public static int XOffset = 100;
        public static bool IsBusy = false;

        public static void Info(this Page page, string msg = null, NotificationPosition pos = NotificationPosition.BottomCenter) => page.ToggleNotification(NotificationType.Info, msg, pos);
        public static void Success(this Page page, string msg = null, NotificationPosition pos = NotificationPosition.BottomCenter) => page.ToggleNotification(NotificationType.Success, msg, pos);
        public static void Warning(this Page page, string msg = null, NotificationPosition pos = NotificationPosition.BottomCenter) => page.ToggleNotification(NotificationType.Warning, msg, pos);
        public static void Error(this Page page, string msg = null, NotificationPosition pos = NotificationPosition.BottomCenter) => page.ToggleNotification(NotificationType.Error, msg, pos);
        public static void ToggleNotification(this Page page, NotificationType type = NotificationType.Info, string msg = null, NotificationPosition pos = NotificationPosition.BottomCenter)
        {
            if (!IsBusy)
            {
                if (msg.IsValid())
                {
                    page.ProcessNotification(true, type, msg, pos);
                }
            }
        }

        private static void ProcessNotification(this Page pg, bool isActive, NotificationType nt = NotificationType.Info, string msg = null, NotificationPosition pos = NotificationPosition.BottomCenter)
        {
            Grid grid = pg.FindName(gridKey) == null ? pg.GetFullGrid(pos).GetStoryboards(pg, pos).grid : (Grid)pg.FindName(gridKey);
            var mainGrid = (Panel)pg.Content;
            if (mainGrid.FindName(gridKey) == null)
            {
                mainGrid.Children.Add(grid);
            }
            var stack = (StackPanel)grid.FindName(stackKey);

            var icon = (FontIcon)grid.FindName(iconKey);
            var text = (TextBlock)grid.FindName(textKey);

            if (!isActive)
            {
                var exit = grid.GetStoryboards(pg, pos).exit;
                exit.Completed += ExitAnimation_Completed;
                exit.Begin();
                void ExitAnimation_Completed(object sender, object e)
                {
                    IsBusy = false;
                }
            }
            else
            {
                if (!IsBusy)
                {
                    text.Text = msg;
                    if (nt == NotificationType.Info)
                    {
                        icon.Glyph = InfoGlyph;
                        icon.Foreground = InfoForeground.ToBrush();
                        text.Foreground = InfoForeground.ToBrush();
                        stack.Background = InfoBackground.ToBrush();
                    }
                    if (nt == NotificationType.Success)
                    {
                        icon.Glyph = SuccessGlyph;
                        icon.Foreground = SuccessForeground.ToBrush();
                        text.Foreground = SuccessForeground.ToBrush();
                        stack.Background = SuccessBackground.ToBrush();
                    }
                    else if (nt == NotificationType.Warning)
                    {
                        icon.Glyph = WarningGlyph;
                        icon.Foreground = WarningForeground.ToBrush();
                        text.Foreground = WarningForeground.ToBrush();
                        stack.Background = WarningBackground.ToBrush();
                    }
                    else if (nt == NotificationType.Error)
                    {
                        icon.Glyph = ErrorGlyph;
                        icon.Foreground = ErrorForeground.ToBrush();
                        text.Foreground = ErrorForeground.ToBrush();
                        stack.Background = ErrorBackground.ToBrush();
                    }

                    var entry = grid.GetStoryboards(pg, pos).entry;
                    entry.Completed += EntryAnimation_Completed;
                    entry.Begin();
                    IsBusy = true;
                    async void EntryAnimation_Completed(object sender, object e)
                    {
                        await Task.Delay(Timeout).ContinueWith((t) =>
                        {
                            pg.LoadOnUI(() =>
                            {
                                pg.ProcessNotification(false, nt, msg);
                            });
                        });
                    }
                }
            }
        }
        private static Grid GetFullGrid(this Page pg, NotificationPosition pos = NotificationPosition.BottomCenter)
        {
            var grid = pg.GetGrid(pos);
            grid.Children.Add(GetStackPanel());
            return grid;
        }
        private static (Grid grid, Storyboard entry, Storyboard exit) GetStoryboards(this Grid grid, Page pg, NotificationPosition pos = NotificationPosition.BottomCenter)
        {
            grid.RenderTransform = new TranslateTransform();
            Storyboard entry = new Storyboard();
            DoubleAnimation d1 = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(400)),
                EasingFunction = new CircleEase()
                {
                    EasingMode = EasingMode.EaseOut
                }
            }.ModifyAnimationBasedOnPosition(pg, AnimationType.Entry, pos);
            Storyboard exit = new Storyboard();
            DoubleAnimation d2 = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(400)),
                EasingFunction = new CircleEase()
                {
                    EasingMode = EasingMode.EaseIn
                }
            }.ModifyAnimationBasedOnPosition(pg, AnimationType.Exit, pos);
            Storyboard.SetTarget(d1, grid.RenderTransform);
            Storyboard.SetTargetProperty(d1, pos.GetTargetPropFromPosition());
            Storyboard.SetTarget(d2, grid.RenderTransform);
            Storyboard.SetTargetProperty(d2, pos.GetTargetPropFromPosition());
            entry.Children.Add(d1);
            exit.Children.Add(d2);

            return (grid, entry, exit);
        }
        private static string GetTargetPropFromPosition(this NotificationPosition pos)
        {
            switch (pos)
            {
                case NotificationPosition.TopRight:
                    return "X";
                case NotificationPosition.CenterRight:
                    return "X";
                case NotificationPosition.BottomRight:
                    return "X";
                case NotificationPosition.BottomCenter:
                    return "Y";
                case NotificationPosition.BottomLeft:
                    return "Y";
                case NotificationPosition.CenterLeft:
                    return "X";
                case NotificationPosition.TopLeft:
                    return "X";
                default:
                    return "Y";
            }
        }
        private static DoubleAnimation ModifyAnimationBasedOnPosition(this DoubleAnimation da, Page pg, AnimationType type, NotificationPosition pos = NotificationPosition.BottomCenter)
        {
            if (pos == NotificationPosition.BottomCenter)
            {
                if (type == AnimationType.Entry)
                {
                    da.From = 0;
                    da.To = YOffset.Negate();
                }
                else
                {
                    da.From = YOffset.Negate();
                    da.To = 0;
                }
            }
            else if (pos == NotificationPosition.TopRight)
            {
                if (type == AnimationType.Entry)
                {
                    da.From = XOffset;
                    da.To = XOffset;
                }
                else
                {
                    da.From = 0;
                    da.To = XOffset;
                }
            }
            return da;
        }
        private static Grid GetGrid(this Page pg, NotificationPosition pos = NotificationPosition.BottomCenter)
        {
            return new Grid
            {
                Name = gridKey,
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 0),
                Visibility = Visibility.Visible
            }.ModifyGridByPosition(pg, pos);
        }
        private static Grid ModifyGridByPosition(this Grid grid, Page pg, NotificationPosition pos)
        {
            switch (pos)
            {
                case NotificationPosition.TopRight:
                    grid.VerticalAlignment = VerticalAlignment.Top;
                    grid.HorizontalAlignment = HorizontalAlignment.Right;
                    grid.Margin = new Thickness(XOffset, 10, 0, 0);
                    break;
                case NotificationPosition.CenterRight:
                    grid.VerticalAlignment = VerticalAlignment.Center;
                    grid.HorizontalAlignment = HorizontalAlignment.Right;
                    break;
                case NotificationPosition.BottomRight:
                    grid.VerticalAlignment = VerticalAlignment.Bottom;
                    grid.HorizontalAlignment = HorizontalAlignment.Right;
                    break;
                case NotificationPosition.BottomCenter:
                    grid.VerticalAlignment = VerticalAlignment.Bottom;
                    grid.HorizontalAlignment = HorizontalAlignment.Center;
                    grid.Margin = new Thickness(0, 0, 0, -100);
                    break;
                case NotificationPosition.BottomLeft:
                    grid.VerticalAlignment = VerticalAlignment.Bottom;
                    grid.HorizontalAlignment = HorizontalAlignment.Left;
                    break;
                case NotificationPosition.CenterLeft:
                    grid.VerticalAlignment = VerticalAlignment.Center;
                    grid.HorizontalAlignment = HorizontalAlignment.Left;
                    break;
                case NotificationPosition.TopLeft:
                    grid.VerticalAlignment = VerticalAlignment.Top;
                    grid.HorizontalAlignment = HorizontalAlignment.Left;
                    break;
                default:
                    break;
            }
            return grid;
        }
        private static StackPanel GetStackPanel()
        {
            var stack = new StackPanel
            {
                Name = stackKey,
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
                CornerRadius = new CornerRadius(CornerRadius),
                Padding = new Thickness(25, 10, 25, 10)
            };
            FontIcon icon = new FontIcon
            {
                Name = iconKey,
                FontSize = FontSize,
                FontWeight = new FontWeight() { Weight = 700 },
                Margin = new Thickness(0, 0, 10, 0)
            };
            TextBlock tb = new TextBlock
            {
                Name = textKey,
                FontSize = FontSize
            };
            stack.Children.Add(icon);
            stack.Children.Add(tb);
            return stack;
        }
    }
    public enum NotificationType
    {
        Info,
        Success,
        Warning,
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
}