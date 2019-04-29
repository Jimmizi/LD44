using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fx
{



    public class JumpFx : StateMachineBehaviour
    {
        
        [SerializeField] private GameObject _particleSystemPrefab;
        
        private Transform _particlesTransform;
        private ParticleSystem _particleSystem;

        private bool _triggered;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            GameObject system = Instantiate(_particleSystemPrefab);
            _particleSystem = system.GetComponent<ParticleSystem>();
            _particleSystem.transform.position = animator.rootPosition;
            _particleSystem.Simulate(0.3f);
            _particleSystem.Play();
            
        }
        
        // This will be called every frame whilst in the state.
        public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_particleSystem == null)
                return;
        
        
            if (!_particleSystem.isPlaying)
                _particleSystem.Play();
        }
    }

}