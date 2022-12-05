// using System;
// using UnityEngine;
// using Blobby.UserInterface;
// using Blobby.Components;
// using Blobby.Game;
//
// namespace Blobby
// {
//
//     public static class InputHelper
//     {
//         public static bool CursorVisible { get; set; } 
//         
//         static Action[,] _downCallbacks = new Action[4, 3];
//         static Action[,] _upCallbacks = new Action[4, 3];
//         static Action _escapeCallback;
//         static Action _tabCallback;
//         static Action _enterCallback;
//
//         // [RuntimeInitializeOnLoadMethod]
//         static void Initialize()
//         {
//             TimeComponent.UpdateTicked += Update;
//
//             var cursorTex = Resources.Load<Texture2D>("Graphics/UI/cursor/cursor");
//             Cursor.SetCursor(cursorTex, Vector2.zero, CursorMode.Auto);
//             CursorVisible = true;
//         }
//
//         public static void SetDownCallback(Action callback, int playerNum, int key) => _downCallbacks[playerNum, key] = callback;
//
//         public static void SetUpCallback(Action callback, int playerNum, int key) => _upCallbacks[playerNum, key] = callback;
//
//         public static void SetEscapeCallback(Action callback) => _escapeCallback = callback;
//
//         public static void SetTabCallback(Action callback) => _tabCallback = callback;
//
//         public static void SetEnterCallback(Action callback) => _enterCallback = callback;
//
//         static void Update()
//         {
//             for (int i = 0; i < 4; i++)
//             {
//                 var controlsData = PanelSettings.SettingsData.Controls[i];
//
//                 for (int j = 0; j < 3; j++)
//                 {
//                     var key = controlsData.Keys[j];
//
//                     if (Input.GetKeyDown(key))
//                     {
//                         _downCallbacks[i, j]?.Invoke();
//                     }
//
//                     if (Input.GetKeyUp(key))
//                     {
//                         _upCallbacks[i, j]?.Invoke();
//                     }
//                 }
//             }
//
//             if (Input.GetKeyDown(KeyCode.Escape)) _escapeCallback?.Invoke();
//
//             if (Input.GetKeyDown(KeyCode.Tab)) _tabCallback?.Invoke();
//
//             if (Input.GetKeyDown(KeyCode.Return)) _enterCallback?.Invoke();
//
//             Cursor.visible = CursorVisible;
//         }
//     }
// }