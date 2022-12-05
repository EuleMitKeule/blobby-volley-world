using System.Collections.Generic;
using UnityEngine;

namespace Blobby.Models
{
    [System.Serializable]
    public class UserData
    {
        public string name;
        public string email;
        public string password;
        public int elo;
        public int hatID;
        public int eyesID;
        public int mouthID;
        public float colorR;
        public float colorG;
        public float colorB;
        public string token;
        public List<int> inventory = new List<int>();

        public Color Color { get { return new Color(colorR, colorG, colorB); } }
    }
}
