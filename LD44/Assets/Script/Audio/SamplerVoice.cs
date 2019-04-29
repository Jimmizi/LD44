using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{

    /// <summary>
    /// Represents a primitive audio unit. Directly drives an AudioSource.
    /// </summary>
    
    [RequireComponent(typeof(AudioSource))]
    public class SamplerVoice : MonoBehaviour
    {
        private readonly ASREnvelope _envelope = new ASREnvelope();

        private AudioSource _audioSource;
        private uint _samplesUntilEnvelopeTrigger;

        private void PrepareClip(AudioClip audioClip, double startTime, double attackTime, double sustainTime,
            double releaseTime)
        {
            sustainTime = (sustainTime < 0.0) ? (audioClip.length - releaseTime) : sustainTime;
            sustainTime = (sustainTime > attackTime) ? (sustainTime - attackTime) : 0.0;
            
            _envelope.Reset(attackTime, sustainTime, releaseTime, AudioSettings.outputSampleRate);
            double timeUntilTrigger = (startTime > AudioSettings.dspTime) ? (startTime - AudioSettings.dspTime) : 0.0;
            _samplesUntilEnvelopeTrigger = (uint) (timeUntilTrigger * AudioSettings.outputSampleRate);

            _audioSource.clip = audioClip;
        }
        
        /*
         * When sustainTime is negative, the envelope extends the entire
         * audio sample. This is rather unconventional, but we might want
         * for a sample to play out completely yet still attack and release
         * with some delay.
         */
        public void Play(AudioClip audioClip, double startTime, double attackTime = 0.05, double sustainTime = -1.0,
            double releaseTime = 0.5)
        {
         
            PrepareClip(audioClip, startTime, attackTime, sustainTime, releaseTime);
            _audioSource.PlayScheduled(startTime);
            

        }
        
        public void Play(AudioClip audioClip, float pitch, double startTime, double attackTime = 0.05, double sustainTime = -1.0,
            double releaseTime = 0.5)
        {
            
            PrepareClip(audioClip, startTime, attackTime, sustainTime, releaseTime);
            _audioSource.pitch = pitch;
            _audioSource.PlayScheduled(startTime);
            

        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            for (int sx = 0; sx < data.Length; sx += channels)
            {
                double volume = 0.0;
                if (_samplesUntilEnvelopeTrigger == 0)
                {
                    volume = _envelope.GetLevel();
                }
                else
                {
                    --_samplesUntilEnvelopeTrigger;
                }

                for (int cx = 0; cx < channels; cx++)
                {
                    data[sx + cx] *= (float) volume;
                }
            }
        }
    }
}
