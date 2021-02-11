using System;
using Blobby.Models;

namespace Blobby.Game.Entities
{
    public interface IPlayerGraphicsProvider
    {
        bool IsRunning { get; }
        bool IsGrounded { get; }
        bool IsInvisible { get; }

        public event Action<PlayerData> PlayerDataChanged;
        public event Action<int, bool> AlphaChanged;
    }
}