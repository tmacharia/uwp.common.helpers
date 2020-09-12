using System;
using System.Numerics;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace UWP.Common.Helpers
{
    public static class Animations
    {
        public const double FadeDurationSecs = 0.8;
        public const string FWD_CONN_ANIM_KEY = "ForwardConnectedAnimation";
        public const string BACK_CONN_ANIM_KEY = "BackConnectedAnimation";
        public static SpringVector3NaturalMotionAnimation _prdAnimation;
        private static Compositor _compositor;


        #region Navigation Connected Animations
        public static void PrepAnimation(string name, UIElement elem) 
            => ConnectedAnimationService.GetForCurrentView().PrepareToAnimate(name, elem);
        public static void PlayConnAnimation(string name, UIElement elem)
        {
            var anim = ConnectedAnimationService.GetForCurrentView().GetAnimation(name);
            if (anim != null)
            {
                anim.TryStart(elem);
            }
        }
        #endregion

        public static void CreateOrUpdateSpringAnimation(float finalValue)
        {
            if (_prdAnimation == null)
            {
                if (_compositor == null)
                    _compositor = Window.Current.Compositor;
                _prdAnimation = _compositor.CreateSpringVector3Animation();
                _prdAnimation.Target = "Scale";
            }
            _prdAnimation.FinalValue = new Vector3(1.0f, finalValue, finalValue);
        }

        public static void FadeInOrOut(this DependencyObject dp, bool isFadeIn, double secs)
        {
            Storyboard sb = new Storyboard();
            Timeline fd = GetFadeTimeline(isFadeIn, secs);
            Storyboard.SetTarget(fd, dp);
            sb.Children.Add(fd);
            sb.Begin();
        }
        private static Timeline GetFadeTimeline(bool isFadeIn, double secs)
        {
            secs = secs > 0 ? secs : FadeDurationSecs;
            if (isFadeIn)
            {
                return new FadeInThemeAnimation
                {
                    Duration = new Duration(TimeSpan.FromSeconds(secs))
                };
            }
            else
            {
                return new FadeOutThemeAnimation
                {
                    Duration = new Duration(TimeSpan.FromSeconds(secs))
                };
            }
        }
    }
}