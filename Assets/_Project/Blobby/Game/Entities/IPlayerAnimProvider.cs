using System;
using Blobby.Models;

namespace Blobby.Game.Entities
{
    public interface IPlayerAnimProvider
    {
        bool IsRunning { get; }
        bool IsGrounded { get; }
        bool IsInvisible { get; }

        public event Action<PlayerData> PlayerDataChanged;
    }
}