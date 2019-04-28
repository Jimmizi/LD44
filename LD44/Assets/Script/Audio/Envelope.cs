using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{

    public abstract class EnvelopePhase
    {
        public double duration;

        public EnvelopePhase() { }
        public EnvelopePhase(EnvelopePhase other)
        {
            duration = other.duration;
        }

        public abstract float Ease(float p, float step);

    }
    
    public class FlatEnvelopePhase : EnvelopePhase
    {

        public FlatEnvelopePhase() { }
        public FlatEnvelopePhase(FlatEnvelopePhase other)
        {
            duration = other.duration;
        }
        
        public override float Ease(float p, float step)
        {
            return p;
        }
        
    }
    
    public class EasedEnvelopePhase : EnvelopePhase
    {
        public double target;
        public EasingType easeIn = EasingType.Linear;
        public EasingType easeOut = EasingType.Linear;

        public EasedEnvelopePhase() { }
        public EasedEnvelopePhase(EasedEnvelopePhase other)
        {
            duration = other.duration;
            target = other.target;
            easeIn = other.easeIn;
            easeOut = other.easeOut;
        }

        public override float Ease(float p, float step)
        {
            float e = 0;
            if (easeIn == EasingType.Linear) e = Easing.EaseOut(step, easeOut);
            else if (easeOut == EasingType.Linear) e = Easing.EaseIn(step, easeIn);
            else e = Easing.EaseInOut(step, easeIn, easeOut);

            return Mathf.Lerp(p, (float) target, e);
        }
        
    }

	/// <summary>
    /// Base envelope class.
    /// </summary>

    public abstract class Envelope
    {
        protected float _current;
        protected float? _sinceTriggered;

        public List<EnvelopePhase> _phases;

        public Envelope()
        {
            _current = 0;
            _sinceTriggered = null;
        }

        public void Trigger()
        {
            _sinceTriggered = 0;
        }

        public static implicit operator float(Envelope e)
        {
            return e._current;
        }

        public float GetLevel()
        {
            return _current;
        }

        public void Sample(float rate)
        {
            if (_phases == null)
            {
                _current = 0;
                return;
            }
                        
            if (_sinceTriggered == null)
            {
                _current = 0;
            }
            else
            {
                
                _sinceTriggered += ((float) 1.0 / rate);

                float t = _sinceTriggered.Value;
                float v = 0;

                for (int i = 0; i < _phases.Count; i++)
                {
                    if (_phases[i].duration <= 0) continue;
                    
                    float step = Mathf.Clamp01(t / ((float) _phases[i].duration));
                    if (step < 1)
                    {
                        _current = _phases[i].Ease(v, step);
                        return;
                    }
     
                    t -= (float) _phases[i].duration;
                    v = (float) _phases[i].Ease(v, step);
                    
                }

                _current = 0;
            }
            
            
        }
    }

}