using UnityEngine;

namespace Blobby.Game
{
    public static class PrefabHelper
    {
        public static GameObject LocalPlayer => 
            Resources.Load<GameObject>("Prefabs/player/local_player");

        public static GameObject AiPlayer =>
            Resources.Load<GameObject>("Prefabs/player/ai_player");

        public static GameObject Ball =>
            Resources.Load<GameObject>("Prefabs/ball/local_ball");

        public static GameObject Bomb =>
            Resources.Load<GameObject>("Prefabs/ball/local_bomb");

        public static GameObject LocalMatch =>
            Resources.Load<GameObject>("Prefabs/match/local_match");

        public static GameObject AiMatch =>
            Resources.Load<GameObject>("Prefabs/match/ai_match");

        public static GameObject OnlineMatch =>
            Resources.Load<GameObject>("Prefabs/match/online_match");

        public static GameObject ClientMatch =>
            Resources.Load<GameObject>("Prefabs/match/client_match");
    }
}