using System.Collections;
using Holypastry.Bakery.Flow;
using UnityEngine;

public class SceneSetupPurse : SceneSetupScript
{
    [SerializeField] private float _initialAmount = 0f;

    public override IEnumerator Routine()
    {
        yield return PurseServices.WaitUntilReady();
        PurseServices.Modify(_initialAmount);
    }
}
