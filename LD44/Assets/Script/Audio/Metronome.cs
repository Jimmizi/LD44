using System;
using UnityEngine;

namespace Audio
{

    /// <summary>
    /// Emits a tick event on every beat subdivision at specified tempo.
    /// </summary>

    public class Metronome : MonoBehaviour
    {
        /// <summary>
        /// Subscribe to this to listen for ticks coming from the metronome.
        /// This also passes the time that the tick should occur, relative to AudioSettings.dspTime.
        /// That means you can schedule audio system calls in the future with this info.
        /// </summary>
        public event Action<double> Ticked;
        
        [SerializeField, Tooltip("The tempo in beats per minute"), Range(15f, 200f)] private double _tempo = 120.0;
        [SerializeField, Tooltip("The number of ticks per beat"), Range(1, 8)] private int _subdivisions = 4;

        private double _tickLength;
        private double _nextTickTime;

        public void SetTempo(double tempo)
        {
            _tempo = tempo;
            Recalculate();
        }
        
        private void Reset()
        {
            Recalculate();
            // bump the next tick time ahead the length of one tick so we don't get a double trigger
            _nextTickTime = AudioSettings.dspTime + _tickLength;
        }

        private void Recalculate()
        {
            double beatsPerSecond = _tempo / 60.0;
            double ticksPerSecond = beatsPerSecond * _subdivisions;
            _tickLength = 1.0 / ticksPerSecond;
        }

        private void Awake()
        {
            Reset();
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                Recalculate();
            }
        }

        private void Update()
        {
            double currentTime = AudioSettings.dspTime;
            currentTime += Time.deltaTime; /* Look-ahead one frame (approx.) */

            while (currentTime > _nextTickTime)
            {
                if (Ticked != null)
                {
                    Ticked(_nextTickTime);
                }

                _nextTickTime += _tickLength;
            }
        }
    }
}
