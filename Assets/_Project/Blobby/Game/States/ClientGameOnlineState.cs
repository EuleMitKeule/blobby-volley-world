using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Game.Entities;
using Blobby.Models;
using Blobby.Networking;
using Blobby.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Blobby.Game.States
{
    public class ClientGameOnlineState : IClientState
    {
        public void EnterState()
        {
            MatchHandler.BlackoutEffect?.Blackout();
        }

        public void ExitState() { }

        public void OnButtonRanked() { }

        public void OnButtonLocalPlay() { }

        public void OnButtonLocalPlayAi() { }

        public void OnButtonResume()
        {
            MenuHelper.TogglePanelPause();
        }

        public void OnButtonSurrender()
        {
            MenuHelper.SetPanelPause(false);
            ClientConnection.SendSurrender();
            Debug.Log("Sending surrender");
        }

        public void OnButtonRevanche()
        {
            Debug.Log("Sending Revanche wanted");
            ClientConnection.SendRevanche(true);
        }

        public void OnButtonMenu()
        {
            Debug.Log("Sending Revanche not wanted");
            ClientConnection.SendRevanche(false);

            MatchHandler.SetState(MatchHandler.ClientMenuState);
        }

        public void OnEscapeDown()
        {
            MenuHelper.TogglePanelPause();
        }

        public void OnConnecting(ServerData serverData) { }

        public void OnConnected()
        {
            MatchHandler.SlideEffect?.Slide(Side.Right);
        }

        public void OnDisconnected()
        {
            Debug.Log("Disconnected from server");

            MatchHandler.SetState(MatchHandler.ClientMenuState);
        }

        public void OnMatchOver(Side winner, int scoreLeft, int scoreRight, int time)
        {
            Debug.Log($"Over Received, winner = {winner}");

            MainThreadManager.Run(() =>
            {
                if (!(MatchHandler.Match is ClientMatch clientMatch)) return;

                ClientPlayer targetPlayer;

                if (winner == Side.None)
                {
                    var revancheButton = GameObject.Find("button_over_revanche").GetComponent<Button>();
                    revancheButton.interactable = false;

                    targetPlayer = clientMatch.Players[clientMatch.OwnPlayerNum];
                }
                else
                {
                    var revancheButton = GameObject.Find("button_over_revanche").GetComponent<Button>();
                    revancheButton.interactable = true;

                    targetPlayer = winner == Side.Left ? clientMatch.Players[0] : clientMatch.Players[1];
                }

                var targetPlayerObj = targetPlayer.PlayerObj;

                var usernames = (from playerData in clientMatch.PlayerDataList select playerData.Name).ToArray();

                PanelOver.Populate(usernames, new int[] { scoreLeft, scoreRight }, time, winner);

                MenuHelper.SetPanelPause(false);

                //MatchHandler.ZoomEffect?.Dispose();
                MatchHandler.ZoomEffect?.ZoomIn(targetPlayerObj.transform);
            });
        }

        public void OnRematchReceived()
        {
            if (!(MatchHandler.Match is ClientMatch clientMatch)) return;

            clientMatch.Ball?.Dispose();

            MatchHandler.ZoomEffect?.ZoomOut();
        }

        public void OnMatchStopped() { }

        public void OnSlideOver()
        {
            MatchHandler.BlackoutEffect.Whiteout();
        }

        public void OnBlackoutOver()
        {
            ClientConnection.Connect();

            MatchHandler.Match?.Dispose();
            MatchHandler.Match = new ClientMatch(MatchHandler.ServerData);
        }

        public void OnWhiteoutOver() { }

        public void OnZoomOutOver() { }
    }
}
