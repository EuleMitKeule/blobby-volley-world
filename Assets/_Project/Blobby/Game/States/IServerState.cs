﻿using BeardedManStudios.Forge.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blobby.Models;
using UnityEngine;

namespace Blobby.Game.States
{
    public interface IServerState
    {
        void EnterState();

        void ExitState();

        void OnServerDisconnected();

        void OnPlayerJoined(PlayerData playerData);

        void OnAllPlayersConnected();

        void OnMatchOver(Side winner);

        void OnSurrenderReceived(int playerNum);

        void OnPlayerDisconnected(int playerNum);

        void OnAllPlayersDisconnected();

        void OnRevancheReceived(int playerNum, bool isRevanche);

        void OnServerCloseTimerStopped();

        void OnMatchStopped();
    }
}
