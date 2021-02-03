using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Models;
using Blobby.Networking;
using Blobby.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Blobby.Game.States
{
    public class ClientMenuState : IClientState
    {
        public void EnterState()
        {
            MatchHandler.BlackoutEffect?.Blackout();

            ClientConnection.Dispose();
        }

        public void ExitState() { }

        public void OnButtonRanked()
        {
            if (ClientConnection.UserData == null) return;
            
            ClientConnection.ToggleMatchQueue();
        }

        public void OnButtonLocalPlay()
        {
            MatchHandler.Ai = false;
            MatchHandler.SetState(MatchHandler.ClientGameLocalState);
        }

        public void OnButtonLocalPlayAi()
        {
            MatchHandler.Ai = true;
            MatchHandler.SetState(MatchHandler.ClientGameLocalState);
        }

        public void OnButtonResume() { }

        public void OnButtonSurrender() { }

        public void OnButtonRevanche() { }

        public void OnButtonMenu() { }

        public void OnEscapeDown() { }

        public void OnConnecting(ServerData serverData)
        {
            MatchHandler.ServerData = serverData;
            MatchHandler.SetState(MatchHandler.ClientGameOnlineState);
        }

        public void OnConnected() { }

        public void OnDisconnected()
        {
            ClientConnection.Start();
        }

        public void OnMatchOver(Side winner, int scoreLeft, int scoreRight, int time) { }

        public void OnRematchReceived() { }

        public void OnMatchStopped()
        {
            Debug.Log("match stopped");
        }

        public void OnSlideOver() { }

        public void OnBlackoutOver()
        {
            Time.timeScale = 1f;

            MainThreadManager.Run(() =>
            {
                MenuHelper.SetPanelPause(false);

                MatchHandler.Match?.Dispose();

                MatchHandler.ZoomEffect?.ZoomOut();
            });
        }

        public void OnWhiteoutOver() { }

        public void OnZoomOutOver()
        {
            MainThreadManager.Run(() =>
            {
                MapHelper.ChangeMap(Map.Menu);

                MatchHandler.BlackoutEffect?.Whiteout();
            });
        }
    }
}
