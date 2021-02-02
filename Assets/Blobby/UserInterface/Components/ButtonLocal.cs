﻿using System;
using UnityEngine;

namespace Blobby.UserInterface.Components
{
    public class ButtonLocal : MonoBehaviour
    {
        public static event Action Clicked;

        public void OnClick() => Clicked?.Invoke();
    }
}