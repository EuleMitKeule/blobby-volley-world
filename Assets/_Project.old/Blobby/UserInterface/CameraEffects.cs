using Blobby.Game;
using Blobby.UserInterface;
using Blobby.UserInterface.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Blobby.UserInterface
{
    public static class CameraEffects
    {
        static SlideEffect _slideEffect;

        // [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            _slideEffect = new SlideEffect(null, null);

            ButtonBrowser.Clicked += () => CameraSlide(true);
            ButtonLocal.Clicked += () => CameraSlide(true);
            ButtonSettings.Clicked += () => CameraSlide(true);
            ButtonBrowserBack.Clicked += () => CameraSlide(false);
            ButtonLocalBack.Clicked += () => CameraSlide(false);
            ButtonSettingsBack.Clicked += () => CameraSlide(false);
            ButtonSettingsSave.Clicked += () => CameraSlide(false);
        }

        public static void CameraSlide(bool isLeft)
        {
            _slideEffect?.Slide(isLeft ? Side.Left : Side.Right);
        }
    }
}
