using Blobby.Game.Entities;

namespace Blobby.Game.States
{
    public interface IMatchState
    {
        void EnterState();

        void ExitState();

        void OnPlayer(PlayerComponent playerComponent);

        void OnGround();

        void OnSideChanged(Side newSide);

        void OnBombTimerStopped();
    }
}
