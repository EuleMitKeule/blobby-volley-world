using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Game;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Blobby.Networking
{
    public static class LoginHelper
    {
        static string _rootUrl = "https://bvonline.eulenet.eu/api/";
        static string Token { get; set; }

        /// <summary>Timeout in seconds for HTTP requests.</summary>
        const int RequestTimeoutSeconds = 5;

        #region Events

        public static event Action<UserData> Login;
        public static event Action LoginFailed;
        public static event Action Logout;
        public static event Action RegisterSuccess;
        public static event Action RegisterFailed;

        #endregion

        /// <summary>
        /// Invoked when the matchmaking queue gets updated
        /// </summary>
        public static event Action<int, int> QueueInfoChanged;

        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            if (ServerHandler.IsServer) return;

            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";

            Token = new string(Enumerable.Range(1, 16).Select(_ => chars[Random.Range(0, chars.Length)]).ToArray());

            var userData = IoHelper.LoadUserData();
            Task.Run(() => LoginRequest(userData.name, userData.password));

            LoginFailed += OnLoginFailed;
        }

        #region Account

        public static void LogoutUser()
        {
            Logout?.Invoke();
        }

        static void OnLoginFailed()
        {

        }

        #endregion

        /// <summary>
        /// Creates an HttpClient with a short timeout to prevent hanging when servers are unreachable.
        /// </summary>
        static HttpClient CreateClient()
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(RequestTimeoutSeconds);
            return client;
        }

        //TODO: swagger.io
        #region WebRequests

        public static async Task LoginRequest(string username, string password)
        {
            try
            {
                var loginData = new LoginData()
                {
                    username = username,
                    password = password
                };

                var jsonData = JsonUtility.ToJson(loginData);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var url = _rootUrl + $"login";

                using var client = CreateClient();
                var response = await client.PostAsync(url, content);

                if (response != null && response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonUtility.FromJson<LoginResponse>(json);
                    var token = loginResponse.token;

                    var userData = await GetUserData(username, token, password);
                    if (userData != null)
                    {
                        OnlineStatusHelper.SetLoginOnline();
                        MainThreadManager.Run(() => Login?.Invoke(userData));
                    }
                    else
                    {
                        OnlineStatusHelper.SetLoginOffline();
                        MainThreadManager.Run(() => LoginFailed?.Invoke());
                    }
                }
                else
                {
                    OnlineStatusHelper.SetLoginOffline();
                    MainThreadManager.Run(() => LoginFailed?.Invoke());
                }
            }
            catch (Exception e)
            {
                Debug.Log($"Login request failed (server may be offline): {e.Message}");
                OnlineStatusHelper.SetLoginOffline();
                MainThreadManager.Run(() => LoginFailed?.Invoke());
            }
        }

        public static async Task<UserData> GetUserData(string username, string token, string password)
        {
            try
            {
                var url = _rootUrl + $"user/{username}?token={token}";

                using var client = CreateClient();
                var response = await client.GetAsync(url);
                if (response != null && response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var node = JsonUtility.FromJson<UserDataResponse>(json);

                    var userData = new UserData()
                    {
                        name = node.username,
                        email = node.email,
                        elo = node.elo,
                        colorR = node.color_r,
                        colorG = node.color_g,
                        colorB = node.color_b,
                        hatID = node.hat_id,
                        eyesID = node.eyes_id,
                        mouthID = node.mouth_id
                    };

                    userData.token = token;
                    userData.password = password;

                    return userData;
                }
            }
            catch (Exception e)
            {
                Debug.Log($"GetUserData request failed: {e.Message}");
            }

            return null;
        }

        public static async Task PostColor(UserData userData)
        {
            try
            {
                var url = _rootUrl + $"user/{userData.name}?token={userData.token}";

                using var client = CreateClient();

                var colorData = new ColorData
                {
                    color_r = userData.colorR,
                    color_g = userData.colorG,
                    color_b = userData.colorB
                };
                var jsonData = JsonUtility.ToJson(colorData);

                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
                if (response != null && response.IsSuccessStatusCode)
                {
                    await LoginRequest(userData.name, userData.password);
                }
            }
            catch (Exception e)
            {
                Debug.Log($"PostColor request failed: {e.Message}");
            }
        }

        public static async Task RegisterRequest(string username, string password, string email)
        {
            try
            {
                var loginData = new LoginData()
                {
                    username = username,
                    password = password,
                    Email = email
                };

                var jsonData = JsonUtility.ToJson(loginData);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var url = _rootUrl + "register";

                using var client = CreateClient();
                var response = await client.PostAsync(url, content);
                if (response != null && response.IsSuccessStatusCode)
                    MainThreadManager.Run(() => RegisterSuccess?.Invoke());
                else
                    MainThreadManager.Run(() => RegisterFailed?.Invoke());
            }
            catch (Exception e)
            {
                Debug.Log($"Register request failed (server may be offline): {e.Message}");
                MainThreadManager.Run(() => RegisterFailed?.Invoke());
            }
        }

        public static async Task OnlineRequest()
        {
            var url = $"https://bvonline.eulenet.eu/online";

            try
            {
                using var client = CreateClient();
                var jsonData = JsonUtility.ToJson(new TokenData { token = Token });
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                await client.PostAsync(url, content);
                OnlineStatusHelper.SetLoginOnline();
            }
            catch (Exception)
            {
                Debug.LogWarning("Online request failed — server unreachable");
                OnlineStatusHelper.SetLoginOffline();
            }
        }

        public static async Task QueueRequest()
        {
            string url = $"https://bvonline.eulenet.eu/queue";

            try
            {
                using var client = CreateClient();
                var jsonData = JsonUtility.ToJson(new TokenData { token = Token });
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                await client.PostAsync(url, content);
            }
            catch (Exception)
            {
                // Server unreachable — already handled by OnlineRequest
            }
        }

        public static async Task PlayerRequest()
        {
            var url = $"https://bvonline.eulenet.eu/info";

            try
            {
                using var client = CreateClient();
                var response = await client.GetAsync(url);
                if (response != null && response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    var node = JsonUtility.FromJson<InfoResponse>(json);

                    var playersOnline = int.Parse(node.online);
                    var playersQueue = int.Parse(node.queue);

                    QueueInfoChanged?.Invoke(playersOnline, playersQueue);
                }
            }
            catch (Exception)
            {
                Debug.LogWarning("Player info request failed — server unreachable");
            }
        }

        #endregion

        #region JSON DTOs

        [Serializable]
        private struct LoginResponse
        {
            public string token;
        }

        [Serializable]
        private struct UserDataResponse
        {
            public string username;
            public string email;
            public int elo;
            public float color_r;
            public float color_g;
            public float color_b;
            public int hat_id;
            public int eyes_id;
            public int mouth_id;
        }

        [Serializable]
        private struct ColorData
        {
            public float color_r;
            public float color_g;
            public float color_b;
        }

        [Serializable]
        private struct TokenData
        {
            public string token;
        }

        [Serializable]
        private struct InfoResponse
        {
            public string online;
            public string queue;
        }

        #endregion
    }
}
