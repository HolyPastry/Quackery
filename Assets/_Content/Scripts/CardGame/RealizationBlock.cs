using System.Collections;
using UnityEngine;



namespace Quackery.Decks
{


    public class RealizationBlock : MonoBehaviour
    {
        private Coroutine _routine;

        void Start()
        {
            _routine = StartCoroutine(Test());


        }


        private IEnumerator Test()
        {
            yield return new WaitForSeconds(5f);
            Debug.Log("Test coroutine finished");




        }


    }
}
