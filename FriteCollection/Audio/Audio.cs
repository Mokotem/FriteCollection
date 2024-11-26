using System;
using Microsoft.Xna.Framework.Media;

namespace FriteCollection.Audio
{

    public abstract class Audio
    {
        private protected bool _isPlaying = false;
        private protected float _songState = -1f;
    }

    /// <summary>
    /// Music. (mp3, wma, ogg)
    /// </summary>
    public class Music : Audio
    {
        private static Scripting.List<int> songIDs = new Scripting.List<int>();
        private static int _playingSong;
        private Song _audio;
        private int _ID;
        private bool _loop;

        /// <summary>
        /// Creates a Music.
        /// </summary>
        public Music(Song audioFile)
        {
            _audio = audioFile;
            Duration = (float)audioFile.Duration.TotalSeconds;
            CreateSongID();
        }

        public static float Volume
        {
            set
            {
                MediaPlayer.Volume = value;
            }
        }

        /// <summary>
        /// Starts playing the music.
        /// </summary>
        public void Start()
        {
            MediaPlayer.IsRepeating = _loop;
            _playingSong = _ID;
            MediaPlayer.Play(_audio);
            _songState = Scripting.Time.Timer;
        }

        /// <summary>
        /// Checks if the song is playing.
        /// </summary>
        public bool isPlaying
        {
            get
            {
                if (_playingSong == _ID && _loop == true)
                {
                    return true;
                }
                else if (_songState < 0)
                {
                    return false;
                }
                else
                {
                    return Scripting.Time.Timer - _songState < _audio.Duration.TotalSeconds;
                }
            }
        }

        /// <summary>
        /// Returns how long the music have been playing (seconds). Returns `-1` if not playing.
        /// </summary>
        public float TimeState
        {
            get
            {
                if (isPlaying)
                {
                    return Scripting.Time.Timer - _songState;
                }
                else
                {
                    return -1f;
                }
            }
        }

        /// <summary>
        /// Music duration (seconds).
        /// </summary>
        public readonly float Duration;

        /// <summary>
        /// Stop the music if it is playing.
        /// </summary>
        public void Stop()
        {
            if (_playingSong == _ID)
            {
                MediaPlayer.Stop();
            }
        }

        /// <summary>
        /// Loops the music.
        /// </summary>
        public bool Loop
        {
            set
            {
                _loop = value;
            }
        }

        /// <summary>
        /// Stops any playing music.
        /// </summary>
        public static void StopAllMusics()
        {
            MediaPlayer.Stop();
            _playingSong = 0;
        }

        private void CreateSongID()
        {
            if (songIDs.Count <= 0)
            {
                _ID = 1;
                songIDs.Add(1);
            }
            else
            {
                _ID = songIDs[songIDs.Count - 1] + 1;
                songIDs.Add(_ID);
            }
        }

        /// <summary>
        /// Gets the audio file of the music.
        /// </summary>
        public Song File
        {
            get
            {
                return _audio;
            }
        }
    }

    /// <summary>
    /// Sound effect. (wav)
    /// </summary>
    public class SoundEffect : Audio
    {
        Microsoft.Xna.Framework.Audio.SoundEffect _audio;
        private float _pitch = 0;
        private bool _loop = false;
        private float _volume = 1f;

        public static float GeneralVolume = 1f;

        public bool Loop
        {
            set { _loop = value; }
        }

        /// <summary>
        /// Creates a Sound effect. 
        /// </summary>
        public SoundEffect(Microsoft.Xna.Framework.Audio.SoundEffect file)
        {
            _audio = file;
        }

        /// <summary>
        /// Creates a Sound effect. 
        /// </summary>
        public SoundEffect(Microsoft.Xna.Framework.Audio.SoundEffect file, float volume)
        {
            _audio = file;
            _volume = MathF.Max(0f, MathF.Min(volume, 1f));
        }

        /// <summary>
        /// Plays the sound.
        /// </summary>
        public void Play()
        {
            _audio.Play(pitch: _pitch, volume: _volume * GeneralVolume, pan: 0); 
        }

        /// <summary>
        /// Sound frequency (-1f to 1f).
        /// </summary>
        public float Pitch
        {
            set
            {
                _pitch = MathF.Max(-1f, MathF.Min(value, 1f));
            }
        }

        /// <summary>
        /// Volume (0f to 1f).
        /// </summary>
        public float Volume
        {
            set
            {
                _volume = MathF.Max(0, MathF.Min(value, 1f));
            }
        }
    }
}