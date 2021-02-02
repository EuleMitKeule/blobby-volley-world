using Blobby.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blobby.Game.States
{
    public interface IClientState
    {
        void EnterState();

        void ExitState();

        void OnButtonRanked();

        void OnButtonLocalPlay();

        void OnButtonLocalPlayAi();

        void OnButtonResume();

        void OnButtonSurrender();

        void OnButtonRevanche();

        void OnButtonMenu();

        void OnEscapeDown();

        void OnConnecting(ServerData serverData);

        void OnConnected();

        void OnDisconnected();

        void OnMatchOver(Side winner, int scoreLeft, int scoreRight, int time);

        void OnRematchReceived();

        void OnMatchStopped();

        void OnSlideOver();

        void OnBlackoutOver();

        void OnWhiteoutOver();

        void OnZoomOutOver();
    }
}
