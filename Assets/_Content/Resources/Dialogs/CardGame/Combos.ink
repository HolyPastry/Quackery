=== HerbalBundleLevelCheck ===

{Herbalisim <1: ->HerbalBundleQuack| {Herbalisim <2: ->HerbalBundleHerbalist| ->HerbalBundleMasterHerbalist}}

->DONE

=== HerbalBundleQuack===
->HerbalBundleQuack.Non

=Non
Me: {~Plants are good for you!| Anything green is good! | Always take your vegetables ;)}

->DONE



=== HerbalBundleHerbalist===

{AilmentStress: ->HerbalBundleHerbalist.Stress | {AilmentMelancholy: ->HerbalBundleHerbalist.Melancholy | ->HerbalBundleHerbalist.Non}}

=Non
Me: {~Plants are such good healers.| Plants have so many benificial properties.| You can't go wrong with natural products | Just sitting with plants will make you feel better}

->DONE

=Stress
Me: { Lavender:Lavender is very calming.| {Valerian: Valerian is very calming. |{Chamomile: Chamoile is very calming.|->HerbalBundleQuack.Non}}}


->DONE

=Melancholy
Me: { Nettle:Nettle is a mood booster.| {LemonBalm: Lemon Balm is good for greif. |->HerbalBundleQuack.Non}}


->DONE

=== HerbalBundleMasterHerbalist===
{AilmentStress: ->HerbalBundleMasterHerbalist.Stress | {AilmentMelancholy: ->HerbalBundleMasterHerbalist.Melancholy | ->HerbalBundleMasterHerbalist.Non}}

=Non
Me: {~Herbal remedies are trending now but it's important to only take the high quality stuff.| Plant remedies is really a holistic approach.}

->DONE

=Stress
Me: { Lavender:The calming properties of lavender is found in it's essential oils. | {Valerian: Valerian helps with increasing  GABA, which is associated with feelings of calmness.  |{Chamomile: Chamoile binds with the  GABA receptors, making it a mild sedetive.|->HerbalBundleQuack.Non}}}


->DONE

=Melancholy
Me: { Nettle:Nettle contains serotonin and acetylcholine, which play a role in regulating mood| {LemonBalm: Lemon Balm soothes the nervous system offering a reprieve from heavy emotions. |->HerbalBundleQuack.Non}}


->DONE