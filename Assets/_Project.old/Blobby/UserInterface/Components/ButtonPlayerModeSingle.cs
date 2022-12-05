using System;
using UnityEngine;

namespace Blobby.UserInterface.Components
{
    public class ButtonPlayerModeSingle : MonoBehaviour
    {
        public static event Action Clicked;

        public void OnClick() => Clicked?.Invoke();
    }
}