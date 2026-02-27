using Assets.Scripts;
using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

[TestFixture]
public class PlayerStateIntegrationTests
{
    private GameObject playerObject;
    private PlayerController playerController;

    [SetUp]
    public void SetUp()
    {
        playerObject = new GameObject("TestPlayer");
        playerObject.AddComponent<SpriteRenderer>();
        playerObject.AddComponent<Rigidbody2D>();
        playerController = playerObject.AddComponent<PlayerController>();

        // Provide a dummy punching arm to avoid null references
        var armObject = new GameObject("TestArm");
        armObject.AddComponent<PunchingArmController>();
        playerController.punchingArm = armObject;
    }

    [TearDown]
    public void TearDown()
    {
        if (playerController.punchingArm != null)
            Object.DestroyImmediate(playerController.punchingArm);
        Object.DestroyImmediate(playerObject);
    }

    [UnityTest]
    public IEnumerator PlayerController_StartState_IsDodging()
    {
        yield return null; // let Start() run

        Assert.AreEqual(Constants.dodgingStateName, playerController.state.getName());
    }

    [UnityTest]
    public IEnumerator PlayerController_Crush_SetsCrushedState()
    {
        yield return null;

        playerController.crush();

        Assert.AreEqual(Constants.crushedStateName, playerController.state.getName());
    }

    [UnityTest]
    public IEnumerator DodgingState_Update_ReturnsSelf()
    {
        yield return null;

        string stateBefore = playerController.state.getName();
        // Let a frame pass so Update() runs
        yield return null;
        string stateAfter = playerController.state.getName();

        Assert.AreEqual(Constants.dodgingStateName, stateBefore);
        Assert.AreEqual(Constants.dodgingStateName, stateAfter);
    }

    [UnityTest]
    public IEnumerator CrushedState_AfterTimer_TransitionsToDodging()
    {
        yield return null;
        playerController.crush();
        Assert.AreEqual(Constants.crushedStateName, playerController.state.getName());

        // CrushedState timer is 0.5 seconds — wait a bit longer to be safe
        yield return new WaitForSeconds(0.6f);

        Assert.AreEqual(Constants.dodgingStateName, playerController.state.getName());
    }
}