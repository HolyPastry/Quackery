
using System.Collections;
using UnityEngine;

namespace Quackery.StateMachines
{

    public abstract class State
    {
        protected StateMachine _stateMachine;

        protected Coroutine _routine;

        public State(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public virtual void Enter()
        {
            _routine = _stateMachine.StartCoroutine(StateRoutine());
        }
        public virtual void Exit()
        {
            if (_routine != null)
            {
                _stateMachine.StopCoroutine(_routine);
                _routine = null;
            }
        }

        public virtual IEnumerator StateRoutine()
        {
            yield return null;
        }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
    }
}