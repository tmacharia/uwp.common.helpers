using System;
using System.Threading.Tasks;
using UWP.Common.Helpers;
using Windows.Foundation;
using Windows.UI.Core;

namespace Windows.UI.Xaml.Controls
{
    public static class XamlControlsExtensions
    {
        public static TElem GetElem<TElem>(this Page pg, string name)
        {
            return (TElem)pg.FindName(name);
        }
        public static TElem GetElem<TElem>(this Panel panel, string name)
        {
            return (TElem)panel.FindName(name);
        }
        public static async void LoadOnUI(this Page pg, Action action)
        {
            var a = pg.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
            while (a.Status == AsyncStatus.Started)
            {
                await Task.Delay(50);
            }
        }
        public static void ForwardAnimateFrom(this Page pg, UIElement elem)
            => Animations.PrepAnimation(Animations.FWD_CONN_ANIM_KEY, elem);
        public static void ForwardAnimateTo(this Page pg, UIElement elem)
            => Animations.PlayConnAnimation(Animations.FWD_CONN_ANIM_KEY, elem);
        public static void BackAnimateFrom(this Page pg, UIElement elem)
            => Animations.PrepAnimation(Animations.BACK_CONN_ANIM_KEY, elem);
        public static void BackAnimateTo(this Page pg, UIElement elem)
            => Animations.PlayConnAnimation(Animations.BACK_CONN_ANIM_KEY, elem);
    }
}