using System.Collections;
using System.Collections.Generic;
using Quackery.TetrisBill;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Quackery
{
    public class TetrisInput : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
    {
        [SerializeField] private float _dragResetTime = 0.5f;
        public int DragDirection = 0; // -1 for left, 1 for right, 0 for no drag
        private bool _dragged;
        private float _timer;


        public void OnBeginDrag(PointerEventData eventData)
        {
            // Reset drag direction at the start of a drag
            DragDirection = 0;
        }

        public void OnDrag(PointerEventData eventData)
        {
            // Determine drag direction based on mouse position
            if (eventData.delta.x > 10)
            {
                DragDirection = 1; // Right drag
            }
            else if (eventData.delta.x < 10)
            {
                DragDirection = -1; // Left drag
            }
            else
            {
                DragDirection = 0; // No horizontal drag
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // Reset drag direction when the drag ends
            DragDirection = 0;

        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (DragDirection == 0)
                TetrisGame.Rotate();
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
    }
}
