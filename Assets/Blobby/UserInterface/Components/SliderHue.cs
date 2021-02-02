using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Blobby.UserInterface.Components
{
    public class SliderHue : MonoBehaviour
    {
        public static event Action<float> ValueChanged;

        public void OnValueChanged(float value) => ValueChanged?.Invoke(value);
    }
}
