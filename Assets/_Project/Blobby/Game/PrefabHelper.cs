using UnityEngine;

namespace Blobby.Game
{
    public static class PrefabHelper
    {
        public static GameObject LocalPlayer => 
            Resources.Load<GameObject>("Prefabs/player/player");

        public static GameObject OnlinePlayer =>
            Resources.Load<GameObject>("Prefabs/player/online_player");

        public static GameObject AiPlayer =>
            Resources.Load<GameObject>("Prefabs/player/ai_player");

        public static GameObject Ball =>
            Resources.Load<GameObject>("Prefabs/ball/ball");

        public static GameObject Bomb =>
            Resources.Load<GameObject>("Prefabs/ball/bomb");

        public static GameObject LocalMatch =>
            Resources.Load<GameObject>("Prefabs/match/local_match");
    }
}