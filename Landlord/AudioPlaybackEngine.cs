using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Landlord
{
    class AudioPlaybackEngine : IDisposable
    {
        private readonly IWavePlayer outputDevice;
        private readonly MixingSampleProvider mixer;
        private Dictionary<string, CachedSound> cachedSoundFX;
        private List<string> songTitles;

        public AudioPlaybackEngine( int sampleRate = 44100, int channelCount = 2 )
        {
            cachedSoundFX = new Dictionary<string, CachedSound>()
            {
                { "openMenu", new CachedSound( @"res\sfx\openMenu.wav" ) },
                { "closeMenu", new CachedSound( @"res\sfx\closeMenu.wav" ) },
                { "openMap", new CachedSound( @"res\sfx\openMap.wav" ) },
                { "closeMap", new CachedSound( @"res\sfx\closeMap.wav" ) },
                { "grabBlueprint", new CachedSound( @"res\sfx\grabBlueprint.wav" ) }
            };
            songTitles = new List<string>()
            {
                "Track1"
            };

            outputDevice = new WaveOutEvent();
            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat( sampleRate, channelCount ) )
            {
                ReadFully = true
            };
            outputDevice.Init( mixer );
            outputDevice.Play();
        }

        public void PlaySound( string filename )
        {
            var input = new AudioFileReader( filename );
            AddMixerInput( new AutoDisposeFileReader( input ) );
        }

        public void PlayMusic( string title )
        {
            string filename = $@"./res/music/{title}.wav";
            var stream = new AudioFileReader( filename );
            var input = new LoopStream( stream );
            StopMusic();
            AddMixerInput( input );
        }

        public void PlayRandomMusic(bool firstTime = false)
        {
            int rand = Program.RNG.Next( 0, songTitles.Count );
            string filename = firstTime ? $@"./res/music/Main_Menu.wav" : $@"./res/music/{songTitles[rand]}.wav";
            var input = new AudioFileReader( filename );
            AddMixerInput( new AutoContinueMusicFileReader( input ) );
        }

        public void StopMusic()
        {
            mixer.RemoveAllMixerInputs();
        }

        private ISampleProvider ConvertToRightChannelCount(ISampleProvider input)
        {
            if (input.WaveFormat.Channels == mixer.WaveFormat.Channels)
                return input;

            if (input.WaveFormat.Channels == 1 && mixer.WaveFormat.Channels == 2)
                return new MonoToStereoSampleProvider(input);

            throw new NotImplementedException("Not yet implemented this channel count conversion");
        }

        public void PlaySound(CachedSound sound)
        {
            AddMixerInput(new CachedSoundSampleProvider(sound));
        }

        private void AddMixerInput(ISampleProvider input)
        {
            mixer.AddMixerInput(ConvertToRightChannelCount(input));
        }

        private void AddMixerInput( WaveStream input )
        {
            mixer.AddMixerInput( input );
        }

        public void Dispose()
        {
            outputDevice.Dispose();
        }

        public Dictionary<string, CachedSound> CachedSoundFX
        {
            get { return cachedSoundFX; }
            set { cachedSoundFX = value; }
        }
    }

    class AutoDisposeFileReader : ISampleProvider
    {
        private readonly AudioFileReader reader;
        private bool isDisposed;
        public AutoDisposeFileReader(AudioFileReader reader)
        {
            this.reader = reader;
            this.WaveFormat = reader.WaveFormat;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            if (isDisposed)
                return 0;
            int read = reader.Read(buffer, offset, count);
            if (read == 0)
            {
                reader.Dispose();
                isDisposed = true;
            }
            return read;
        }

        public WaveFormat WaveFormat { get; private set; }
    }

    class AutoContinueMusicFileReader : ISampleProvider
    {
        private readonly AudioFileReader reader;
        private bool isDisposed;
        public AutoContinueMusicFileReader( AudioFileReader reader )
        {
            this.reader = reader;
            this.WaveFormat = reader.WaveFormat;
        }

        public int Read( float[] buffer, int offset, int count )
        {
            if (isDisposed)
                return 0;
            int read = reader.Read( buffer, offset, count );
            if (read == 0)
            {
                reader.Dispose();
                isDisposed = true;
                Program.AudioEngine.PlayRandomMusic();
            }
            return read;
        }

        public WaveFormat WaveFormat { get; private set; }
    }

    class CachedSound
    {
        public float[] AudioData { get; private set; }
        public WaveFormat WaveFormat { get; private set; }
        public CachedSound(string audioFileName)
        {
            using (var audioFileReader = new AudioFileReader(audioFileName))
            {
                // TODO: could add resampling in here if required
                WaveFormat = audioFileReader.WaveFormat;
                var wholeFile = new List<float>((int)(audioFileReader.Length / 4));
                var readBuffer = new float[audioFileReader.WaveFormat.SampleRate * audioFileReader.WaveFormat.Channels];
                int samplesRead;
                while ((samplesRead = audioFileReader.Read(readBuffer, 0, readBuffer.Length)) > 0)
                {
                    wholeFile.AddRange(readBuffer.Take(samplesRead));
                }
                AudioData = wholeFile.ToArray();
            }
        }
    }

    class CachedSoundSampleProvider : ISampleProvider
    {
        private readonly CachedSound cachedSound;
        private long position;

        public CachedSoundSampleProvider(CachedSound cachedSound)
        {
            this.cachedSound = cachedSound;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            var availableSamples = cachedSound.AudioData.Length - position;
            var samplesToCopy = Math.Min(availableSamples, count);
            Array.Copy(cachedSound.AudioData, position, buffer, offset, samplesToCopy);
            position += samplesToCopy;
            return (int)samplesToCopy;
        }

        public WaveFormat WaveFormat { get { return cachedSound.WaveFormat; } }
    }

    public class LoopStream : WaveStream
    {
        WaveStream sourceStream;

        /// <summary>
        /// Creates a new Loop stream
        /// </summary>
        /// <param name="sourceStream">The stream to read from. Note: the Read method of this stream should return 0 when it reaches the end
        /// or else we will not loop to the start again.</param>
        public LoopStream( WaveStream sourceStream )
        {
            this.sourceStream = sourceStream;
            this.EnableLooping = true;
        }

        /// <summary>
        /// Use this to turn looping on or off
        /// </summary>
        public bool EnableLooping { get; set; }

        /// <summary>
        /// Return source stream's wave format
        /// </summary>
        public override WaveFormat WaveFormat
        {
            get { return sourceStream.WaveFormat; }
        }

        /// <summary>
        /// LoopStream simply returns
        /// </summary>
        public override long Length
        {
            get { return sourceStream.Length; }
        }

        /// <summary>
        /// LoopStream simply passes on positioning to source stream
        /// </summary>
        public override long Position
        {
            get { return sourceStream.Position; }
            set { sourceStream.Position = value; }
        }

        public override int Read( byte[] buffer, int offset, int count )
        {
            int totalBytesRead = 0;

            while (totalBytesRead < count)
            {
                int bytesRead = sourceStream.Read( buffer, offset + totalBytesRead, count - totalBytesRead );
                if (bytesRead == 0)
                {
                    if (sourceStream.Position == 0 || !EnableLooping)
                    {
                        // something wrong with the source stream
                        break;
                    }
                    // loop
                    sourceStream.Position = 0;
                }
                totalBytesRead += bytesRead;
            }
            return totalBytesRead;
        }
    }
}
