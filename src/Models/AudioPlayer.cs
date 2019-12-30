using System;
using System.Collections.Generic;
using System.Text;
using Common;
using Common.Models;
using Windows.Media.Playback;
using Windows.UI.Xaml;

namespace UWP.Common.Helpers.Models
{
    public delegate void PlayingEventHandler(PlayProgress progress);
    public delegate void PlayerStateChangeEventHandler(MediaPlaybackSession session);
    public class AudioPlayer : MutableModel
    {
        private readonly DispatcherTimer Timer;
        private bool _autoPlay;
        private double _volume;
        public int DispatcherTimerSecs { get; set; }

        public event PlayingEventHandler OnPlaying;
        public event EmptyEventHandler OnClosed;
        public event EmptyEventHandler OnMediaEnded;
        public event PlayerStateChangeEventHandler OnStateChanged;

        /// <summary>
        /// Creates a new instance of an <see cref="AudioPlayer"/>
        /// </summary>
        /// <param name="autoPlay">Whether to autoplay media.</param>
        /// <param name="defaultVolume">Default volume to set. Min-0, Max=1</param>
        /// <param name="tickIntervalMs">After how many milliseconds to update/call OnPlaying event.</param>
        public AudioPlayer(bool autoPlay = false, double defaultVolume = 1, double tickIntervalMs = 100)
        {
            _autoPlay = autoPlay;
            _volume = defaultVolume;
            Timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(tickIntervalMs)
            };
            Timer.Tick += Timer_Tick;
            MediaPlayer = new MediaPlayer();
            MediaPlayer.PlaybackSession.PlaybackStateChanged += (s, e) => OnStateChanged?.Invoke(s);
            MediaPlayer.MediaEnded += (s, e) => OnMediaEnded?.Invoke();
            MediaPlayer.Volume = defaultVolume;
            MediaPlayer.AutoPlay = autoPlay;
            MediaPlayer.AudioCategory = MediaPlayerAudioCategory.Media;
        }

        public MediaPlayer MediaPlayer { get; }
        public bool AutoPlay
        {
            get { return _autoPlay; }
            set
            {
                _autoPlay = value;
                MediaPlayer.AutoPlay = value;
                NotifyPropertyChanged();
            }
        }
        public double Volume
        {
            get { return _volume; }
            set
            {
                _volume = value;
                MediaPlayer.Volume = value;
                NotifyPropertyChanged();
            }
        }
        public bool IsPlaying => State == MediaPlaybackState.Playing;
        public MediaPlaybackState State => MediaPlayer.PlaybackSession.PlaybackState;

        public void ToggleState()
        {
            if (State == MediaPlaybackState.Playing)
            {
                Pause();
            }
            else if (State == MediaPlaybackState.Paused)
            {
                Play();
            }
        }
        public void Play()
        {
            MediaPlayer.Play();
            Timer.Start();
        }
        public void Pause()
        {
            MediaPlayer.Pause();
            Timer.Stop();
        }
        public void Stop() => Close();
        public void Close()
        {
            Pause();
            MediaPlayer.Source = null;
            OnClosed?.Invoke();
        }

        private void Timer_Tick(object sender, object e)
        {
            DispatcherTimerSecs++;
            OnPlaying?.Invoke(new PlayProgress(MediaPlayer.PlaybackSession.Position, MediaPlayer.PlaybackSession.NaturalDuration));
        }
    }
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