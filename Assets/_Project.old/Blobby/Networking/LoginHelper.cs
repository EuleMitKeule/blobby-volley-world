// using Blobby.Game;
// using System.Net.Http;
// using _Project.BlobbyExternal.SimpleJSON;
// using Blobby.Models;
//
// namespace Blobby.Networking
// {
//     public static class LoginHelper
//     {
//         static string _rootUrl = "https://bvonline.eulenet.eu/api/";
//         static string Token { get; set; }
//         
//         #region Events
//
//         public static event Action<UserData> Login;
//         public static event Action LoginFailed;
//         public static event Action Logout;
//         public static event Action RegisterSuccess;
//         public static event Action RegisterFailed;
//
//         #endregion
//
//         /// <summary>
//         /// Invoked when the matchmaking queue gets updated
//         /// </summary>
//         public static event Action<int, int> QueueInfoChanged;
//
//         [RuntimeInitializeOnLoadMethod]
//         static void Initialize()
//         {
//             if (ServerHandler.IsServer) return;
//
//             var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
//
//             Token = new string(Enumerable.Range(1, 16).Select(_ => chars[Random.Range(0, chars.Length)]).ToArray());
//             
//             var userData = IoHelper.LoadUserData();
//             Task.Run(() => LoginRequest(userData.name, userData.password));
//
//             LoginFailed += OnLoginFailed;
//         }
//
//         #region Account
//
//         public static void LogoutUser()
//         {
//             Logout?.Invoke();
//         }
//
//         static void OnLoginFailed()
//         {
//
//         }
//
//         #endregion
//
//         //TODO: swagger.io
//         #region WebRequests
//
//         public static async Task LoginRequest(string username, string password)
//         {
//             var loginData = new LoginData()
//             {
//                 username = username,
//                 password = password
//             };
//
//             var jsonData = JsonUtility.ToJson(loginData);
//             var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
//             var url = _rootUrl + $"login";
//
//             using var client = new HttpClient();
//             var response = await client.PostAsync(url, content);
//
//             if (response != null)
//             {
//                 var json = await response.Content.ReadAsStringAsync();
//                 var dic = BeardedManStudios.SimpleJSON.JSON.Parse(json);
//                 var token = dic["token"];
//
//                 var userData = await GetUserData(username, token, password);
//                 if (userData != null) MainThreadManager.Run(() => Login?.Invoke(userData));
//                 else MainThreadManager.Run(() => LoginFailed?.Invoke());
//             }
//             else
//             {
//                 MainThreadManager.Run(() => LoginFailed?.Invoke());
//             }
//         }
//
//         public static async Task<UserData> GetUserData(string username, string token, string password)
//         {
//             var url = _rootUrl + $"user/{username}?token={token}";
//
//             using var client = new HttpClient();
//             var response = await client.GetAsync(url);
//             if (response != null)
//             {
//                 var json = await response.Content.ReadAsStringAsync();
//                 var node = JSON.Parse(json);
//
//                 var userData = new UserData()
//                 {
//                     name = node["username"],
//                     email = node["email"],
//                     elo = node["elo"],
//                     colorR = node["color_r"],
//                     colorG = node["color_g"],
//                     colorB = node["color_b"],
//                     hatID = node["hat_id"],
//                     eyesID = node["eyes_id"],
//                     mouthID = node["mouth_id"]
//                 };
//
//                 userData.token = token;
//                 userData.password = password;
//
//                 return userData;
//             }
//             else
//             {
//                 return null;
//             }
//         }
//
//         public static async Task PostColor(UserData userData)
//         {
//             var url = _rootUrl + $"user/{userData.name}?token={userData.token}";
//
//             using var client = new HttpClient();
//
//             var jsonData = $"{{\"color_r\": {userData.colorR.ToString(CultureInfo.GetCultureInfo("en-US"))}," +
//                            $"\"color_g\": {userData.colorG.ToString(CultureInfo.GetCultureInfo("en-US"))}," +
//                            $"\"color_b\": {userData.colorB.ToString(CultureInfo.GetCultureInfo("en-US"))}}}";
//
//             var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
//             var response = await client.PostAsync(url, content);
//             if (response != null && response.IsSuccessStatusCode)
//             {
//                 await LoginRequest(userData.name, userData.password);
//             }
//         }
//
//         public static async Task RegisterRequest(string username, string password, string email)
//         {
//             var loginData = new LoginData()
//             {
//                 username = username,
//                 password = password,
//                 Email = email
//             };
//
//             var jsonData = JsonUtility.ToJson(loginData);
//             var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
//             var url = _rootUrl + "register";
//
//             using var client = new HttpClient();
//             var response = await client.PostAsync(url, content);
//             if (response != null) MainThreadManager.Run(() => RegisterSuccess?.Invoke());
//             else MainThreadManager.Run(() => RegisterFailed?.Invoke());
//         }
//
//         public static async Task OnlineRequest()
//         {
//             var url = $"https://bvonline.eulenet.eu/online";
//
//             using var client = new HttpClient();
//             var content = new StringContent($"{{\"token\":\"{Token}\"}}", Encoding.UTF8, "application/json");
//
//             try
//             {
//                 await client.PostAsync(url, content);
//             }
//             catch (Exception e)
//             {
//                 Debug.Log("Could not send online request!");
//                 Debug.Log(e);
//             }
//         }
//
//         public static async Task QueueRequest()
//         {
//             string url = $"https://bvonline.eulenet.eu/queue";
//
//             using var client = new HttpClient();
//             var content = new StringContent($"{{\"token\":\"{Token}\"}}", Encoding.UTF8, "application/json");
//             await client.PostAsync(url, content);
//         }
//
//         public static async Task PlayerRequest()
//         {
//             var url = $"https://bvonline.eulenet.eu/info";
//
//             using var client = new HttpClient();
//
//             try
//             {
//                 var response = await client.GetAsync(url);
//                 if (response != null)
//                 {
//                     var json = await response.Content.ReadAsStringAsync();
//
//                     var node = JSON.Parse(json);
//
//                     var playersOnline = int.Parse(node["online"]);
//                     var playersQueue = int.Parse(node["queue"]);
//
//                     QueueInfoChanged?.Invoke(playersOnline, playersQueue);
//                 }
//             }
//             catch (Exception e)
//             {
//                 Debug.Log("Could not send player request!");
//                 Debug.Log(e);
//             }
//         }
//
//         #endregion
//     }
// }
