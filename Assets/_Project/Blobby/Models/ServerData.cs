using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blobby.Models
{
    [Serializable]
    public class ServerData
    {
        public string Name;
        public string Host;
        public ushort Port;
        public string Token;
        public string MasterServerHost;
        public ushort MasterServerPort;

        public MatchData MatchData { get; set; }

        public int PlayerCount { get; set; }

        public int MaxPlayerCount
        {
            get
            {
                if (MatchData == null) return 0;
                else return MatchData.PlayerCount;
            }
        }
    }
}
