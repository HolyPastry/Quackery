using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static partial class DialogQueueServices
{
    public static Action<string> QueueDialog = (dialog) => { };
    public static Func<WaitUntil> WaitUntilAllDialogEnds = () => new WaitUntil(() => true);
}

namespace Quackery
{
    public class DialogQueueExtension : MonoBehaviour
    {
        [SerializeField] private float dialogDelay = 0.1f; // Delay between dialog starts
        private readonly Queue<string> dialogQueue = new();

        void OnEnable()
        {
            DialogQueueServices.QueueDialog = EnqueueDialog;
            DialogQueueServices.WaitUntilAllDialogEnds = WaitUntilAllDialogEnds;
        }
        void OnDisable()
        {
            DialogQueueServices.QueueDialog = (inkKNot) => { };
            DialogQueueServices.WaitUntilAllDialogEnds = () => new WaitUntil(() => true);
        }

        void Start()
        {
            StartCoroutine(ProcessDialogQueue());
        }

        private IEnumerator ProcessDialogQueue()
        {
            while (true)
            {
                yield return DialogServices.WaitUntilDialogEnds();
                if (dialogQueue.Count > 0)
                {
                    yield return new WaitForSeconds(dialogDelay); // Wait for the specified delay
                    string inkKnot = dialogQueue.Dequeue();
                    DialogServices.Start(inkKnot);
                }
                else
                {
                    yield return null; // Wait for the next frame if no dialog is queued
                }
            }
        }

        private void EnqueueDialog(string inkKnot)
        {
            dialogQueue.Enqueue(inkKnot);
        }

        internal WaitUntil WaitUntilAllDialogEnds()
        {
            return new WaitUntil(() => !DialogServices.IsDialogInProgress() && dialogQueue.Count == 0);
        }
    }
}
