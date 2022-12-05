// using Blobby.Game;
// using Blobby.Models;
// using UnityEngine;
//
// namespace Blobby.Components
// {
//     public class ServerComponent : MonoBehaviour
//     {
//         void Awake()
//         {
//             var serverData = IoHelper.LoadServerData();
//             serverData ??= new ServerData()
//             {
//                 Name = "BV Server",
//                 Host = "127.0.0.1",
//                 Port = 1337,
//                 PlayerCount = 0,
//                 MasterServerHost = "bvmaster.eulenet.eu",
//                 MasterServerPort = 443
//             };
//             serverData.MatchData = IoHelper.LoadServerMatchData();
//             serverData.MatchData ??= new MatchData();
//
//             //TODO: if no file is found, ask in console for input
//
//             IoHelper.SaveServerData(serverData);
//             IoHelper.SaveServerMatchData(serverData.MatchData);
//
//             ServerHandler.Initialize(serverData);
//         }
//     }
// }
