using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public interface IPlayerState
    {

        public void handleInput(PlayerController playerController, PlayerController.MoveInput moveInput);
        public void update(PlayerController playerController);
    }
    
}
