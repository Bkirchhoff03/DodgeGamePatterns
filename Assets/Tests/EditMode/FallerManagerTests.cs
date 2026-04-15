using Assets.Scripts;
using NUnit.Framework;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;


[TestFixture]
public class FallerManagerTests
{
    private GameManager gameManager;
    private FallerManager fallerManager;
    [SetUp]
    public void SetUp()
    {
        gameManager = new GameManager();
        fallerManager = new FallerManager();
        // FallerManager now owns the faller dictionary, sprite, and spawn height logic
        fallerManager.init(FallerManager.FallerType.Block, 60.0f);

    }



    [Test]
    public void FallerManager_GaussianRandom_InBounds()
    {

        float randVal = FallerManager.instance().RandomGaussian(Constants.minX, Constants.maxX);
        Assert.GreaterOrEqual(randVal, Constants.minX,
            "Random spawn X point " + randVal + " should be greater than " + Constants.minX);
        Assert.LessOrEqual(randVal, Constants.maxX,
            "Random spawn X point " + randVal + " should be less than " + Constants.maxX);
    }
}