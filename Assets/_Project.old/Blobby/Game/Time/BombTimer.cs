using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blobby.Game.Time
{
    public class BombTimer
    {
        public event Action BombTimerStarted;
        public event Action BombTimerTicked;
        public event Action BombTimerStopped;

        CancellationTokenSource _tokenSource;
        static int _bombTime;
        static float _bombSpeed;

        const int _bombStartTime = 42;
        const float _bombStartSpeed = 1f;
        const float _bombSpeedUp = 0.95f;
        const float _bombSpeedMin = 0.15f;

        public BombTimer()
        {
            _tokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            _tokenSource = new CancellationTokenSource();
            _bombTime = _bombStartTime;
            _bombSpeed = _bombStartSpeed;

            Task.Run(Timer);
        }

        public void Stop()
        {
            _tokenSource?.Cancel();
        }

        async Task Timer()
        {
            BombTimerStarted?.Invoke();

            do
            {
                for (int i = 0; i < 10; i++)
                {
                    if (_tokenSource.IsCancellationRequested)
                    {
                        BombTimerStopped?.Invoke();
                        return;
                    }

                    await Task.Delay((int)(_bombSpeed * 100), _tokenSource.Token);
                }

                if (!_tokenSource.IsCancellationRequested) BombTimerTicked?.Invoke();
                IncreaseSpeed();
            }
            while (--_bombTime > 0);

            if (!_tokenSource.IsCancellationRequested) BombTimerStopped?.Invoke();
        }

        void IncreaseSpeed()
        {
            _bombSpeed *= _bombSpeedUp;

            if (_bombSpeed < _bombSpeedMin)
                _bombSpeed = _bombSpeedMin;
        }
    }
}
