using Blobby;
using Blobby.Game;
using Blobby.Game.Entities;

public interface IJumpStrategy
{
    void OnJumpDown();

    void OnJumpUp();

    void OnJump();

    void OnJumpHold();
}

public static class JumpStrategyFactory
{
    public static IJumpStrategy Create(Player player, MatchComponent matchComponent)
    {
        return matchComponent.JumpMode switch
        {
            JumpMode.Standard => new StandardJumpStrategy(player, matchComponent),
            JumpMode.NoJump => new NoJumpStrategy(),
            JumpMode.Pogo => new PogoJumpStrategy(player, matchComponent),
            JumpMode.Spring => new SpringJumpStrategy(player, matchComponent),
            _ => null
        };
    }
}