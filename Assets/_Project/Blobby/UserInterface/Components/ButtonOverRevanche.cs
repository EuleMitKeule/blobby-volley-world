using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blobby.UserInterface.Components
{
    public class ButtonOverRevanche : MonoBehaviour
    {
        public static event Action Clicked;

        public void OnClick() => Clicked?.Invoke();
    }
}
