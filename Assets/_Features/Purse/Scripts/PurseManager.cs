using System;
using System.Collections;

using Bakery.Saves;
using UnityEngine;
public class PurseManager : MonoBehaviour
{
    private const string SaveKey = "Purse";
    [Serializable]
    public class SerialPurse : SerialData
    {
        public float Amount;
    }
    private SerialPurse _purse;


    void OnEnable()
    {
        PurseServices.Modify = Modify;
    }

    void OnDisable()
    {
        PurseServices.Modify = delegate { };
    }

    IEnumerator Start()
    {
        yield return FlowServices.WaitUntilReady();

        _purse = SaveServices.Load<SerialPurse>(SaveKey);
        _purse ??= new SerialPurse();
    }

    private void Modify(float amount)
    {
        _purse.Amount += amount;
        SaveServices.Save(SaveKey, _purse);
    }

}
