using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BlobbyVolleyWorld.Match
{
    public class MatchTimeComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        [ReadOnly]
        public int Time { get; private set; }
        
        [ShowInInspector]
        [ReadOnly]
        public bool IsRunning { get; private set; }
        
        public event EventHandler<float> TimeChanged;

        Coroutine TimerCoroutine { get; set; }
        
        void Awake()
        {
            TimerCoroutine = StartCoroutine(UpdateTimer());
        }
        
        public void StartTimer()
        {
            IsRunning = true;
        }
        
        public void PauseTimer()
        {
            IsRunning = false;
        }
        
        public void ResetTimer()
        {
            IsRunning = false;
            Time = 0;
            TimeChanged?.Invoke(this, Time);
        }
        
        public void RestartTimer()
        {
            PauseTimer();
            ResetTimer();
            StartTimer();
        }
        
        IEnumerator UpdateTimer()
        {
            var timeout = new WaitForSeconds(1f);
            
            while (gameObject.activeSelf)
            {
                if (!IsRunning)
                {
                    yield return timeout;
                    continue;
                }
                
                Time += 1;
                TimeChanged?.Invoke(this, Time);
                yield return timeout;
            }
        }
    }
}