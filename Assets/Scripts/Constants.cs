using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public static class Constants
    {
        public const float minX = -11f;
        public const float maxX = 11f;
        public const float minXRescueSpawn = -12.4f;
        public const float maxXRescueSpawn = 12.4f;
        public const float spawnY = 25.0f;
        public const float minFallerSpeed = 1.0f;
        public const float maxFallerSpeed = 2.0f;
        public const float minFallerSize = 0.5f;
        public const float maxFallerSize = 3.0f;
        public const float moveSpeed = 7.5f;
        public const float dodgeSpeed = 10.0f;
        public const float gameGravity = 0.05f;//0.1f;
        public const float fallerGravityPostCollision = 0.25f;
        public const float playerGravity = 4.8f;
        public const float halfPlayerWidth = 0.25f;
        public const float boulderDynamicMass = 5.0f;
        public const float boulderSettleLinearThreshold = 0.15f;
        public const float boulderSettleAngularThreshold = 5.0f;
        public const float boulderSettleTime = 0.5f;
        public const float PunchingArmOffsetY = 0.45f;
        public const float PunchingArmOffsetX = 0.475f;
        public const int fallerCollisionFreezeThreshold = 30;
        public const float blockPunchForceMultiplier = 1.5f;
        public const float boulderPunchForceMultiplier = 10f;
        public const float idleDelay = 0.045f; // ~3 frames at 60fps — prevents idle flicker when switching directions
        public const float maxXJumpDistance = 7.66f;
        public const float maxYJumpHeight = 4.94f;
        public const float EMT_Radius = 5.0f;
        public const float EMT_Impulse_block = 2.5f;
        public const float EMT_Impulse_bolder = 10f;
        public static readonly Vector2 defaultFallerSize = new(1.5f, 1.5f);
        public const string heightTrackerText = "ft from the top!";
        public const string jumpingStateName = "Jumping";
        public const string dodgingStateName = "Dodging";
        public const string crushedStateName = "Crushed";
        public const string ridingFallerStateName = "RidingFaller";
        public const string fallingStateName = "Falling";
        public const string fallerPhysicsMaterialPath = "Physics/FallerPhysicsMaterial"; 
        public const string fallerPhysicsMaterial2DPath = "Physics/FallerPhysicsMaterial2D";
        public const string fallerNamePrefix = "Faller_";
        public static string fallerDataSavePath => Application.persistentDataPath + "/FallerSaveData/";
        public static string playerDataSavePath => Application.persistentDataPath + "/PlayerSaveData/";
        public static string saveFilePath => Application.persistentDataPath + "/SaveFiles/";

        
    }
}
