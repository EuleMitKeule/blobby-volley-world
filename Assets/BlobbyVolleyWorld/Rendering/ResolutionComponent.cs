using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace BlobbyVolleyWorld.Rendering
{
    public class ResolutionComponent : MonoBehaviour
    {
        public ResolutionChangedEvent resolutionChangedEvent;
        [Serializable]
        public class ResolutionChangedEvent : UnityEvent<int, int, bool> { }

        [SerializeField]
        bool allowFullscreen = true;

        [SerializeField]
        float aspectRatioWidth  = 16;
        [SerializeField]
        float aspectRatioHeight = 9;

        [SerializeField]
        int minWidthPixel  = 512;
        [SerializeField]
        int minHeightPixel = 512;
        [SerializeField]
        int maxWidthPixel  = 2048;
        [SerializeField]
        int maxHeightPixel = 2048;

        float aspect;
        int setWidth  = -1;
        int setHeight = -1;
        bool wasFullscreenLastFrame;
        bool started;
        int pixelHeightOfCurrentScreen;
        int pixelWidthOfCurrentScreen;
        bool quitStarted;

        #region WINAPI

        const int WM_SIZING = 0x214;

        const int WMSZ_LEFT    = 1;
        const int WMSZ_RIGHT   = 2;
        const int WMSZ_TOP     = 3;
        const int WMSZ_BOTTOM  = 6;
        const int GWLP_WNDPROC = -4;

        delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        WndProcDelegate wndProcDelegate;

        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        static extern bool EnumThreadWindows(uint dwThreadId, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hwnd, ref RECT lpRect);

        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, ref RECT lpRect);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
        static extern IntPtr SetWindowLong32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)]
        static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        const string UNITY_WND_CLASSNAME = "UnityWndClass";

        IntPtr unityHWnd;

        IntPtr oldWndProcPtr;

        IntPtr newWndProcPtr;

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        #endregion

        void Start()
        {
            if (Application.isEditor)
            {
                return;
            }

            Application.wantsToQuit += ApplicationWantsToQuit;

            EnumThreadWindows(GetCurrentThreadId(), (hWnd, lParam) =>
            {
                var classText = new StringBuilder(UNITY_WND_CLASSNAME.Length + 1);
                GetClassName(hWnd, classText, classText.Capacity);

                if (classText.ToString() == UNITY_WND_CLASSNAME)
                {
                    unityHWnd = hWnd;
                    return false;
                }
                return true;
            }, IntPtr.Zero);

            SetAspectRatio(aspectRatioWidth, aspectRatioHeight, true);

            wasFullscreenLastFrame = Screen.fullScreen;
            wndProcDelegate = wndProc;
            newWndProcPtr = Marshal.GetFunctionPointerForDelegate(wndProcDelegate);
            oldWndProcPtr = SetWindowLong(unityHWnd, GWLP_WNDPROC, newWndProcPtr);

            started = true;
        }

        public void SetAspectRatio(float newAspectWidth, float newAspectHeight, bool apply)
        {
            aspectRatioWidth = newAspectWidth;
            aspectRatioHeight = newAspectHeight;
            aspect = aspectRatioWidth / aspectRatioHeight;

            if (apply)
            {
                Screen.SetResolution(Screen.width, Mathf.RoundToInt(Screen.width / aspect), Screen.fullScreen);
            }
        }

        IntPtr wndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == WM_SIZING)
            {
                RECT rc = (RECT)Marshal.PtrToStructure(lParam, typeof(RECT));

                RECT windowRect = new RECT();
                GetWindowRect(unityHWnd, ref windowRect);

                RECT clientRect = new RECT();
                GetClientRect(unityHWnd, ref clientRect);

                int borderWidth = windowRect.Right - windowRect.Left - (clientRect.Right - clientRect.Left);
                int borderHeight = windowRect.Bottom - windowRect.Top - (clientRect.Bottom - clientRect.Top);

                rc.Right -= borderWidth;
                rc.Bottom -= borderHeight;

                int newWidth = Mathf.Clamp(rc.Right - rc.Left, minWidthPixel, maxWidthPixel);
                int newHeight = Mathf.Clamp(rc.Bottom - rc.Top, minHeightPixel, maxHeightPixel);

                switch (wParam.ToInt32())
                {
                    case WMSZ_LEFT:
                        rc.Left = rc.Right - newWidth;
                        rc.Bottom = rc.Top + Mathf.RoundToInt(newWidth / aspect);
                        break;
                    case WMSZ_RIGHT:
                        rc.Right = rc.Left + newWidth;
                        rc.Bottom = rc.Top + Mathf.RoundToInt(newWidth / aspect);
                        break;
                    case WMSZ_TOP:
                        rc.Top = rc.Bottom - newHeight;
                        rc.Right = rc.Left + Mathf.RoundToInt(newHeight * aspect);
                        break;
                    case WMSZ_BOTTOM:
                        rc.Bottom = rc.Top + newHeight;
                        rc.Right = rc.Left + Mathf.RoundToInt(newHeight * aspect);
                        break;
                    case WMSZ_RIGHT + WMSZ_BOTTOM:
                        rc.Right = rc.Left + newWidth;
                        rc.Bottom = rc.Top + Mathf.RoundToInt(newWidth / aspect);
                        break;
                    case WMSZ_RIGHT + WMSZ_TOP:
                        rc.Right = rc.Left + newWidth;
                        rc.Top = rc.Bottom - Mathf.RoundToInt(newWidth / aspect);
                        break;
                    case WMSZ_LEFT + WMSZ_BOTTOM:
                        rc.Left = rc.Right - newWidth;
                        rc.Bottom = rc.Top + Mathf.RoundToInt(newWidth / aspect);
                        break;
                    case WMSZ_LEFT + WMSZ_TOP:
                        rc.Left = rc.Right - newWidth;
                        rc.Top = rc.Bottom - Mathf.RoundToInt(newWidth / aspect);
                        break;
                }

                setWidth = rc.Right - rc.Left;
                setHeight = rc.Bottom - rc.Top;

                rc.Right += borderWidth;
                rc.Bottom += borderHeight;

                resolutionChangedEvent.Invoke(setWidth, setHeight, Screen.fullScreen);

                Marshal.StructureToPtr(rc, lParam, true);
            }

            return CallWindowProc(oldWndProcPtr, hWnd, msg, wParam, lParam);
        }

        void Update()
        {
            if (!allowFullscreen && Screen.fullScreen)
            {
                Screen.fullScreen = false;
            }

            if (Screen.fullScreen && !wasFullscreenLastFrame)
            {
                int height;
                int width;

                bool blackBarsLeftRight = aspect < (float) pixelWidthOfCurrentScreen / pixelHeightOfCurrentScreen;

                if (blackBarsLeftRight) {
                    height = pixelHeightOfCurrentScreen;
                    width = Mathf.RoundToInt(pixelHeightOfCurrentScreen * aspect);
                }
                else
                {
                    width = pixelWidthOfCurrentScreen;
                    height = Mathf.RoundToInt(pixelWidthOfCurrentScreen / aspect);
                }

                Screen.SetResolution(width, height, true);
                resolutionChangedEvent.Invoke(width, height, true);
            }
            else if (!Screen.fullScreen && wasFullscreenLastFrame)
            {
                Screen.SetResolution(setWidth, setHeight, false);
                resolutionChangedEvent.Invoke(setWidth, setHeight, false);
            }
            else if (!Screen.fullScreen && setWidth != -1 && setHeight != -1 && (Screen.width != setWidth || Screen.height != setHeight))
            {
                setHeight = Screen.height;
                setWidth = Mathf.RoundToInt(Screen.height * aspect);

                Screen.SetResolution(setWidth, setHeight, Screen.fullScreen);
                resolutionChangedEvent.Invoke(setWidth, setHeight, Screen.fullScreen);
            }
            else if (!Screen.fullScreen)
            {
                pixelHeightOfCurrentScreen = Screen.currentResolution.height;
                pixelWidthOfCurrentScreen = Screen.currentResolution.width;
            }

            wasFullscreenLastFrame = Screen.fullScreen;

#if UNITY_EDITOR
            if (Screen.width != setWidth || Screen.height != setHeight)
            {
                setWidth = Screen.width;
                setHeight = Screen.height;
                resolutionChangedEvent.Invoke(setWidth, setHeight, Screen.fullScreen);
            }
#endif
        }

        static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 4)
            {
                return SetWindowLong32(hWnd, nIndex, dwNewLong);
            }
            return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
        }

        bool ApplicationWantsToQuit()
        {
            if (!started)
                return false;

            if (!quitStarted)
            {
                StartCoroutine("DelayedQuit");
                return false;
            }

            return true;
        }

        IEnumerator DelayedQuit()
        {
            SetWindowLong(unityHWnd, GWLP_WNDPROC, oldWndProcPtr);

            yield return new WaitForEndOfFrame();

            quitStarted = true;
            Application.Quit();
        }
    }
}
