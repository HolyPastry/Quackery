using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quackery.StateMachines
{

    public class StateMachine : MonoBehaviour
    {
        [SerializeField] private bool _debugState;
        protected State _currentState;
        public State CurrentState => _currentState;

        private readonly List<(State, State, Func<bool>)> _transitions = new();

        protected virtual void OnEnable()
        {
            StateServices.GetCurrentState = () => _currentState;
        }

        protected virtual void OnDisable()
        {
            StateServices.GetCurrentState = () => null;
        }
        public virtual void ChangeState(State newState)
        {
            _currentState?.Exit();

            _currentState = newState;
            _currentState.Enter();
            if (_debugState) Debug.Log($"Changed state to {_currentState}");
        }


        void Update()
        {
            foreach (var (from, to, condition) in _transitions)
            {
                if (from != _currentState && from != null) continue;
                if (to == _currentState) continue;

                if (!condition()) continue;

                ChangeState(to);
                break;
            }

            _currentState?.Update();
        }
        void FixedUpdate() => _currentState?.FixedUpdate();

        internal void AddTransition(State from, State to, Func<bool> condition)
        {
            _transitions.Add((from, to, condition));
        }

        internal void AddAnyTransition(State to, Func<bool> condition)
        {
            _transitions.Add((null, to, condition));
        }
        protected void At(State from, State to, Func<bool> condition) => AddTransition(from, to, condition);

        protected void Any(State to, Func<bool> condition) => AddAnyTransition(to, condition);
    }
}
