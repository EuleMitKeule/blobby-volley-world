// using Blobby;
// using UnityEngine;
//
// namespace Blobby.Models
// {
//     [System.Serializable]
//     public class ControlsData
//     {
//         public KeyCode[] Keys = new KeyCode[3];
//
//         public ControlsData(int playerNum)
//         {
//             switch (playerNum)
//             {
//                 case 1:
//                     Keys[0] = KeyCode.W;
//                     Keys[1] = KeyCode.A;
//                     Keys[2] = KeyCode.D;
//                     break;
//                 case 2:
//                     Keys[0] = KeyCode.UpArrow;
//                     Keys[1] = KeyCode.LeftArrow;
//                     Keys[2] = KeyCode.RightArrow;
//                     break;
//                 case 3:
//                     Keys[0] = KeyCode.U;
//                     Keys[1] = KeyCode.H;
//                     Keys[2] = KeyCode.K;
//                     break;
//                 case 4:
//                     Keys[0] = KeyCode.Keypad8;
//                     Keys[1] = KeyCode.Keypad4;
//                     Keys[2] = KeyCode.Keypad6;
//                     break;
//                 default:
//                     break;
//             }
//         }
//
//         public void SetControl(Control control, KeyCode keyCode)
//         {
//             Keys[(int)control] = keyCode;
//         }
//     }
// }