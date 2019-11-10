using System;
using System.Text.RegularExpressions;
using Common;
using Windows.Data.Html;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace UWP.Common.Helpers.Controls
{
    public sealed class HtmlTextBlock : Control
    {
        static RichTextBlock _canvas;
        public HtmlTextBlock()
        {
            this.DefaultStyleKey = typeof(HtmlTextBlock);
        }
        
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _canvas = (RichTextBlock)GetTemplateChild("RichTextBlockCanvas");
            if (_canvas == null) throw new NullReferenceException();

            this.DrawHtml();
        }

        public string Html
        {
            get { return (string)GetValue(HtmlProperty); }
            set { SetValue(HtmlProperty, value); }
        }
        public double LineHeight
        {
            get { return (double)GetValue(LineHeightProperty); }
            set { SetValue(LineHeightProperty, value); }
        }


        public TextWrapping Wrapping
        {
            get { return (TextWrapping)GetValue(WrappingProperty); }
            set { SetValue(WrappingProperty, value); }
        }

        public static readonly DependencyProperty WrappingProperty =
            DependencyProperty.Register(nameof(Wrapping), typeof(TextWrapping), typeof(HtmlTextBlock), new PropertyMetadata(TextWrapping.NoWrap));



        public static readonly DependencyProperty LineHeightProperty =
            DependencyProperty.Register(nameof(LineHeight), typeof(double), typeof(HtmlTextBlock), new PropertyMetadata(15));



        public static readonly DependencyProperty HtmlProperty =
            DependencyProperty.Register(nameof(Html), typeof(string), typeof(HtmlTextBlock), new PropertyMetadata(string.Empty,new PropertyChangedCallback(OnHtmlChanged)));

        private static void OnHtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HtmlTextBlock iwlc = d as HtmlTextBlock; //null checks omitted
            if(e.NewValue != null || e.OldValue != null)
            {
                if (_canvas == null) return;
                _canvas.Blocks.Clear();
                Paragraph p = new Paragraph();
                Span span;
                if(e.NewValue != null)
                {
                    string html = e.NewValue.ToString();
                    if (Regex.IsMatch(html, Utils.UrlRegex, RegexOptions.IgnoreCase))
                    {
                        span = HtmlExts.FormatText(html);
                    }
                    else
                    {
                        span = new Span();
                        span.Inlines.Add(new Run() { Text = HtmlUtilities.ConvertToText(html) });
                    }
                    p.Inlines.Add(span);
                }
                _canvas.Blocks.Add(p);
            }
        }

        public void DrawHtml()
        {
            if (_canvas == null) return;
            _canvas.Blocks.Clear();

            Paragraph p = new Paragraph
            {
                LineHeight = this.LineHeight,
                FontSize = this.FontSize
            };
            Span span;
            if (Regex.IsMatch(Html, Utils.UrlRegex, RegexOptions.IgnoreCase))
            {
                span = HtmlExts.FormatText(Html);
            }
            else
            {
                span = new Span();
                span.Inlines.Add(new Run() { Text = HtmlUtilities.ConvertToText(Html) });
            }
            p.Inlines.Add(span);
            _canvas.Blocks.Add(p);
        }
    }
    public static class HtmlExts
    {
        public static Run NewLine => new Run() { Text = "\n" };
        public static Span FormatText(this string text)
        {
            string[] lines = text.Split('\n', '\r');
            Span sp = new Span
            {
                FontSize = 16,
                
            };
            foreach (string line in lines)
            {
                Span span = new Span();
                if (!line.IsValid())
                {
                    sp.Inlines.Add(NewLine);
                }
                else
                {
                    if (!line.ContainsAnyUrl())
                    {
                        Run run = new Run
                        {
                            Text = line + "\r",
                        };
                        if (line.IsUpper())
                        {
                            run.FontWeight = new FontWeight() { Weight = 700 };
                        }

                        span.Inlines.Add(run);
                    }
                    else
                    {
                        string[] words = line.GetFullWords();
                        for (int i = 0; i < words.Length; i++)
                        {
                            string word = words[i];
                            if (word.IsValid())
                            {
                                if (word.IsValidUrl())
                                {
                                    string url = word.SanifUrl();
                                    url = url.StartsWith("www") ? $"http://{url}" : url;
                                    if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
                                    {
                                        try
                                        {
                                            //InlineUIContainer cont = new InlineUIContainer();
                                            //var linkBtn = new HyperlinkButton()
                                            //{
                                            //    Tag = word.SanifUrl(),
                                            //    Padding = new Thickness(0, 0, 0, -5),
                                            //    VerticalAlignment = VerticalAlignment.Bottom,
                                            //    VerticalContentAlignment = VerticalAlignment.Bottom,
                                            //};
                                            var link = new Hyperlink()
                                            {
                                                AccessKey = url,
                                                TextDecorations = TextDecorations.None,
                                                NavigateUri = new Uri(url)
                                            };
                                            link.Inlines.Add(new Run() { Text = word + " " });
                                            
                                            //linkBtn.Content = link;
                                            //cont.Child = linkBtn;
                                            span.Inlines.Add(link);
                                        }
                                        catch (Exception)
                                        { }
                                    }
                                    else
                                    {
                                        span.Inlines.Add(new Run()
                                        {
                                            Text = word + " ",
                                        });
                                    }
                                }
                                else
                                {
                                    span.Inlines.Add(new Run()
                                    {
                                        Text = word + " ",
                                    });
                                }
                            }
                            if (i == words.Length - 1)
                            {
                                span.Inlines.Add(NewLine);
                            }
                        }
                    }
                }
                sp.Inlines.Add(span);
            }
            return sp;
        }
    }
}