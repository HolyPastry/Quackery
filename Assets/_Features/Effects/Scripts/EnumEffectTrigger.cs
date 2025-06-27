namespace Quackery.Effects
{

    public enum EnumEffectTag
    {
        Activated,
        OneTime,
        Client,
        Status
    }
    public enum EnumEffectTrigger
    {
        OnActivated = 0,
        OnCardPlayed = 1,
        OnDraw = 2,

        Continous = 3,
        AfterCartCalculation = 4,
        OnEffectApplied = 5,
        Passive = 6,
        OnRoundStart = 7,
        OnRoundEnd = 8,
        OnDiscard = 9,
    }
}
