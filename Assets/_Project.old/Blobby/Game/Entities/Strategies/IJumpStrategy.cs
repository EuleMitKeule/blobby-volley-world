using Blobby;
using Blobby.Game;
using Blobby.Game.Entities;

namespace Blobby.Game.Entities.Strategies
{
    public interface IJumpStrategy
    {
        void OnJumpDown();

        void OnJumpUp();

        void OnJump();

        void OnJumpHold();
    }

    public static class JumpStrategyFactory
    {
        public static IJumpStrategy Create(PlayerComponent playerComponent, JumpMode jumpMode)
        {
            return jumpMode switch
            {
                JumpMode.Standard => new StandardJumpStrategy(playerComponent),
                JumpMode.NoJump => new NoJumpStrategy(),
                JumpMode.Pogo => new PogoJumpStrategy(playerComponent),
                JumpMode.Spring => new SpringJumpStrategy(),
                _ => null
            };
        }
    }
}