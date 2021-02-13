using System;
using TMPro;
using UnityEngine;

namespace Blobby.UserInterface.Components
{
    public class InputUsername : MonoBehaviour
    {
        public static event Action<string> Changed;
        public static event Action EndEdit;
        
        public void OnValueChanged(string username) => Changed?.Invoke(username);

        public void OnEndEdit() => EndEdit?.Invoke();
    }
}