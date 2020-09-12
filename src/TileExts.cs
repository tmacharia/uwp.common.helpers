using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Controls;

namespace UWP.Common.Helpers
{
    public static class TileExts
    {
        public const int TileExpiryInDays = 2;

        public static void UpdateBadgeGlyph(this Page page, BadgeGlyph glyph) => UpdateBadgeGlyph(glyph);
        public static void UpdateBadgeGlyph(BadgeGlyph glyph)
        {
            // Create the badge updater for the application
            BadgeUpdater updater = BadgeUpdateManager.CreateBadgeUpdaterForApplication();

            if (glyph == BadgeGlyph.None)
            {
                updater.Clear();
            }
            else
            {
                // Get the blank badge XML payload for a badge number
                XmlDocument badgeXml = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeGlyph);

                // Set the value of the badge in the xml to our number
                XmlElement badgeElem = badgeXml.SelectSingleNode("/badge") as XmlElement;
                badgeElem.SetAttribute("value", glyph.DescriptionAttr());

                // Create the badge notification
                BadgeNotification badge = new BadgeNotification(badgeXml)
                {
                    ExpirationTime = DateTimeOffset.UtcNow.AddMinutes(10)
                };
                // And update the badge
                updater.Update(badge);
            }
        }
        public static void CreateImageTileNotification(this Page page, string mediumImageUrl, string largeImageUrl) 
            => UpdateTileNotificationContent(ImageTileContent(mediumImageUrl, largeImageUrl));
        public static void CreateImageTileNotification(this Page page, string imageUrl)
            => UpdateTileNotificationContent(ImageTileContent(imageUrl));
        public static void CreateBasicTileNotification(this Page page, string title, string subTitle, string caption)
            => UpdateTileNotificationContent(BasicTileContent(title, subTitle, caption));
        public static void CreateTileNotification(Uri imageUrl, string text)
            => UpdateTileNotificationContent(ImageTileContent(imageUrl, text));
        public static void CreateTileNotification(Uri imageUrl, string text, Uri wideImageUrl = default, string display = default, string desc = default)
            => UpdateTileNotificationContent(ImageTileContent(imageUrl, text, wideImageUrl, display, desc));

        private static void UpdateTileNotificationContent(TileContent tile)
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(tile.GetContent());
                var updater = TileUpdateManager.CreateTileUpdaterForApplication();
                if (updater != null)
                {
                    updater.EnableNotificationQueue(true);
                    updater.Update(new TileNotification(xml)
                    {
                        Tag = tile.Visual.ContentId.GetHashCode().ToString(),
                        ExpirationTime = DateTimeOffset.Now.AddDays(TileExpiryInDays)
                    });
                }
            }
            catch (Exception)
            { }
        }
        public static void ClearTiles()
        {
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            if (updater != null)
            {
                updater.Clear();
            }
        }
        private static TileContent ImageTileContent(Uri imageUrl, string text, Uri wideImageUrl=default, string display=default, string desc = default)
        {
            return new TileContent()
            {
                Visual = new TileVisual()
                {
                    Branding = TileBranding.Auto,
                    DisplayName = display.IsValid() ? display : string.Empty,
                    TileMedium = GenerateImageTileBinding(imageUrl, text, false, desc),
                    TileLarge = GenerateImageTileBinding(imageUrl, text, true, desc),
                    TileWide = GenerateImageTileBinding(wideImageUrl ?? imageUrl, text, true, desc),
                    ContentId = imageUrl != null ? imageUrl.OriginalString : text.GetHashCode().ToString()
                }
            };
        }
        private static TileBinding GenerateImageTileBinding(Uri imageUrl, string text = default, bool isLarge = false,string desc=default)
        {
            var ctx = new TileBindingContentAdaptive()
            {
                PeekImage = new TilePeekImage() { Source = imageUrl != null ? imageUrl.OriginalString : "" },
                TextStacking = (isLarge && desc.IsValid()) ? TileTextStacking.Top : TileTextStacking.Bottom
            };
            AdaptiveTextStyle ats = isLarge ? AdaptiveTextStyle.Caption : AdaptiveTextStyle.CaptionSubtle;
            ctx.Children.Add(new AdaptiveText() { Text = text.Trim(), HintMaxLines = 2, HintStyle = ats });
            if (isLarge && desc.IsValid())
            {
                ctx.Children.Add(new AdaptiveText() { Text = desc.Trim().FromHtmlToText(), HintMaxLines = 5, HintWrap = true, HintStyle = AdaptiveTextStyle.CaptionSubtle });
            }
            return new TileBinding() { Content = ctx };
        }

        private static TileContent ImageTileContent(string mediumImageUrl, string largeImageUrl)
        {
            return new TileContent()
            {
                Visual = new TileVisual()
                {
                    Branding = TileBranding.None,
                    TileMedium = GenerateImageTileBinding(mediumImageUrl),
                    TileLarge = GenerateImageTileBinding(largeImageUrl),
                    TileWide = GenerateImageTileBinding(largeImageUrl)
                }
            };
        }
        private static TileContent ImageTileContent(string imageUrl)
        {
            return new TileContent()
            {
                Visual = new TileVisual()
                {
                    Branding = TileBranding.None,
                    TileSmall = GenerateImageTileBinding(imageUrl),
                    TileMedium = GenerateImageTileBinding(imageUrl),
                    TileLarge = GenerateImageTileBinding(imageUrl),
                    TileWide = GenerateImageTileBinding(imageUrl)
                }
            };
        }
        private static TileContent BasicTileContent(string title, string subTitle, string caption)
        {
            return new TileContent()
            {
                Visual = new TileVisual()
                {
                    Branding = TileBranding.Logo,
                    TileWide = GenerateBasicTileBinding(title, subTitle, caption),
                    TileMedium = GenerateBasicTileBinding(title, subTitle, caption),
                    TileLarge = GenerateBasicTileBinding(title, subTitle, caption)
                }
            };
        }
        private static TileBinding GenerateImageTileBinding(params string[] urls)
        {
            if (urls != null && urls.Length > 0)
            {
                var tile = new TileBinding();
                var content = new TileBindingContentPhotos();
                if (urls != null && urls.Length > 0)
                {
                    urls.ForEach(x =>
                    {
                        content.Images.Add(new TileBasicImage() { Source = x });
                    });
                }
                tile.Content = content;
                return tile;
            }
            return null;
        }
        private static TileBinding GenerateImageTileBinding(string url,string alternate=default)
        {
            return new TileBinding()
            {
                Content = new TileBindingContentPhotos()
                {
                    Images =
                    {
                        new TileBasicImage() { Source = url,AlternateText = alternate.IsValid()?alternate:string.Empty }
                    }
                }
            };
        }
        private static TileBinding GenerateImageTileBinding(List<KeyValuePair<string, string>> pairs)
        {
            if (pairs != null && pairs.Count > 0)
            {
                return new TileBinding
                {
                    Branding = TileBranding.NameAndLogo,
                    Content = TileContentBuilder.CreatePhotosTileContent(pairs.Select(x => new TileBasicImage() { Source = x.Key, AlternateText = x.Value }))
                };
            }
            return null;
        }
        private static TileBinding GenerateBasicTileBinding(string title, string subTitle, string caption)
        {
            return new TileBinding()
            {
                Content = new TileBindingContentAdaptive()
                {
                    Children =
                    {
                        new AdaptiveText()
                        {
                            Text = title,
                            HintStyle = AdaptiveTextStyle.Body
                        },
                        new AdaptiveText()
                        {
                            Text = subTitle,
                            HintStyle = AdaptiveTextStyle.Caption
                        },
                        new AdaptiveText()
                        {
                            Text = caption,
                            HintStyle = AdaptiveTextStyle.CaptionSubtle
                        }
                    }
                }
            };
        }
    }
}