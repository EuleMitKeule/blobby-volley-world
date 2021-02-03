using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Game.Entities;
using Blobby.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Blobby.Game
{
    public interface IMatchState
    {
        void EnterState();

        void ExitState();

        void OnPlayer(Player player);

        void OnGround();

        void OnSideChanged(Side newSide);

        void OnBombTimerStopped();
    }
}
