using System.Collections;
using UnityEngine;

namespace Quackery.TetrisBill
{
    public class TetrisGameTest : MonoBehaviour
    {
        IEnumerator Start()
        {
            yield return FlowServices.WaitUntilEndOfSetup();
            yield return BillServices.WaitUntilReady();
            var game = FindObjectOfType<TetrisGame>();
            if (game == null)
            {
                Debug.LogError("TetrisGame not found");
                yield break;
            }
            game.StartGame();
        }
    }
}

