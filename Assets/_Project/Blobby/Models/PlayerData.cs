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
        public event Action PlayerNumChanged;

        private int _playerNum;
        public int PlayerNum
        {
            get { return _playerNum; }
            set { _playerNum = value; PlayerNumChanged?.Invoke(); }
        }

        public string Name;
        public Color Color;

        public PlayerData(int playerNum, string name, Color color) =>
            (PlayerNum, Name, Color) = (playerNum, name, color);

        public Side Side { get { return PlayerNum % 2 == 0 ? Side.Left : Side.Right; } }

        public static PlayerData None() => new PlayerData(-1, "", Color.gray);
    }
}
