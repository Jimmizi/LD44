using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{

    /// <summary>
    /// Triggers SamplerVoice instances.
    /// </summary>

    public class Sampler : MonoBehaviour
    {
        
        [Header("Debug")]
        [SerializeField] private bool _debugPlay;

        [Header("Config")]
        [SerializeField] private AudioClip _audioClip;
        [SerializeField, Range(0f, 2f)] private double _attackTime;
        [SerializeField, Range(0f, 2f)] private double _releaseTime;
        [SerializeField, Range(1, 8)] private int _maxVoices = 2;
        [SerializeField] private SamplerVoice _samplerVoicePrefab;

        [SerializeField] private Metronome _metronome;
        
        private SamplerVoice[] _voices;
        private int _voiceIndex;
        private bool _queuePlay = false;

        private void Awake()
        {
            _voices = new SamplerVoice[_maxVoices];
            for (int i = 0; i < _maxVoices; i++)
            {
                SamplerVoice samplerVoice = Instantiate(_samplerVoicePrefab);
                samplerVoice.transform.parent = transform;
                samplerVoice.transform.localPosition = Vector3.zero;
                _voices[i] = samplerVoice;
            }
        }

        private void Update()
        {
            if (_debugPlay)
            {
                _queuePlay = true;
                _debugPlay = false;
            }
        }
        
        /*private void Play()
        {
            _voices[_voiceIndex].Play(_audioClip, 0.0, _attackTime, -1.0, _releaseTime);
            _voiceIndex = (_voiceIndex + 1) % _voices.Length;
        }*/

        private void OnEnable() {
            if (_metronome != null)
            {
                _metronome.Ticked += HandleTicked;
            }
        }

        private void OnDisable()
        {
            if (_metronome != null)
            {
                _metronome.Ticked -= HandleTicked;
            }
        }

        private void HandleTicked(double tickTime) {

            if (_queuePlay)
            {
                _voices[_voiceIndex].Play(_audioClip, tickTime, _attackTime, -1.0, _releaseTime);
                _voiceIndex = (_voiceIndex + 1) % _voices.Length;
                _queuePlay = false;
            }
        }
        
    }

}