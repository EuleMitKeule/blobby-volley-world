using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Blobby.Game
{
    public class ServerCloseTimer
    {
        public event Action ServerCloseTimerStopped;

        CancellationTokenSource _tokenSource;

        public async void Start()
        {
            if (_tokenSource == null || _tokenSource.IsCancellationRequested)
            {
                _tokenSource = new CancellationTokenSource();

                await Timer();
            }
        }

        public void Stop()
        {
            _tokenSource?.Cancel();
        }

        async Task Timer()
        {
            try
            {
                await Task.Delay(15000, _tokenSource.Token);

                _tokenSource.Cancel();

                ServerCloseTimerStopped?.Invoke();
            }
            catch (TaskCanceledException)
            { }
        }
    }
}
