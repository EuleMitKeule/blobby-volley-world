using Blobby.Models;
using UnityEngine;

namespace Blobby.Game.Entities
{
    public class LocalPlayer : Player
    {
        static GameObject Prefab => PrefabHelper.LocalPlayer;

        public LocalPlayer(LocalMatch match, PlayerData playerData) : base(match, playerData, Prefab)
        {
            SubscribeControls();
        }

        public override void Dispose()
        {
            base.Dispose();
            
            UnsubscribeControls();
        }

        void SubscribeControls()
        {
            InputHelper.SetDownCallback(OnJumpDown, PlayerData.PlayerNum, 0);
            InputHelper.SetUpCallback(OnJumpUp, PlayerData.PlayerNum, 0);

            InputHelper.SetDownCallback(OnLeftDown, PlayerData.PlayerNum, 1);
            InputHelper.SetUpCallback(OnLeftUp, PlayerData.PlayerNum, 1);

            InputHelper.SetDownCallback(OnRightDown, PlayerData.PlayerNum, 2);
            InputHelper.SetUpCallback(OnRightUp, PlayerData.PlayerNum, 2);
        }

        void UnsubscribeControls()
        {
            InputHelper.SetDownCallback(null, PlayerData.PlayerNum, 0);
            InputHelper.SetUpCallback(null, PlayerData.PlayerNum, 0);

            InputHelper.SetDownCallback(null, PlayerData.PlayerNum, 1);
            InputHelper.SetUpCallback(null, PlayerData.PlayerNum, 1);

            InputHelper.SetDownCallback(null, PlayerData.PlayerNum, 2);
            InputHelper.SetUpCallback(null, PlayerData.PlayerNum, 2);
        }
    }
}
