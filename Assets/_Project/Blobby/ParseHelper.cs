using Blobby.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static BeardedManStudios.Forge.Networking.MasterServerResponse;

namespace Blobby
{
    public static class ParseHelper
    {
        public static List<ServerData> ParseServerList(List<Server> servers)
        {
            var serverDatas = new List<ServerData>();

            foreach (var server in servers)
            {
                serverDatas.Add(ParseHelper.ParseServer(server));
            }

            return serverDatas;
        }

        public static ServerData ParseServer(Server server)
        {
            var serverData = new ServerData
            {
                Host = server.Address,
                Port = server.Port,
                PlayerCount = server.PlayerCount,
                Name = server.Name,
                IsRanked = server.Type == "official"
            };

            var matchData = new MatchData();
            serverData.MatchData = matchData;

            string[] modeParse = server.Mode.Split(' ');

            Enum.TryParse(modeParse[0], out matchData.Map);
            Enum.TryParse(modeParse[1], out matchData.GameMode);
            Enum.TryParse(modeParse[2], out matchData.PlayerMode);
            Enum.TryParse(modeParse[3], out matchData.JumpMode);
            bool.TryParse(modeParse[4], out matchData.JumpOverNet);
            float.TryParse(modeParse[5], out matchData.TimeScale);

            return serverData;
        }
    }
}
