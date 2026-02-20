using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public interface IPlayerState
    {
        public void EnterState(PlayerController playerController)
        {

        }
        public void ExitState(PlayerController playerController)
        {

        }
        public string getName();
        public bool canBeDamaged();
        public IPlayerState HandleInput(PlayerController playerController, PlayerController.MoveDirection moveInput);
        public IPlayerState Update(PlayerController playerController);
    }
    
}
