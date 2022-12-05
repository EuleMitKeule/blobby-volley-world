using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Game;
using Blobby.Models;
using Blobby.Networking;
using TMPro;
using UnityEngine;


namespace Blobby.UserInterface
{
    public class MatchScore
    {
        IClientMatch _match;
        PlayerData _leftPlayerData;
        PlayerData _rightPlayerData;

        public MatchScore(IClientMatch match, PlayerData leftPlayerData, PlayerData rightPlayerData)
        {
            _leftPlayerData = leftPlayerData;
            _rightPlayerData = rightPlayerData;

            _match = match;

            SetScore(0, 0, Side.None);
            SetTime(0);

            SubscribeEventHandler();

            Apply();
        }

        public void SetLeftPlayerData(PlayerData playerData)
        {
            _leftPlayerData = playerData;
            Apply();
        }

        public void SetRightPlayerData(PlayerData playerData)
        {
            _rightPlayerData = playerData;
            Apply();
        }

        void Apply()
        {
            if (_leftPlayerData != null) SetScoreColor(_leftPlayerData.Color, Side.Left);
            if (_rightPlayerData != null) SetScoreColor(_rightPlayerData.Color, Side.Right);
        }

        void SetScore(int scoreLeft, int scoreRight, Side lastWinner)
        {
            MainThreadManager.Run(() =>
            {
                if (lastWinner == Side.None)
                {
                    MapHelper.LabelScoreLeft.text = "00";
                    MapHelper.LabelScoreRight.text = "00";
                }
                else
                {
                    var labelLeft = _match.Switched ? MapHelper.LabelScoreRight : MapHelper.LabelScoreLeft;
                    var labelRight = _match.Switched ? MapHelper.LabelScoreLeft : MapHelper.LabelScoreRight;

                    labelLeft.text = $"{scoreLeft % 100:00}";
                    labelRight.text = $"{scoreRight % 100:00}";
                    
                    var exclamationLeft = _match.Switched ? MapHelper.ExclamationRight : MapHelper.ExclamationLeft;
                    var exclamationRight = _match.Switched ? MapHelper.ExclamationLeft : MapHelper.ExclamationRight;

                    exclamationLeft.enabled = lastWinner == Side.Left;
                    exclamationRight.enabled = lastWinner != Side.Left;
                }
            });
        }

        void SetScoreColor(Color color, Side side)
        {
            MainThreadManager.Run(() =>
            {
                var panel = side == Side.Left ? (_match.Switched ? MapHelper.PanelScoreRight : MapHelper.PanelScoreLeft) :
                                                (_match.Switched ? MapHelper.PanelScoreLeft : MapHelper.PanelScoreRight);
                var light = side == Side.Left ? (_match.Switched ? MapHelper.LightScoreRight : MapHelper.LightScoreLeft) :
                                                (_match.Switched ? MapHelper.LightScoreLeft : MapHelper.LightScoreRight);
                var exclamation = side == Side.Left ? (_match.Switched ? MapHelper.ExclamationRight : MapHelper.ExclamationLeft) :
                    (_match.Switched ? MapHelper.ExclamationLeft : MapHelper.ExclamationRight);

                panel.GetComponent<SpriteRenderer>().color = color;
                light.GetComponent<UnityEngine.Rendering.Universal.Light2D>().color = color;
                exclamation.color = color;
            });
        }

        void SetTime(int time)
        {
            MainThreadManager.Run(() =>
            {
                if (MapHelper.LabelTime == null) return; //TODO: später löschen weil dann alle timer labels vorhanden sind
                MapHelper.LabelTime.text = (time / 10 / 60).ToString("00") + ":" + ((time / 10) % 60).ToString("00");
            });
        }

        void SubscribeEventHandler()
        {
            _match.ScoreChanged += SetScore;
            _match.TimeChanged += SetTime;
        }
    }
}
