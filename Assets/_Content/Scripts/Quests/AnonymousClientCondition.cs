using Holypastry.Bakery.Quests;
using Quackery.Clients;
using UnityEngine;

namespace Quackery
{
    [CreateAssetMenu(fileName = "AnonymousClientCondition", menuName = "Quackery/Quests/Conditions/Anonymous Client Condition")]
    public class AnonymousClientCondition : Condition
    {
        public override bool Check => ClientServices.IsCurrentClientAnonymous();
    }
}
