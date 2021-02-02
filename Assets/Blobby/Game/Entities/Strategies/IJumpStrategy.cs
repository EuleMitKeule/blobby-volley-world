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
    public static IJumpStrategy Create(Player player, Match match)
    {
        return match.JumpMode switch
        {
            JumpMode.Standard => new StandardJumpStrategy(player, match),
            JumpMode.NoJump => new NoJumpStrategy(),
            JumpMode.Pogo => new PogoJumpStrategy(player, match),
            JumpMode.Spring => new SpringJumpStrategy(player, match),
            _ => null
        };
    }
}