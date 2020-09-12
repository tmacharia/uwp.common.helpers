using System;
using System.Collections.Generic;
using Common;
using Windows.Networking.Connectivity;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.ApplicationModel.DataTransfer;

namespace UWP.Common.Helpers
{
    public static class UwpExts
    {
        public static bool IsConnected => HasInternetConnection();
        public static bool HasInternet(this Page page) => HasInternetConnection();
        public static bool HasInternetConnection()
        {
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            return connectionProfile != null && connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
        }
        public static bool IsCtrlOrShiftKeyPressed(this Page pg) => pg.IsCtrlKeyPressed() || pg.IsShiftKeyPressed();
        public static bool IsCtrlKeyPressed(this Page pg)
        {
            var CtrlState = CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Control);
            return (CtrlState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
        }
        public static bool IsShiftKeyPressed(this Page pg)
        {
            var ShiftState = CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Shift);
            return (ShiftState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
        }
        
        public static void Enable(this Control control) => control.IsEnabled = true;
        public static void Disable(this Control control) => control.IsEnabled = false;
        public static void ClearText(this TextBlock txt) => txt.Text = string.Empty;
        public static void ClearText(this Run run) => run.Text = string.Empty;
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
        public static void CopyToClipboard(this Page pg, string txt) => CopyToClipboard(txt);
        public static void CopyToClipboard(string txt)
        {
            if (txt.IsValid())
            {
                DataPackage dp = new DataPackage();
                dp.SetText(txt);
                Clipboard.SetContent(dp);
            }
        }
        public static Dictionary<string, string> BaseEventProps(this Page page) => BaseEventProps();
        public static Dictionary<string, string> BaseEventProps()
        {
            return new Dictionary<string, string>
            {
                { "PC", Environment.MachineName+$", {ZoneExts.CurrentGeo}" }
            };
        }
    }
}