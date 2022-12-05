using System;
using BlobbyVolleyWorld.Match;
using Sirenix.Serialization;
using Unity.Collections;
using UnityEngine;

namespace BlobbyVolleyWorld.Settings
{
    [Serializable]
    public struct ClientSettings
    {
        [OdinSerialize]
        public Side Side { get; private set; }
        
        [OdinSerialize]
        public int PlayerNum { get; private set; }
        
        [OdinSerialize]
        public Color FirstColor { get; private set; }
        
        [OdinSerialize]
        public Color SecondColor { get; private set; }
        
        [OdinSerialize]
        public Color ThirdColor { get; private set; }
        
        [OdinSerialize]
        public Color FourthColor { get; private set; }
        
        [OdinSerialize]
        public bool IsWindowed { get; private set; }
        
        [OdinSerialize]
        public float Volume { get; private set; }
        
        [OdinSerialize]
        public string Username { get; private set; }
        
        public ClientSettings(
            Side side, 
            int playerNum, 
            Color firstColor,
            Color secondColor,
            Color thirdColor,
            Color fourthColor,
            bool isWindowed, 
            float volume,
            string username)
        {
            Side = side;
            PlayerNum = playerNum;
            FirstColor = firstColor;
            SecondColor = secondColor;
            ThirdColor = thirdColor;
            FourthColor = fourthColor;
            IsWindowed = isWindowed;
            Volume = volume;
            Username = username;
        }
        
        public ClientSettings WithSide(Side side) => 
            new(side, PlayerNum, FirstColor, SecondColor, ThirdColor, FourthColor, IsWindowed, Volume, Username);

        public ClientSettings WithPlayerNum(int playerNum) => 
            new(Side, playerNum, FirstColor, SecondColor, ThirdColor, FourthColor, IsWindowed, Volume, Username);
        
        public ClientSettings WithIsWindowed(bool isWindowed) =>
            new(Side, PlayerNum, FirstColor, SecondColor, ThirdColor, FourthColor, isWindowed, Volume, Username);
        
        public ClientSettings WithFirstColor(Color firstColor) =>
            new(Side, PlayerNum, firstColor, SecondColor, ThirdColor, FourthColor, IsWindowed, Volume, Username);
        
        public ClientSettings WithSecondColor(Color secondColor) =>
            new(Side, PlayerNum, FirstColor, secondColor, ThirdColor, FourthColor, IsWindowed, Volume, Username);
        
        public ClientSettings WithThirdColor(Color thirdColor) =>
            new(Side, PlayerNum, FirstColor, SecondColor, thirdColor, FourthColor, IsWindowed, Volume, Username);
        
        public ClientSettings WithFourthColor(Color fourthColor) =>
            new(Side, PlayerNum, FirstColor, SecondColor, ThirdColor, fourthColor, IsWindowed, Volume, Username);
        
        public ClientSettings WithColor(Color color, int index) =>
            index switch
            {
                0 => WithFirstColor(color),
                1 => WithSecondColor(color),
                2 => WithThirdColor(color),
                3 => WithFourthColor(color),
                _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
            };

        public ClientSettings WithVolume(float volume) =>
            new(Side, PlayerNum, FirstColor, SecondColor, ThirdColor, FourthColor, IsWindowed, volume, Username);
        
        public ClientSettings WithUsername(string username) =>
            new(Side, PlayerNum, FirstColor, SecondColor, ThirdColor, FourthColor, IsWindowed, Volume, username);

        public Color GetColor(int index) =>
            index switch
            {
                0 => FirstColor,
                1 => SecondColor,
                2 => ThirdColor,
                3 => FourthColor,
                _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
            };
        
        public static ClientSettings Default => 
            new(Side.Left, 0, Color.blue, Color.red, Color.green, Color.yellow, false, 1f, "");
    }
}