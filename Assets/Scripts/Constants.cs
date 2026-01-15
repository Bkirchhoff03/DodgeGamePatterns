using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public static class Constants
    {
        public const float minX = -11.25f;
        public const float maxX = 11.25f;
        public const float spawnY = 12.0f;
        public const float minFallerSpeed = 1.0f;
        public const float maxFallerSpeed = 2.0f;
        public const float minFallerSize = 0.5f;
        public const float maxFallerSize = 3.0f;
        public const float moveSpeed = 5.0f;
        public const float dodgeSpeed = 10.0f;
        public const float gameGravity = 0.05f;//0.1f;
        public const float playerGravity = 4.8f;
        public const string jumpingStateName = "Jumping";
        public const string dodgingStateName = "Dodging";
        public const string crushedStateName = "Crushed";
        public const string ridingFallerStateName = "RidingFaller";
        public const string fallingStateName = "Falling";
        public const string fallerPhysicsMaterialPath = "Physics/FallerPhysicsMaterial";
        public const string fallerPhysicsMaterial2DPath = "Physics/FallerPhysicsMaterial2D";
        public const string fallerNamePrefix = "Faller_";

    }
}
