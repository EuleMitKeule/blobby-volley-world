using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Models;
using Blobby.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Blobby.Game.States
{
    public class ClientGameLocalState : IClientState
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
            if (MatchHandler.MatchData == null) return;

            MenuHelper.TogglePanelPause();
            Time.timeScale = MatchHandler.MatchData.TimeScale;
        }

        public void OnButtonRevanche()
        {
            MainThreadManager.Run(() =>
            {
                if (!(MatchHandler.Match is LocalMatchComponent localMatch)) return;

                MatchHandler.ZoomEffect?.ZoomOut();

                localMatch.Restart();
            });
        }

        public void OnButtonSurrender()
        {
            if (MatchHandler.MatchData == null) return;

            MenuHelper.SetPanelPause(false);
            Time.timeScale = MatchHandler.MatchData.TimeScale;

            if (!(MatchHandler.Match is LocalMatchComponent localMatch)) return;
            localMatch.InvokeOver(Side.Right);
            SoundHelper.PlayAudio(SoundHelper.SoundClip.Whistle);
        }

        public void OnButtonMenu()
        {
            MatchHandler.SetState(MatchHandler.ClientMenuState);
        }

        public void OnEscapeDown()
        {
            if (MatchHandler.MatchData == null) return;

            MenuHelper.TogglePanelPause();

            var stopped = Time.timeScale == 0f;
            Time.timeScale = stopped ? MatchHandler.MatchData.TimeScale : 0f;
        }

        public void OnConnecting(ServerData serverData)
        {
            MatchHandler.ServerData = serverData;
            MatchHandler.SetState(MatchHandler.ClientGameOnlineState);
        }

        public void OnConnected() { }

        public void OnDisconnected() { }

        public void OnMatchOver(Side winner, int scoreLeft, int scoreRight, int time)
        {
            MainThreadManager.Run(() =>
            {
                if (!(MatchHandler.Match is LocalMatchComponent localMatch)) return;

                var revancheButton = GameObject.Find("button_over_revanche").GetComponent<Button>();
                revancheButton.interactable = true;

                MenuHelper.SetPanelPause(false);

                //MatchHandler.ZoomEffect?.Dispose();
                MatchHandler.ZoomEffect?.ZoomIn(localMatch.Players[winner == Side.Left ? 0 : 1].transform);

                var usernames = (from i in Enumerable.Range(0, 4) select "").ToArray();
                PanelOver.Populate(usernames, new int[] { localMatch.ScoreLeft, localMatch.ScoreRight }, localMatch.MatchTimer.MatchTime, winner);
            });
        }

        public void OnRematchReceived() { }

        public void OnMatchStopped() { }

        public void OnSlideOver()
        {
            MatchHandler.BlackoutEffect?.Whiteout();
        }

        public void OnBlackoutOver()
        {
            var prefab = MatchHandler.IsAi ? PrefabHelper.AiMatch : PrefabHelper.LocalMatch;
            MatchHandler.MatchData = MatchHandler.LocalMatchData;
            var matchObject = Object.Instantiate(prefab);
            MatchHandler.Match = matchObject.GetComponent<IMatch>();

            if (!(MatchHandler.Match is LocalMatchComponent localMatch)) return;

            localMatch.Over += MatchHandler.OnMatchOver; 

            MatchHandler.SlideEffect?.Slide(Side.Right);
        }

        public void OnWhiteoutOver()
        {
            MainThreadManager.Run(() =>
            {
                if (!(MatchHandler.Match is LocalMatchComponent localMatch)) return;

                localMatch.StartMatch();
            });
        }

        public void OnZoomOutOver() { }
    }
}
