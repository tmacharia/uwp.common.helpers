using System;
using System.Collections.Generic;
using Common;
using Windows.Networking.Connectivity;
using Windows;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel;

namespace UWP.Common.Helpers
{
    public static class UwpExts
    {
        public static Color AccentColor = (Color)Application.Current.Resources["SystemAccentColor"];

        
        public static bool IsConnected => HasInternetConnection();
        public static bool HasInternet(this Page page) => HasInternetConnection();
        public static bool HasInternetConnection()
        {
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            return (connectionProfile != null && connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);
        }
        public static bool IsVisible(this FrameworkElement elem) => elem.Visibility == Visibility.Visible ? true : false;
        public static void Hide(this FrameworkElement elem) => elem.Visibility = Visibility.Collapsed;
        public static void UnHide(this FrameworkElement elem) => elem.Visibility = Visibility.Visible;
        public static void Enable(this Control control) => control.IsEnabled = true;
        public static void Disable(this Control control) => control.IsEnabled = false;
        public static void ClearText(this TextBlock txt) => txt.Text = string.Empty;
        public static void ClearText(this Run run) => run.Text = string.Empty;
        public static Brush ToBrush(this Color col) => new SolidColorBrush(col);
        public static async void LoadOnUI(this Page page, Action action)
        {
            var a = page.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
            while (a.Status == AsyncStatus.Started)
            {
                await Task.Delay(100);
            }
        }
        public static async void ShowMessageDialog(this Page page, string title = null, string message = null)
        {
            var a = ShowMessageDialog(title, message);
            while (a.Status == AsyncStatus.Started)
            {
                await Task.Delay(100);
            }
        }
        public static IAsyncOperation<IUICommand> ShowMessageDialog(string title = null, string message = null)
        {
            title = title.IsValid() ? title : "Message Log!";
            var dialog = new MessageDialog(message, title);
            return dialog.ShowAsync();
        }
        public static async void YesOrNoMessageDialog(this Page page, string message, string title = null, string yesBtnTxt = "Yes", Action yesAction = null, string noBtnText = "No", Action noAction = null)
        {
            title = title.IsValid() ? title : "Attention Required";
            message = message.IsValid() ? message : "";
            yesBtnTxt = yesBtnTxt.IsValid() ? yesBtnTxt : "Yes";
            noBtnText = noBtnText.IsValid() ? noBtnText : "No";

            var yesCommand = new UICommand(yesBtnTxt);
            var noCommand = new UICommand(noBtnText);

            var dialog = new MessageDialog(message, title)
            {
                Options = MessageDialogOptions.None
            };
            dialog.Commands.Add(yesCommand);
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 0;
            if (noCommand != null)
            {
                dialog.Commands.Add(noCommand);
                dialog.CancelCommandIndex = (uint)dialog.Commands.Count - 1;
            }
            var cmd = dialog.ShowAsync();
            while (cmd.Status == AsyncStatus.Started)
            {
                await Task.Delay(100);
            }
            var res = cmd.GetResults();
            if (res == yesCommand)
            {
                yesAction();
            }
            else if (res == noCommand)
            {
                if (noAction != null)
                {
                    noAction();
                }
                else
                {
                    return;
                }
            }
        }
        public static Dictionary<string, string> BaseEventProps(this Page page) => BaseEventProps();
        public static Dictionary<string, string> BaseEventProps()
        {
            return new Dictionary<string, string>
            {
                { "PC", Environment.MachineName+$", {Utils.CurrentRegion}" }
            };
        }
        //public static DataRequest LoadDefaults(this DataRequest req, string webUrl, string appWebUrl)
        //{
        //    var cur = Package.Current;
        //    req.Data.Properties.ApplicationListingUri = new Uri($"ms-windows-store://pdp/?PFN={cur.Id.FamilyName}");
        //    req.Data.Properties.ApplicationName = cur.DisplayName;
        //    req.Data.Properties.PackageFamilyName = cur.Id.FamilyName;
        //    req.Data.Properties.ContentSourceWebLink = webUrl.IsValid() ? new Uri(webUrl) : new Uri(appWebUrl);

        //    return req;
        //}
        public static bool IsCtrlKeyPressed(this Page page)
        {
            var ctrlState = CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Control);
            return (ctrlState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
        }
    }
}