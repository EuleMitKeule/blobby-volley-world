// using BeardedManStudios.Forge.Networking.Unity;
// using Blobby.Game.Entities;
// using Blobby.Models;
// using Blobby.Networking;
// using Blobby.UserInterface;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using BeardedManStudios.Forge.Networking;
// using UnityEngine;
// using UnityEngine.UI;
// using Object = UnityEngine.Object;
//
// namespace Blobby.Game.States
// {
//     public class ClientGameOnlineState : IClientState
//     {
//         public void EnterState()
//         {
//             MatchHandler.IsWaitingForGame = true;
//             
//             MatchHandler.BlackoutEffect?.Blackout();
//         }
//
//         public void ExitState() { }
//
//         public void OnButtonRanked() { }
//
//         public void OnButtonLocalPlay() { }
//
//         public void OnButtonLocalPlayAi() { }
//
//         public void OnButtonResume()
//         {
//             MenuHelper.TogglePanelPause();
//         }
//
//         public void OnButtonSurrender()
//         {
//             MenuHelper.SetPanelPause(false);
//             ClientConnection.SendSurrender();
//         }
//
//         public void OnButtonRevanche()
//         {
//             ClientConnection.SendRevanche(true);
//         }
//
//         public void OnButtonMenu()
//         {
//             ClientConnection.SendRevanche(false);
//
//             MatchHandler.SetState(MatchHandler.ClientMenuState);
//         }
//
//         public void OnEscapeDown()
//         {
//             MenuHelper.TogglePanelPause();
//         }
//
//         public void OnConnecting(ServerData serverData) { }
//
//         public void OnConnected()
//         {
//             MatchHandler.SlideEffect?.Slide(Side.Right);
//         }
//
//         public void OnDisconnected()
//         {
//             Debug.Log("OnDisconnected");
//             MatchHandler.SetState(MatchHandler.ClientMenuState);
//         }
//
//         public void OnMatchOver(Side winner, int scoreLeft, int scoreRight, int time)
//         {
//             MainThreadManager.Run(() =>
//             {
//                 if (!(MatchHandler.Match is ClientMatchComponent clientMatch)) return;
//
//                 GameObject targetPlayer;
//
//                 if (winner == Side.None)
//                 {
//                     var revancheButton = GameObject.Find("button_over_revanche").GetComponent<Button>();
//                     revancheButton.interactable = false;
//
//                     targetPlayer = clientMatch.Players[clientMatch.NetPlayerNum];
//                 }
//                 else
//                 {
//                     var revancheButton = GameObject.Find("button_over_revanche").GetComponent<Button>();
//                     revancheButton.interactable = true;
//
//                     targetPlayer = winner == Side.Left ? clientMatch.Players[0] : clientMatch.Players[1];
//                 }
//
//                 var targetPlayerObj = targetPlayer;
//
//                 var usernames = (from playerData in clientMatch.PlayerDataList select playerData.Name).ToArray();
//
//                 var color = targetPlayer.GetComponent<PlayerNetworkComponent>().PlayerData.Color;
//
//                 PanelOver.Populate(usernames, new int[] { scoreLeft, scoreRight }, time, winner, color);
//
//                 MenuHelper.SetPanelPause(false);
//
//                 
//                 
//                 //MatchHandler.ZoomEffect?.Dispose();
//                 MatchHandler.ZoomEffect?.ZoomIn(targetPlayerObj.transform);
//
//                 InputHelper.CursorVisible = true;
//             });
//         }
//
//         public void OnStartReceived()
//         {
//             MatchHandler.IsWaitingForGame = false;
//         }
//
//         public void OnRematchReceived()
//         {
//             if (!(MatchHandler.Match is ClientMatchComponent clientMatch)) return;
//
//             //TODO Ball destroyen ??
//
//             MatchHandler.ZoomEffect?.ZoomOut();
//
//             InputHelper.CursorVisible = false;
//         }
//
//         public void OnMatchStopped() { }
//
//         public void OnSlideOver()
//         {
//             MatchHandler.BlackoutEffect.Whiteout();
//         }
//
//         public void OnBlackoutOver()
//         {
//             MainThreadManager.Run(() =>
//             {
//                 ClientConnection.Connect();
//
//                 var clientMatchObj = Object.Instantiate(PrefabHelper.ClientMatch);
//                 var clientMatchComponent = clientMatchObj.GetComponent<ClientMatchComponent>();
//                 clientMatchComponent.MatchData = MatchHandler.ServerData.MatchData;
//
//                 MatchHandler.Match = clientMatchComponent;
//
//                 InputHelper.CursorVisible = false;
//             });
//         }
//
//         public void OnWhiteoutOver() { }
//
//         public void OnZoomOutOver() { }
//     }
// }
