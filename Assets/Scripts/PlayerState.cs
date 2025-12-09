using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public interface IPlayerState
    {

        public IPlayerState HandleInput(PlayerController playerController, PlayerController.MoveType moveInput);
        public IPlayerState Update(PlayerController playerController);
    }
    
}
