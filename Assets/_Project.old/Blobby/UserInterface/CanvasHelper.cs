using Blobby.Game;
using UnityEngine;

namespace Blobby.UserInterface
{
    public static class CanvasHelper
    {
        static GameObject _canvasGame;

        // [RuntimeInitializeOnLoadMethod]
        public static void Initialize()
        {
            _canvasGame = GameObject.Find("canvas_game");
        }

        public static void SetCanvasGame(bool value)
        {
            _canvasGame.gameObject.SetActive(value);
        }
    }
}
