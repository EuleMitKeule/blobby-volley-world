using System;
using UnityEngine;

namespace Blobby.UserInterface.Components
{
    public class SliderVolume : MonoBehaviour
    {
        public static event Action<float> ValueChanged;

        public void OnValueChanged(float value) => ValueChanged?.Invoke(value);
    }
}
