using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Blobby.Components
{
    public class TimeComponent : MonoBehaviour
    {
        public static event Action FixedUpdateTicked;
        public static event Action UpdateTicked;
        public static event Action LateUpdateTicked;

        void Awake()
        {
            Application.targetFrameRate = 144;
        }

        void FixedUpdate()
        {
            FixedUpdateTicked?.Invoke();
        }

        void Update()
        {
            UpdateTicked?.Invoke();
        }

        void LateUpdate()
        {
            LateUpdateTicked?.Invoke();
        }

        void OnApplicationQuit()
        {
            ThreadHelper.Stop();    
        }
    }
}
