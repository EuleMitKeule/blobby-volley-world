using Blobby.Models;
using System.IO;
using UnityEngine;

namespace Blobby.Game
{
    public static class IoHelper
    {
        // [RuntimeInitializeOnLoadMethod]
        // static void Initialize()
        // {
        //     LoginHelper.Login += OnLogin;
        //     LoginHelper.Logout += OnLogout;
        // }
        //
        // static void OnLogin(UserData userData)
        // {
        //     SaveUserData(userData);
        // }
        //
        // static void OnLogout()
        // {
        //     DeleteUserData();
        // }
        //
        // public static UserData LoadUserData()
        // {
        //     var path = Application.persistentDataPath + @"\user";
        //
        //     if (!File.Exists(path)) return null;
        //
        //     var json = File.ReadAllText(path);
        //     var userData = JsonUtility.FromJson<UserData>(json);
        //     
        //     return userData;
        // }
        //
        // public static void SaveUserData(UserData userData)
        // {
        //     var path = Application.persistentDataPath + @"\user";
        //     var json = JsonUtility.ToJson(userData);
        //     File.WriteAllText(path, json);
        // }
        //
        // public static void DeleteUserData()
        // {
        //     var path = Application.persistentDataPath + @"\user";
        //     if (!File.Exists(path)) return;
        //     File.Delete(path);
        // }

        // public static SettingsData LoadSettingsData()
        // {
        //     var path = Application.persistentDataPath + @"\settings";
        //
        //     if (!File.Exists(path)) return null;
        //
        //     var json = File.ReadAllText(path);
        //     var settingsData = JsonUtility.FromJson<SettingsData>(json);
        //     return settingsData;
        // }
        //
        // public static void SaveSettingsData(SettingsData settingsData)
        // {
        //     var path = Application.persistentDataPath + @"\settings";
        //     var json = JsonUtility.ToJson(settingsData);
        //     File.WriteAllText(path, json);
        // }
        //
        // public static void DeleteSettingsData()
        // {
        //     var path = Application.persistentDataPath + @"\settings";
        //     if (!File.Exists(path)) return;
        //     File.Delete(path);
        // }

//         public static ServerData LoadServerData()
//         {
//             string path = "";
// #if UNITY_STANDALONE_LINUX
//             path = Directory.GetCurrentDirectory() + "/serverData.json";
// #else
//             path = Directory.GetCurrentDirectory() + @"\serverData.json";
// #endif
//             if (File.Exists(path))
//             {
//                 var json = File.ReadAllText(path);
//                 return JsonUtility.FromJson<ServerData>(json);
//             }
//             return null;
//         }
//
//         public static void SaveServerData(ServerData serverData)
//         {
//             string path = "";
// #if UNITY_STANDALONE_LINUX
//             path = Directory.GetCurrentDirectory() + "/serverData.json";
// #else
//             path = Directory.GetCurrentDirectory() + @"\serverData.json";
// #endif
//             var json = JsonUtility.ToJson(serverData);
//             File.WriteAllText(path, json);
//         }
//
//         public static MatchData LoadServerMatchData()
//         {
//             string path = "";
// #if UNITY_STANDALONE_LINUX
//             path = Directory.GetCurrentDirectory() + "/matchData.json";
// #else
//             path = Directory.GetCurrentDirectory() + @"\matchData.json";
// #endif
//             if (File.Exists(path))
//             {
//                 var json = File.ReadAllText(path);
//                 return JsonUtility.FromJson<MatchData>(json);
//             }
//             else return null;
//         }
//
//         public static void SaveServerMatchData(MatchData matchData)
//         {
//             string path = "";
// #if UNITY_STANDALONE_LINUX
//             path = Directory.GetCurrentDirectory() + "/matchData.json";
// #else
//             path = Directory.GetCurrentDirectory() + @"\matchData.json";
// #endif
//             var json = JsonUtility.ToJson(matchData);
//             File.WriteAllText(path, json);
//         }
    }
}
