using System;
using Common;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Controls;

namespace UWP.Common.Helpers
{
    public static class TileExts
    {
        private const int TileExpiryInDays = 2;

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
        public static void CreateBasicTileNotification(this Page page, string title, string subTitle, string caption)
            => UpdateTileNotificationContent(BasicTileContent(title, subTitle, caption));

        private static void UpdateTileNotificationContent(TileContent tile)
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(tile.GetContent());
                var notification = new TileNotification(xml)
                {
                    ExpirationTime = DateTimeOffset.UtcNow.AddDays(TileExpiryInDays)
                };
                var updater = TileUpdateManager.CreateTileUpdaterForApplication();
                updater.Clear();
                if (updater != null)
                {
                    updater.Update(notification);
                }
            }
            catch (Exception)
            { }
        }
        private static TileContent ImageTileContent(string[] mediumUrls, string[] largeUrls)
        {
            return new TileContent()
            {
                Visual = new TileVisual()
                {
                    Branding = TileBranding.Auto,
                    TileMedium = GenerateImageTileBinding(mediumUrls),
                    TileLarge = GenerateImageTileBinding(largeUrls),
                    TileWide = GenerateImageTileBinding(largeUrls)
                }
            };
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
        private static TileBinding GenerateImageTileBinding(string url)
        {
            return new TileBinding()
            {
                Content = new TileBindingContentPhotos()
                {
                    Images =
                    {
                        new TileBasicImage() { Source = url }
                    }
                }
            };
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