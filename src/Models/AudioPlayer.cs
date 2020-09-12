using System;
using Common;
using Common.Models;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;

namespace UWP.Common.Helpers.Models
{
    public delegate void DoubleEventHandler(double d);
    public delegate void PlayingEventHandler(PlayProgress progress);
    public delegate void PlayerStateChangeEventHandler(MediaPlaybackSession session);
    public delegate void MediaErrorEventHandler(MediaPlayerFailedEventArgs args);
    public class AudioPlayer : MutableModel
    {
        private readonly DispatcherTimer Timer;
        private bool _autoPlay;
        private double _volume;
        public int DispatcherTimerSecs { get; set; }

        /// <summary>
        /// Occurs when the buffering progress changes.
        /// </summary>
        public event DoubleEventHandler OnBuffer;
        /// <summary>
        /// Occurs when the download progress changes.
        /// </summary>
        public event DoubleEventHandler OnDownload;
        public event PlayingEventHandler OnPlaying;
        /// <summary>
        /// Occurs when media playback is stopped and the MediaSource cleared.
        /// </summary>
        public event EmptyEventHandler OnClosed;
        /// <summary>
        /// Occurs when the media finishes playback.
        /// </summary>
        public event EmptyEventHandler OnMediaEnded;
        public event PlayerStateChangeEventHandler OnStateChanged;
        /// <summary>
        /// Occurs when an error is encountered with the media.
        /// </summary>
        public event MediaErrorEventHandler OnError;
        /// <summary>
        /// Occurs when the media successfully opens.
        /// </summary>
        public event PlayerStateChangeEventHandler OnOpened;

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
            MediaPlayer.MediaFailed += (s, e) => OnError?.Invoke(e);
            MediaPlayer.MediaOpened += (s, e) => OnOpened?.Invoke(s.PlaybackSession);
            MediaPlayer.PlaybackSession.BufferingProgressChanged += (s, e) => OnBuffer?.Invoke(s.BufferingProgress);
            MediaPlayer.PlaybackSession.DownloadProgressChanged += (s, e) => OnDownload?.Invoke(s.DownloadProgress);
        }

        public MediaPlayer MediaPlayer { get; }
        /// <summary>
        /// Whether to autoplay media.
        /// </summary>
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
        /// <summary>
        /// Default volume to set. Min-0, Max=1
        /// </summary>
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

        public void SetMediaDisplay(StorageFile file, RandomAccessStreamReference img,string title=default,string subtitle=default)
        {
            try
            {
                MediaPlaybackItem item = new MediaPlaybackItem(MediaSource.CreateFromStorageFile(file))
                {
                    AutoLoadedDisplayProperties = AutoLoadedDisplayPropertyKind.MusicOrVideo
                };
                var props = item.GetDisplayProperties();
                props.Type = MediaPlaybackType.Video;
                props.VideoProperties.Title = title.IsValid() ? title : file.DisplayName;
                props.VideoProperties.Subtitle = subtitle.IsValid() ? subtitle : string.Empty;
                if (img != null)
                {
                    props.Thumbnail = img;
                }
                item.ApplyDisplayProperties(props);
                MediaPlayer.Source = item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
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
        public void SkipTo(double secs, double total = 0)
        {
            if (MediaPlayer.PlaybackSession.CanSeek)
            {
                double _total = total > 0 ? total : MediaPlayer.GetTotalSeconds();
                secs = secs < 0 ? 0 : secs;
                if (secs < _total)
                {
                    MediaPlayer.PlaybackSession.Position = TimeSpan.FromSeconds(secs);
                }
            }
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
}