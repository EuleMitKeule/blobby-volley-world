using System;
using UnityEngine;

namespace Blobby.UserInterface.Components
{
    public class ButtonControlsRight : MonoBehaviour
    {
        public static event Action<Control> Clicked;

        public void OnClick() => Clicked?.Invoke(Control.Right);
    }
}