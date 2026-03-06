using NUnit.Framework;
using Assets.Scripts;

[TestFixture]
public class ConstantsTests
{
    [Test]
    public void GameGravity_IsExpectedValue()
    {
        Assert.AreEqual(0.05f, Constants.gameGravity);
    }

    [Test]
    public void PlayerGravity_IsExpectedValue()
    {
        Assert.AreEqual(4.8f, Constants.playerGravity);
    }

    [Test]
    public void FallerNamePrefix_IsExpected()
    {
        Assert.AreEqual("Faller_", Constants.fallerNamePrefix);
    }

    [Test]
    public void SpawnBounds_AreSymmetric()
    {
        Assert.AreEqual(-Constants.maxX, Constants.minX,
            "minX should be the negative of maxX for symmetric spawning");
    }

    [Test]
    public void FallerSizeRange_MinLessThanMax()
    {
        Assert.Less(Constants.minFallerSize, Constants.maxFallerSize,
            "Min faller size must be less than max faller size");
    }
}