using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blobby.Game.Time
{
    public class MatchTimer
    {
        public int MatchTime { get; private set; }

        public event Action MatchTimerStarted;
        public event Action<int> MatchTimerTicked;
        public event Action MatchTimerStopped;

        CancellationTokenSource _tokenSource;

        public MatchTimer()
        {

        }

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
            MatchTimerStarted?.Invoke();

            try
            {
                await Task.Delay(100, _tokenSource.Token);
            }
            catch (TaskCanceledException) { }

            do
            {
                if (UnityEngine.Time.timeScale != 0f)
                {
                    if (_tokenSource.IsCancellationRequested)
                    {
                        MatchTimerStopped?.Invoke();
                        return;
                    }

                    MatchTimerTicked?.Invoke(++MatchTime);
                }

                try
                {
                    await Task.Delay(100, _tokenSource.Token);
                }
                catch (TaskCanceledException) { }
            }
            while (true);
        }
    }
}
