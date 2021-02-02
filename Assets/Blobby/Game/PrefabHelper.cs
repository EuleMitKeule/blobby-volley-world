using UnityEngine;

namespace Blobby.Game
{
    public static class PrefabHelper
    {
        public static GameObject LocalPlayer => 
            Resources.Load<GameObject>("Prefabs/player/player");

        public static GameObject OnlinePlayer =>
            Resources.Load<GameObject>("Prefabs/player/online_player");
    }
}