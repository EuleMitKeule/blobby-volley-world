using System;
using TMPro;
using UnityEngine;

namespace Blobby.UserInterface.Components
{
    public class InputUsername : MonoBehaviour
    {
        public static event Action<string> EndEdit;
        

        public void OnEndEdit(string value) => EndEdit?.Invoke(value);
    }
}