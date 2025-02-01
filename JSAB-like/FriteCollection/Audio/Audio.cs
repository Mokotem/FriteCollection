using System;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

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
    public class Music : Audio, IDisposable
    {
        private Song _audio;
        private bool _loop;

        /// <summary>
        /// Creates a Music.
        /// </summary>
        public Music(Song audioFile)
        {
            _audio = audioFile;
            Duration = (float)audioFile.Duration.TotalSeconds;
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
            MediaPlayer.IsRepeating = true;
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
                if (_loop == true)
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
            MediaPlayer.Stop();
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

        public void Dispose()
        {
            _audio.Dispose();
        }
    }

    /// <summary>
    /// Sound effect. (wav)
    /// </summary>
    public class SoundEffect : Audio, IDisposable
    {
        Microsoft.Xna.Framework.Audio.SoundEffect _audio;
        private float _volume = 1f;
        SoundEffectInstance sfx;

        public static float GeneralVolume = 1f;

        /// <summary>
        /// Creates a Sound effect. 
        /// </summary>
        public SoundEffect(Microsoft.Xna.Framework.Audio.SoundEffect file)
        {
            _audio = file;
            _volume = 1f;
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
        public void Play(bool loop = false)
        {
            sfx = _audio.CreateInstance();
            sfx.IsLooped = loop;
            sfx.Volume = _volume;
            sfx.Play();
        }

        public void Stop()
        {
            if (sfx is not null)
            {
                sfx.Stop();
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
                if (sfx is not null)
                {
                    sfx.Volume = value;
                }
            }
        }

        public void Dispose()
        {
            _audio.Dispose();
            if (sfx is not null)
                sfx.Dispose();
        }
    }
}