using System;
using UnityEngine;

namespace Blobby.UserInterface.Components
{
    public class ToggleController : MonoBehaviour
    {
        public static event Action Toggled;

        public void OnToggle() => Toggled?.Invoke();
    }
}