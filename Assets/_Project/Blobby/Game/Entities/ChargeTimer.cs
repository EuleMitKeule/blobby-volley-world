using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blobby.Game.Entities
{
    public class ChargeTimer
    {
        public event Action ChargeTimerStarted;
        public event Action ChargeTimerTicked;
        public event Action ChargeTimerStopped;

        CancellationTokenSource _tokenSource;

        const float jumpChargeTime = 0.04f;

        public ChargeTimer()
        {
            _tokenSource = new CancellationTokenSource();
        }

        async Task Timer()
        {
            ChargeTimerStarted?.Invoke();

            await Task.Delay((int)(jumpChargeTime * 1000));
            do
            {
                if (_tokenSource.IsCancellationRequested)
                {
                    ChargeTimerStopped?.Invoke();
                    return;
                }

                ChargeTimerTicked?.Invoke();
                await Task.Delay((int)(jumpChargeTime * 1000));
            }
            while (true);
        }

        public void Start()
        {
            _tokenSource = new CancellationTokenSource();

            Task.Run(Timer);
        }

        public void Stop()
        {
            _tokenSource.Cancel();
        }
    }
}
