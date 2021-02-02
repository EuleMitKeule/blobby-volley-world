using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Blobby.Game
{
    public class ResetBallTimer
    {
        public event Action ResetBallTimerStarted;
        public event Action ResetBallTimerStopped;

        CancellationTokenSource _tokenSource;

        public ResetBallTimer()
        {
            _tokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            _tokenSource = new CancellationTokenSource();

            Task.Run(Timer);
        }

        public void Stop()
        {
            _tokenSource?.Cancel();
        }

        async Task Timer()
        {
            ResetBallTimerStarted?.Invoke();

            for (int i = 0; i < 150; i++)
            {
                if (_tokenSource.IsCancellationRequested) return;

                await Task.Delay(10, _tokenSource.Token);
            }

            if (!_tokenSource.IsCancellationRequested) ResetBallTimerStopped?.Invoke();
        }
    }
}
