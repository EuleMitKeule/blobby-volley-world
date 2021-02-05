using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blobby.Game
{
    public interface IMatch
    {
        public event Action MatchStarted;
        public event Action MatchStopped;
    }
}
