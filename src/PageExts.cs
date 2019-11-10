using Common;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP.Common.Helpers
{
    public static class PageExts
    {
        private const string TopPrgStackKey = "topProgressStack";
        private const string TopPrgBarKey = "topProgressBar";
        private const string TopPrgTxtKey = "topProgressTxt";

        private const string PrgRingStackKey = "progressRingStack";
        private const string PrgRingTxtKey = "progressRingTxt";
        private const string PrgRingKey = "progressRing";

        public static void ToggleTopLoader(this Page page, bool isActive = true, string msg = null, Color? progressColor=null)
        {
            page.LoadOnUI(() =>
            {
                StackPanel progressStack = null;
                ProgressBar progressBar = null;
                TextBlock progressTxt = null;

                if(page.FindName(TopPrgStackKey) != null)
                {
                    progressStack = (StackPanel)page.FindName(TopPrgStackKey);
                    progressBar = (ProgressBar)progressStack.FindName(TopPrgBarKey);
                    progressTxt = (TextBlock)progressStack.FindName(TopPrgTxtKey);
                }
                else
                {
                    StackPanel panel = new StackPanel
                    {
                        Name = TopPrgStackKey,
                        Margin = new Thickness(0, 5, 0, 0),
                        Orientation = Orientation.Vertical,
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Stretch
                    };
                    TextBlock text = new TextBlock
                    {
                        Name = TopPrgTxtKey,
                        HorizontalAlignment = HorizontalAlignment.Left
                    };
                    ProgressBar bar = new ProgressBar
                    {
                        Name = TopPrgBarKey,
                        FontSize = 30,
                        Minimum = 0,
                        Maximum = 100,
                        IsIndeterminate = true,
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Foreground = progressColor.HasValue ? progressColor.Value.ToBrush() : UwpExts.AccentColor.ToBrush()
                    };
                    panel.Children.Add(text);
                    panel.Children.Add(bar);
                    panel.UnHide();

                    var grid = (Grid)page.Content;
                    grid.Children.Add(panel);
                    progressStack = panel;
                    progressBar = bar;
                    progressTxt = text;
                }

                if (isActive)
                {
                    progressStack.UnHide();
                    progressBar.UnHide();
                    if (msg.IsValid())
                    {
                        progressTxt.UnHide();
                        progressTxt.Text = msg;
                    }
                    else
                    {
                        progressTxt.ClearText();
                        progressTxt.Hide();
                    }
                }
                else
                {
                    progressStack.Hide();
                    progressTxt.ClearText();
                    progressBar.IsIndeterminate = true;
                    progressBar.Hide();
                }
            });
        }

        public static void ToggleCenterProgress(this Page page, bool isActive = true, string text = null, Color? progressColor = null)
        {
            page.LoadOnUI(() =>
            {
                StackPanel progressStack = null;
                TextBlock progressTxt = null;

                if (page.FindName(PrgRingStackKey) != null)
                {
                    progressStack = (StackPanel)page.FindName(PrgRingStackKey);
                    progressTxt = (TextBlock)progressStack.FindName(PrgRingTxtKey);
                }
                else
                {
                    StackPanel panel = new StackPanel
                    {
                        Name = PrgRingStackKey,
                        Orientation = Orientation.Vertical,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };
                    ProgressRing ring = new ProgressRing
                    {
                        Name = PrgRingKey,
                        Height = 50,
                        Width = 50,
                        IsActive = true,
                        Margin = new Thickness(0,0,0,20),
                        Foreground = progressColor.HasValue ? progressColor.Value.ToBrush() : UwpExts.AccentColor.ToBrush()
                    };
                    TextBlock txt = new TextBlock
                    {
                        Name = PrgRingTxtKey,
                        HorizontalAlignment = HorizontalAlignment.Left
                    };
                    panel.Children.Add(ring);
                    panel.Children.Add(txt);
                    panel.Hide();
                    var grid = (Grid)page.Content;
                    grid.Children.Add(panel);
                    progressStack = panel;
                    progressTxt = txt;
                }
                if (isActive)
                {
                    progressStack.UnHide();
                    progressTxt.Text = text.IsValid() ? text : "";
                }
                else
                {
                    progressStack.Hide();
                    progressTxt.ClearText();
                }
            });
        }
    }
}