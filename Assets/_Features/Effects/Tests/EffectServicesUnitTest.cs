using System;
using System.Collections;
using System.Collections.Generic;
using Quackery.Decks;
using Quackery.Effects;
using Quackery.Inventories;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Assertions;

namespace Quackery
{
    public class EffectServicesUnitTest : MonoBehaviour, IEffectCarrier
    {
        [SerializeField] private List<EffectData> _testEffectDataList;
        [SerializeField] private float _timeScale = 3f;

        public List<EffectData> EffectDataList => _testEffectDataList;
        private EffectData _testEffectData => _testEffectDataList[0];

        public static Action SubmitAcknowledgment = delegate { };

        private int _acknowledged = 0;

        void OnEnable()
        {
            SubmitAcknowledgment = () => _acknowledged++;
        }

        void OnDisable()
        {
            SubmitAcknowledgment = delegate { };
        }


        IEnumerator Start()
        {
            yield return FlowServices.WaitUntilEndOfSetup();
            StartCoroutine(TestEffectServices());
        }

        private IEnumerator TestEffectServices()
        {
            Time.timeScale = _timeScale;
            int numEffect = _testEffectDataList.Count;

            Debug.Log("EffectServicesUnitTest: Start Test");

            //
            //
            // public static Action<EffectData, object> Add = (effect, linkedObject) => { };


            yield return EffectServices.Add(this);
            Assert.IsTrue(EffectServices.GetCurrent().Count == numEffect
            , "Expected three effects to remain after adding test effect.");

            //
            //
            // public static Action<Predicate<Effect>> Remove = (predicate) => { };

            yield return EffectServices.Remove(e => e.Data == _testEffectData);
            Assert.IsTrue(EffectServices.GetCurrent().Count == numEffect - 1
            , "Expected one effect to remain after removing test effect.");

            //
            //
            // public static Action<object> RemoveLinkedToObject = (linkedObject) => { };

            yield return EffectServices.Add(this);
            yield return EffectServices.RemoveLinkedToObject(this);
            Assert.IsTrue(EffectServices.GetCurrent().Count == 0,
                "Expected no effects to remain after removing linked object.");

            //
            //
            // public static Func<Type, float> GetModifier = (effectData) => 0;

            yield return EffectServices.Add(this);
            Assert.IsTrue(EffectServices.GetModifier(typeof(TestStatusEffectData)) == 3.5f
            , "Expected modifier to be 3.5.");
            yield return EffectServices.Add(this);
            Assert.IsTrue(EffectServices.GetModifier(typeof(TestStatusEffectData)) == 7f,
                "Expected modifier to be 3 after adding all effects of type TestStatusEffectData.");
            yield return EffectServices.Remove(e => e.Data is TestStatusEffectData);
            Assert.IsTrue(EffectServices.GetModifier(typeof(TestStatusEffectData)) == 0
            , "Expected modifier to be 0 after removing all effects of type TestStatusEffectData.");

            yield return EffectServices.RemoveLinkedToObject(this);

            //
            //
            // public static Func<List<Effect>> GetCurrent = () => new();

            Assert.IsTrue(EffectServices.GetCurrent().Count == 0, "Expected no effects to remain after removing test effect.");

            yield return EffectServices.Add(this);
            yield return EffectServices.Add(this);

            Assert.IsTrue(EffectServices.GetCurrent().Count == numEffect * 2, "Expected six effects to remain after execution.");
            yield return EffectServices.RemoveLinkedToObject(this);



            //
            //
            // internal static Func<EnumEffectTrigger, Card, Coroutine> Execute = (trigger, card) => null;
            yield return EffectServices.Add(this);
            _acknowledged = 0;
            yield return EffectServices.Execute(EnumEffectTrigger.OnCardPlayed, this);
            Assert.IsTrue(_acknowledged == 2, $"Expected two acknowledgments, got {_acknowledged}.");
            _acknowledged = 0;

            yield return EffectServices.RemoveLinkedToObject(this);

            //
            //
            // internal static Func<Coroutine> UpdateDurationEffects = () => null;
            yield return EffectServices.Add(this);
            yield return EffectServices.UpdateDurationEffects();
            Assert.IsTrue(EffectServices.GetModifier(typeof(TestStatusEffectData)) == 2.5f
            , "Expected modifier to be 2.5. got " + EffectServices.GetModifier(typeof(TestStatusEffectData)));

            yield return EffectServices.UpdateDurationEffects();
            Assert.IsTrue(EffectServices.GetCurrent().Count == numEffect - 1,
             "Expected two effects, got " + EffectServices.GetCurrent().Count + ".");
            yield return EffectServices.RemoveLinkedToObject(this);



            // public static Func<EnumEffectTrigger, CardPile, Coroutine> ExecutePile = (trigger, cardPile) => null;

            // internal static Action CleanEffects = delegate { };



            // internal static Func<EffectData, int, int> CounterEffect = (counterEffect, value) => 0;



            Debug.Log("EffectServicesUnitTest: Successfully completed all tests.");

            yield return EffectServices.Add(this);
            Time.timeScale = 1f;
        }

    }
}
