using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery.Bills
{
    public class OverdueCross : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private Image _cross;
        public enum State
        {
            Overdue,
            DueToday,
            NotDue
        }

        private State _state = State.NotDue;
        private TweenerCore<Vector3, Vector3, VectorOptions> _loopTween;

        public State CurrentState => _state;

        public void SetState(State newState)
        {
            if (newState == _state) return;
            _state = newState;
            if (_loopTween != null)
            {
                _loopTween.Kill();
                _loopTween = null;
            }

            switch (newState)
            {
                case State.Overdue:
                    gameObject.SetActive(true);
                    _cross.color = Color.black;
                    transform.localScale = Vector3.one;
                    break;

                case State.DueToday:
                    gameObject.SetActive(true);
                    _cross.color = Color.red;
                    _loopTween = transform.DOScale(0.3f, 1f)
                         .SetLoops(-1, LoopType.Yoyo)
                         .SetEase(Ease.InOutSine);
                    // .SetAutoKill(false);
                    break;
                case State.NotDue:
                    _cross.gameObject.SetActive(false);
                    return;
            }
        }
    }
}