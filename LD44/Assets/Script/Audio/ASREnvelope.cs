using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    /// <summary>
    /// Attack sustain release envelope.
    /// </summary>

    public class ASREnvelope
    {
        private enum State
        {
            IDLE,
            ATTACK,
            SUSTAIN,
            RELEASE
        }

        private State _state;
        private double _attackIncrement;
        private uint _sustainSamples;
        private double _releaseIncrement;
        private double _outputLevel;

        public void Reset(double attackTime, double sustainTime, double releaseTime, int sampleRate)
        {
            _state = State.ATTACK;
            _attackIncrement = (attackTime > 0.0) ? (1.0 / (attackTime * sampleRate)) : 1.0;
            _sustainSamples = (uint) (sustainTime * sampleRate);
            _releaseIncrement = (releaseTime > 0.0) ? (1.0 / (releaseTime * sampleRate)) : 1.0;
            _outputLevel = 0.0;
        }

        public double GetLevel()
        {
            switch (_state)
            {
                case State.IDLE:
                    _outputLevel = 0.0;
                    break;
                case State.ATTACK:
                    _outputLevel += _attackIncrement;
                    if (_outputLevel > 1.0)
                    {
                        _outputLevel = 1.0;
                        _state = State.SUSTAIN;
                    }
                    break;
                case State.SUSTAIN:
                    if ((_sustainSamples == 0) || (--_sustainSamples == 0))
                    {
                        _state = State.RELEASE;
                    }
                    break;
                case State.RELEASE:
                    _outputLevel -= _releaseIncrement;
                    if (_outputLevel < 0.0)
                    {
                        _outputLevel = 0.0;
                        _state = State.IDLE;
                    }
                    break;
                 default:
                     throw new ArgumentOutOfRangeException();
            }

            return _outputLevel;
        }
    }
}
