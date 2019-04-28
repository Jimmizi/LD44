using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;

namespace Audio
{
	
	/// <summary>
    /// Attack decay sustain release envelope with easing support.
    /// </summary>

    public class ADSREnvelope : Envelope
    {
        public ADSREnvelope()
        {
            EasedEnvelopePhase A = new EasedEnvelopePhase(); /* Attack */
            EasedEnvelopePhase D = new EasedEnvelopePhase(); /* Decay */
            FlatEnvelopePhase S = new FlatEnvelopePhase(); /* Sustain */
            EasedEnvelopePhase R = new EasedEnvelopePhase(); /* Release */

            A.target = 1.0;
            A.easeIn = EasingType.Cubic;
            A.easeOut = EasingType.Cubic;
            
            D.target = 0.9;
            R.target = 0.0;
            
            _phases = new List<EnvelopePhase>(new EnvelopePhase[]
            {
                A, D, S, R
            });
            
        }

        public void Trigger(double attackTime, double decayTime, double sustainTime, double releaseTime)
        {
            if (_phases != null)
            {
                _phases[0].duration = attackTime;
                _phases[1].duration = decayTime;
                _phases[2].duration = sustainTime;
                _phases[3].duration = releaseTime;
                Debug.Log("______________________________________");
                Debug.LogFormat("DURATION: {0}, {1}, {2}, {3}", attackTime, decayTime, sustainTime, releaseTime);
            }
            
            _sinceTriggered = 0;
        }
        
        
    }
}
