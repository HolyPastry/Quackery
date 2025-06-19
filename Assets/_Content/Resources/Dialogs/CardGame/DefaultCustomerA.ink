=== DefaultCustomerIntro === 
Customer: {Hi,|Hello,|Oh! Hi there!|Hello!!Hi there,}{I'm umm... not feeling too well.| I think I have the flu.| I'm feeling a little under the weather.| I'm not feeling great| Can you give me something? I'm feeling a bit off| I've not been feeling myself lately.| I'm not feeling my usual self.| Could you recommend me something| I'd like to feel better.| Do you have anything good for my condition?}

Me: {No problem!| Let me see...| Let's see what we've got here...}
->DONE

=== DefaultCustomerSucess ===
Customer: {HerbalBundleQuack: ->HerbalQuack| {HerbalBundleHerbalist: ->HerbalHerbalist | {HerbalBundleMasterHerbalist:->HerbalMasterHerbalist |{~Thank you!| Nice!| Thanks| Thanks a million| Thanks, bye!| Perfect!| Thank you so much| Much appriciated.| Thank you :)}}}}


=HerbalQuack
Me: {~Who knew plants could heal?!|I want to know more about plants now!}
->DONE

=HerbalHerbalist
Me: {I didn't know plants could be so good for you!|I should have eaten more greens a kid!}
->DONE

=HerbalMasterHerbalist
Me: {Wow you are so knowledgeable about plants!|You make me want to study plants too!| How long did it take you to learn so much about plants?!| Wow! You're a plant genius!}
->DONE

=== DefaultCustomerFailure === 

Customer: {Ummm no thank you| Oh that's not really my thing| Sorry, I've changed my mind| Sorry I got to go.| Oh that's too pricey. | Yeah that's above my buget.| I wasn't expecting this sort of poor service.| You've got to be kidding me this is a scam!| Are you sure you know what you're doing?| What kind of business are you running here?!| Yeah this isn't my kind of thing.}
-> DONE

