using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Blobby.Models
{
    public class PlayerData
    {
        public int PlayerNum { get; }
        public string Name { get; }
        public Color Color { get; }

        public PlayerData(int playerNum, string name, Color color) =>
            (PlayerNum, Name, Color) = (playerNum, name, color);

        public Side Side => PlayerNum % 2 == 0 ? Side.Left : Side.Right;
    }
}
