using Windows.Media.Playback;

namespace UWP.Common.Helpers.Models
{
    public static class MediaPlayerExts
    {
        public static double GetSecondsOutOf(this MediaPlayer md, double value, double multiplyBy = 100, double total = 0)
            => md.GetSeconds(value * multiplyBy, total);
        public static double GetSeconds(this MediaPlayer md, double percentage, double total = 0)
        {
            double _total = total > 0 ? total : md.GetTotalSeconds();
            double ans = (percentage * _total) / 100;
            return ans > _total ? _total : ans;
        }
        public static double GetTotalSeconds(this MediaPlayer md) => md.PlaybackSession.NaturalDuration.TotalSeconds;
    }
}