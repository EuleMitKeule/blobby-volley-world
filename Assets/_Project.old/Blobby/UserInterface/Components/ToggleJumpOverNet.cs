using System;
using UnityEngine;

namespace Blobby.UserInterface.Components
{
    public class ToggleJumpOverNet : MonoBehaviour
    {
        public static event Action<bool> Toggled;

        public void OnToggle(bool value) => Toggled?.Invoke(value);
    }
}