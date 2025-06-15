using System;
using System.Collections;

using Bakery.Saves;
using Holypastry.Bakery.Flow;
using UnityEngine;
public class PurseManager : Service
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
        PurseServices.GetString = () => Mathf.Floor(_purse.Amount).ToString(); //MoneyFormat(_purse?.Amount ?? 0f);
        PurseServices.WaitUntilReady = () => WaitUntilReady;
    }

    private string MoneyFormat(float v)
    {
        var dollar = Mathf.Floor(v);
        var cents = Mathf.Floor((v - dollar) * 100);
        if (dollar > 0)
            return $"{dollar}.{cents:00}$";
        return $"{cents:00}¢";

    }

    void OnDisable()
    {
        PurseServices.Modify = delegate { };
    }

    protected override IEnumerator Start()
    {
        yield return FlowServices.WaitUntilReady();

        _purse = SaveServices.Load<SerialPurse>(SaveKey);
        _purse ??= new SerialPurse();
        _isReady = true;
    }

    private void Modify(float amount)
    {
        _purse.Amount += amount;
        SaveServices.Save(SaveKey, _purse);
        PurseEvents.OnPurseUpdated?.Invoke(_purse.Amount);
    }

}
