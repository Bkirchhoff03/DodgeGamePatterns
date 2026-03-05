using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Assets.Scripts;
using UnityEditor.SceneManagement;

[TestFixture]
public class FallerControllerTests
{
    
    private GameObject fallerObject;
    private FallerController controller;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Assets/Scenes/SampleScene.unity");
        yield return null; // Wait a frame for scene to load

        
        FallerManager.instance().SpawnFaller(10f);
        FallerManager.instance().fallersInPlay.TryGetValue("Faller_1", out controller);
        fallerObject = controller.gameObject;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(fallerObject);
    }

    [Test]
    public void VerifyScene()
    {
        var gameObject = GameObject.Find("Player");

        Assert.That(gameObject, Is.Not.Null);
    }
    // --- FloorPause tests ---

    [UnityTest]
    public IEnumerator FloorPause_SetsIsFrozenTrue()
    {
        yield return null;

        controller.FloorPause();

        Assert.IsTrue(controller.amIFrozen());
    }

    [UnityTest]
    public IEnumerator FloorPause_SetsBodyTypeToStatic()
    {
        yield return null;

        controller.FloorPause();

        Assert.AreEqual(RigidbodyType2D.Static,
            fallerObject.GetComponent<Rigidbody2D>().bodyType);
    }

    [UnityTest]
    public IEnumerator FloorPause_SetsVelocityToZero()
    {
        var rb = fallerObject.GetComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector2(1f, -2f);
        yield return null;

        controller.FloorPause();

        Assert.AreEqual(Vector2.zero, rb.linearVelocity);
    }

    

    

    // --- StartRiding / StopRiding ---

    [Test]
    public void StartRiding_StopRiding_TogglesState()
    {
        controller.StartRiding();
        // No public getter for BeingRidden, but we verify no exception thrown
        // and StopRiding can be called after
        controller.StopRiding();
    }

    // --- shouldPointDamage ---

    [UnityTest]
    public IEnumerator ShouldPointDamage_PointBelowCenter_ReturnsTrue()
    {
        fallerObject.transform.position = new Vector3(0f, 5f, 0f);
        yield return null;

        bool result = controller.shouldPointDamage(new Vector2(0f, 3f));

        Assert.IsTrue(result);
    }

    [UnityTest]
    public IEnumerator ShouldPointDamage_PointAboveCenter_ReturnsFalse()
    {
        fallerObject.transform.position = new Vector3(0f, 5f, 0f);
        yield return null;

        bool result = controller.shouldPointDamage(new Vector2(0f, 7f));

        Assert.IsFalse(result);
    }

    // --- isRidingMe (documents inverted logic bug) ---

    [UnityTest]
    public IEnumerator IsRidingMe_PlayerWithinBounds_ReturnsFalse()
    {
        fallerObject.transform.position = new Vector3(0f, 5f, 0f);
        fallerObject.transform.localScale = new Vector3(2f, 1f, 1f);
        yield return null;

        // Player at x=0 is within bounds (-1 to 1), but returns false (inverted)
        bool result = controller.isRidingMe(new Vector3(0f, 6f, 0f));

        Assert.IsFalse(result, "Known bug: returns false when player IS within bounds");
    }

    [UnityTest]
    public IEnumerator IsRidingMe_PlayerOutsideBounds_ReturnsTrue()
    {
        fallerObject.transform.position = new Vector3(0f, 5f, 0f);
        fallerObject.transform.localScale = new Vector3(2f, 1f, 1f);
        yield return null;

        // Player at x=5 is outside bounds (-1 to 1), but returns true (inverted)
        bool result = controller.isRidingMe(new Vector3(5f, 6f, 0f));

        Assert.IsTrue(result, "Known bug: returns true when player is NOT within bounds");
    }
}