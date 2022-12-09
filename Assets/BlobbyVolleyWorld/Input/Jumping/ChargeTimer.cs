using System;
using System.Threading;
using System.Threading.Tasks;
using BlobbyVolleyWorld.Game;

namespace BlobbyVolleyWorld.Entities.Input.Jumping
{
    public class ChargeTimer
    {
        public event Action ChargeTimerStarted;
        public event Action ChargeTimerTicked;
        public event Action ChargeTimerStopped;

        CancellationTokenSource tokenSource;

        GameComponent GameComponent { get; }
        
        public ChargeTimer()
        {
            GameComponent = UnityEngine.Object.FindObjectOfType<GameComponent>();
            tokenSource = new CancellationTokenSource();
        }

        async Task Timer()
        {
            ChargeTimerStarted?.Invoke();

            var delay = (int)(GameComponent.PhysicsAsset.JumpChargeTime * 1000f);
            
            await Task.Delay(delay);
            do
            {
                if (tokenSource.IsCancellationRequested)
                {
                    ChargeTimerStopped?.Invoke();
                    return;
                }

                ChargeTimerTicked?.Invoke();
                await Task.Delay(delay);
            }
            while (true);
        }

        public void Start()
        {
            tokenSource = new CancellationTokenSource();

            Task.Run(Timer);
        }

        public void Stop()
        {
            tokenSource.Cancel();
        }
    }
}
