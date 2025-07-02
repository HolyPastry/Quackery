using System.Collections;

using UnityEngine;
using UnityEngine.Assertions;

namespace Quackery.Followers.Tests
{
    public class FollowerTest : MonoBehaviour
    {

        void OnEnable()
        {
            FollowerEvents.OnFollowersChanged += OnFollowersChangedHandler;
        }
        void OnDisable()
        {
            FollowerEvents.OnFollowersChanged -= OnFollowersChangedHandler;
        }
        // A Test behaves as an ordinary method
        IEnumerator Start()
        {
            yield return FlowServices.WaitUntilEndOfSetup();
            yield return FollowerServices.WaitUntilReady();
            Assert.IsTrue(FollowerServices.GetNumberOfFollowers() == 100, "Followers should be 100 at start");
            FollowerServices.ModifyFollowers(10);
            Assert.IsTrue(FollowerServices.GetNumberOfFollowers() == 110, "Followers should be 110 after modification");
            FollowerServices.ModifyFollowers(-20);
            Assert.IsTrue(FollowerServices.GetNumberOfFollowers() == 90, "Followers should be 90 after modification");
            FollowerServices.SetNumberOfFollowers(50);
            Assert.IsTrue(FollowerServices.GetNumberOfFollowers() == 50, "Followers should be 50 after setting number");
            Debug.Log("Follower test completed successfully.");
        }

        private void OnFollowersChangedHandler()
        {
            Debug.Log("Followers changed: " + FollowerServices.GetNumberOfFollowers());
        }

        // // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // // `yield return null;` to skip a frame.
        // [UnityTest]
        // public IEnumerator FollowerTestWithEnumeratorPasses()
        // {
        //     // Use the Assert class to test conditions.
        //     // Use yield to skip a frame.
        //     yield return null;
        // }
    }
}
