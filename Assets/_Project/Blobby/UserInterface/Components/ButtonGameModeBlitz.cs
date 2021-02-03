using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blobby
{
    public class ButtonGameModeBlitz : MonoBehaviour
    {
        public static event Action Clicked;

        public void OnClick() => Clicked?.Invoke();
    }
}
