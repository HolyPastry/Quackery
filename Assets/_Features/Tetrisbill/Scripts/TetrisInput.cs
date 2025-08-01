using System.Collections;
using System.Collections.Generic;
using Holypastry.Bakery;
using Quackery.TetrisBill;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Quackery
{
    public class TetrisInput : MonoBehaviour,
                             IDragHandler,
                             IBeginDragHandler,
                              IEndDragHandler,
                               IPointerDownHandler,
                                IPointerUpHandler
    {
        [SerializeField] private float _dragThreshold = 75f; // Minimum drag distance to register a drag
        [SerializeField] private float _dragResetTime = 0.5f;
        [SerializeField] private float _holdTimerDuration = 0.5f; // Duration to hold before rotating
        public int DragDirection { get; private set; } = 0;  // -1 for left, 1 for right, 0 for no drag
        private bool _dragged;
        private float _timer;
        private bool _rotated;

        private CountdownTimer _holdTimer;

        void Awake()
        {
            _holdTimer = new CountdownTimer(_holdTimerDuration);
            _holdTimer.OnTimerEnd += () =>
            {
                TetrisGame.SetFastFall(true);
            };
        }

        void Update()
        {
            _holdTimer.Tick(Time.deltaTime);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            // Reset drag direction at the start of a drag
            DragDirection = 0;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Debug.Log(eventData.delta);
            // Determine drag direction based on mouse position
            if (eventData.delta.x > _dragThreshold)
            {
                _holdTimer.Stop();
                TetrisGame.SetFastFall(false);
                DragDirection = 1; // Right drag
                return;
            }
            if (eventData.delta.x < -_dragThreshold)
            {
                _holdTimer.Stop();
                TetrisGame.SetFastFall(false);
                DragDirection = -1; // Left drag
                return;
            }

            DragDirection = 0; // No horizontal drag

            if (eventData.delta.y > _dragThreshold && !_rotated)
            {
                _holdTimer.Stop();
                TetrisGame.SetFastFall(false);
                TetrisGame.Rotate();
                _rotated = true;
                return;
            }


        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _rotated = false;
            // Reset drag direction when the drag ends
            DragDirection = 0;

        }



        void LateUpdate()
        {

            if (DragDirection != 0)
            {
                _timer = 0f; // Reset timer when a drag is detected
                if (_dragged) return; // Prevent multiple actions on the same drag
                _dragged = true;
                // Handle the drag direction logic here
                if (DragDirection == 1)
                {
                    TetrisGame.MoveRight();

                    // Implement right movement logic
                }
                else if (DragDirection == -1)
                {
                    TetrisGame.MoveLeft();
                    // Implement left movement logic
                }
            }
            else
            {
                _timer += Time.deltaTime;
                if (_timer >= _dragResetTime)
                    _dragged = false;
            }

            DragDirection = 0;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _holdTimer.Stop();
            _holdTimer.Start();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _holdTimer.Stop();
            TetrisGame.SetFastFall(false);
        }
    }
}
