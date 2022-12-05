// using BeardedManStudios.Forge.Networking.Unity;
// using Blobby.Networking;
// using System.Linq;
// using Blobby.Models;
// using UnityEngine;
// using Object = UnityEngine.Object;
//
// namespace Blobby.Game.States
// {
//     public class ServerOverState : IServerState
//     {
//         public void EnterState()
//         {
//             ServerHandler.MatchComponent.InvokeOver(ServerHandler.Winner);
//
//             ServerConnection.StopAccepting();
//             ServerConnection.Unlist();
//             ServerConnection.SendOver(ServerHandler.Winner, ServerHandler.MatchComponent.ScoreLeft, ServerHandler.MatchComponent.ScoreRight, ServerHandler.MatchComponent.MatchTimer.MatchTime);
//
//             ServerHandler.RevancheWanted = new bool[ServerHandler.ServerData.MatchData.PlayerCount];
//
//             ServerHandler.ServerCloseTimer?.Start();
//         }
//
//         public void ExitState()
//         {
//
//         }
//
//         public void OnServerDisconnected()
//         {
//             Application.Quit();
//         }
//
//         public void OnPlayerJoined(PlayerData playerData)
//         {
//
//         }
//
//         public void OnAllPlayersConnected()
//         {
//
//         }
//
//         public void OnMatchOver(Side winner)
//         {
//
//         }
//
//         public void OnSurrenderReceived(int playerNum)
//         {
//
//         }
//
//         public void OnPlayerDisconnected(int playerNum)
//         {
//
//         }
//
//         public void OnAllPlayersDisconnected()
//         {
//             ServerHandler.ServerCloseTimer?.Stop();
//
//             MainThreadManager.Run(() =>
//             {
//                 if (ServerHandler.MatchComponent) Object.Destroy(ServerHandler.MatchComponent.gameObject);
//             });
//         }
//
//         public void OnRevancheReceived(int playerNum, bool isRevanche)
//         {
//             ServerHandler.RevancheWanted[playerNum] = isRevanche;
//
//             if (ServerHandler.RevancheWanted.All(val => val))
//             {
//                 ServerHandler.ServerCloseTimer?.Stop();
//
//                 ServerConnection.SendRematch();
//
//                 MainThreadManager.Run(() =>
//                 {
//                     if (ServerHandler.MatchComponent) ServerHandler.MatchComponent.Restart();
//                 });
//
//                 ServerHandler.SetState(ServerHandler.RunningState);
//             }
//         }
//
//         public void OnServerCloseTimerStopped()
//         {
//             MainThreadManager.Run(() =>
//             {
//                 if (ServerHandler.MatchComponent) Object.Destroy(ServerHandler.MatchComponent.gameObject);
//             });
//         }
//
//         public void OnMatchStopped()
//         {
//             ServerHandler.SetState(ServerHandler.RestartState);
//         }
//     }
// }
