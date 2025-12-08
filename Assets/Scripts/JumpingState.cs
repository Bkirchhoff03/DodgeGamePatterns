using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    class JumpingState : IPlayerState
    {
        public void handleInput(PlayerController playerController, PlayerController.MoveInput moveInput)
        {
            // Handle input specific to dodging state
        }

        public void update(PlayerController playerController)
        {
            // Update logic specific to dodging state
        }
    }
}
