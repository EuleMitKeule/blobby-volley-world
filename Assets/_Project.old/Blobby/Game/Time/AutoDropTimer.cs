using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blobby.Game.Time
{
    public class AutoDropTimer
    {
        public event Action AutoDropTimerStarted;
        public event Action<int> AutoDropTimerTicked;
        public event Action AutoDropTimerStopped;

        CancellationTokenSource _tokenSource;

        public AutoDropTimer()
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
            AutoDropTimerStarted?.Invoke();

            for (int i = 0; i < 200; i++)
            {
                if (_tokenSource.IsCancellationRequested) return;

                await Task.Delay(10, _tokenSource.Token);

                if (!_tokenSource.IsCancellationRequested) AutoDropTimerTicked?.Invoke(i * 10);
            }

            if (!_tokenSource.IsCancellationRequested)
            {
                AutoDropTimerStopped?.Invoke();
            }
        }
    }
}
