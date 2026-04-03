using UnityEngine;

public interface IFallerBehavior
{
    bool UseSettleTimer { get; }
    bool FreezeRotation { get; }
    void BuildVisuals(GameObject fallerObj, Vector2 size);
    void OnFloorPause(GameObject fallerObj, Vector2 fallerSize);
    void OnUnfreeze(GameObject fallerObj);
    void HandleArmCollision(FallerController fc, PunchingArmController arm);
}
