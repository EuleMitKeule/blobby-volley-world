using System;
using UnityEngine;

namespace Blobby.UserInterface.Components
{
    public class InputPass : MonoBehaviour
    {
        public static event Action Changed;

        public void OnChange() => Changed?.Invoke();
    }
}