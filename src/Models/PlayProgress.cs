using System;

namespace UWP.Common.Helpers.Models
{
    public struct PlayProgress
    {
        private TimeSpan TotalDuration { get; set; }

        public PlayProgress(TimeSpan position, TimeSpan total)
        {
            CurrentPosition = position;
            TotalDuration = total;
        }
        public TimeSpan CurrentPosition { get; set; }
        /// <summary>
        /// Position of the currently playing media in seconds.
        /// </summary>
        public double ElapsedSeconds => CurrentPosition.TotalSeconds;
        /// <summary>
        /// Full/Total Duration of the currently playing media in seconds.
        /// </summary>
        public double TotalSeconds => TotalDuration.TotalSeconds;
        /// <summary>
        /// Seconds remaining for currently playing media to reach the end.
        /// </summary>
        public double RemainingSeconds => TotalSeconds - ElapsedSeconds;
        /// <summary>
        /// Percentage duration progress played out of the total duration
        /// </summary>
        public double PercentageProgress => (ElapsedSeconds / TotalSeconds) * 100;

        public override bool Equals(object obj)
        {
            if (obj is PlayProgress p)
            {
                return GetHashCode() == p.GetHashCode();
            }
            return false;
        }

        public override int GetHashCode() => Convert.ToInt32((ElapsedSeconds + TotalSeconds)).GetHashCode();
        public static bool operator ==(PlayProgress left, PlayProgress right) => left.Equals(right);
        public static bool operator !=(PlayProgress left, PlayProgress right) => !(left == right);
    }
}