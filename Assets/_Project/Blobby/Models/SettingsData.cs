using UnityEngine;

namespace Blobby.Models
{
    [System.Serializable]
    public class SettingsData
    {
        public Side Side;

        public ControlsData[] Controls =
        {
            new ControlsData(1),
            new ControlsData(2),
            new ControlsData(3),
            new ControlsData(4)
        };

        public Color[] Colors =
        {
            Color.blue,
            Color.red,
            Color.green,
            Color.yellow
        };

        public float Volume;

        public bool Windowed;
    }
}
