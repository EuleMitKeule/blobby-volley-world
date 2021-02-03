using System;
using UnityEngine;

namespace Blobby.UserInterface.Components
{
    public class SliderSpeed : MonoBehaviour
    {
        public static event Action<int> Changed;

        public void OnChange(float value) => Changed?.Invoke((int)value);
    }
}