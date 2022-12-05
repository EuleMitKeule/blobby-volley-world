// using Blobby.Networking;
// using System.Threading.Tasks;
// using Blobby.Models;
//
// namespace Blobby.Game.States
// {
//     public class ServerRestartState : IServerState
//     {
//         public void EnterState()
//         {
//             ServerConnection.Stop();
//         }
//
//         public void ExitState()
//         {
//             
//         }
//
//         public async void OnServerDisconnected()
//         {
//             await Task.Delay(2000);
//             ServerHandler.Start();
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
//         public void OnPlayerDisconnected(int playernum)
//         {
//
//         }
//
//         public void OnAllPlayersDisconnected()
//         {
//
//         }
//
//         public void OnRevancheReceived(int playerNum, bool isRevanche)
//         {
//
//         }
//
//         public void OnServerCloseTimerStopped()
//         {
//
//         }
//
//         public void OnMatchStopped()
//         {
//
//         }
//     }
// }
