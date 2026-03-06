using Assets.Scripts;
using NUnit.Framework;


[TestFixture]
public class PlayerStateTests
{
    // --- getName tests ---

    [Test]
    public void DodgingState_GetName_ReturnsDodging()
    {
        var state = new DodgingState();
        Assert.AreEqual(Constants.dodgingStateName, state.getName()); 
    }

    [Test]
    public void JumpingState_GetName_ReturnsJumping()
    {
        var state = new JumpingState();
        Assert.AreEqual(Constants.jumpingStateName, state.getName());
    }

    [Test]
    public void CrushedState_GetName_ReturnsCrushed()
    {
        var state = new CrushedState();
        Assert.AreEqual(Constants.crushedStateName, state.getName());
    }

    [Test]
    public void FallingState_GetName_ReturnsFalling()
    {
        var state = new FallingState();
        Assert.AreEqual(Constants.fallingStateName, state.getName());
    }

    [Test]
    public void RidingFallerState_GetName_ReturnsRidingFaller()
    {
        var state = new RidingFallerState(null);
        Assert.AreEqual(Constants.ridingFallerStateName, state.getName());
    }

    // --- canBeDamaged tests ---

    [Test]
    public void DodgingState_CanBeDamaged_ReturnsTrue()
    {
        var state = new DodgingState();
        Assert.IsTrue(state.canBeDamaged());
    }

    [Test]
    public void RidingFallerState_CanBeDamaged_ReturnsTrue()
    {
        var state = new RidingFallerState(null);
        Assert.IsTrue(state.canBeDamaged());
    }

    [Test]
    public void JumpingState_CanBeDamaged_ReturnsFalse()
    {
        var state = new JumpingState();
        Assert.IsFalse(state.canBeDamaged());
    }

    [Test]
    public void FallingState_CanBeDamaged_ReturnsFalse()
    {
        var state = new FallingState();
        Assert.IsFalse(state.canBeDamaged());
    }

    [Test]
    public void CrushedState_CanBeDamaged_ReturnsFalse()
    {
        var state = new CrushedState();
        Assert.IsFalse(state.canBeDamaged());
    }
}
