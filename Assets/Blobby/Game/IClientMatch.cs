using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blobby.Game
{
    public interface IClientMatch : IMatch
    {
        public bool Switched { get; }

        public event Action<int, int, Side> ScoreChanged;
        public event Action<int> TimeChanged;
    }
}
