using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Blobby.UserInterface
{
    public static class PanelOver
    {
        public static void Populate(string[] usernames, int[] scores, int time, Side winner, Color color)
        {
            var labelOverScoreLeft = GameObject.Find("label_over_score_left").GetComponent<TextMeshProUGUI>();
            var labelOverScoreRight = GameObject.Find("label_over_score_right").GetComponent<TextMeshProUGUI>();

            var labelOverNameLeftUpper = GameObject.Find("label_over_name_left_upper").GetComponent<TextMeshProUGUI>();
            var labelOverNameLeftLower = GameObject.Find("label_over_name_left_lower").GetComponent<TextMeshProUGUI>();
            var labelOverNameRightUpper = GameObject.Find("label_over_name_right_upper").GetComponent<TextMeshProUGUI>();
            var labelOverNameRightLower = GameObject.Find("label_over_name_right_lower").GetComponent<TextMeshProUGUI>();
            var labelOverNameLeft = GameObject.Find("label_over_name_left").GetComponent<TextMeshProUGUI>();
            var labelOverNameRight = GameObject.Find("label_over_name_right").GetComponent<TextMeshProUGUI>();

            var labelOverNames = new[]
                {labelOverNameLeftUpper, labelOverNameLeftLower, labelOverNameRightUpper, labelOverNameRightLower, labelOverNameLeft, labelOverNameRight};

            foreach (var label in labelOverNames)
            {
                label.text = "";
            }
            
            if (usernames.Length <= 2)
            {
                labelOverNameLeft.text = usernames[0];
                labelOverNameRight.text = usernames[1];
            }
            else
            {
                for (int i = 0; i < usernames.Length; i++)
                {
                    labelOverNames[i].text = usernames[i];
                }
            }

            var labelOverTimeLeft = GameObject.Find("label_over_time_left").GetComponent<TextMeshProUGUI>();
            var labelOverTimeRight = GameObject.Find("label_over_time_right").GetComponent<TextMeshProUGUI>();

            labelOverScoreLeft.text = scores[0].ToString();
            labelOverScoreRight.text = scores[1].ToString();

            labelOverTimeLeft.text = (time / 10 / 60).ToString("00");
            labelOverTimeRight.text = ((time / 10) % 60).ToString("00");

            // if (usernames.Length == 4)
            // {
            //     labelOverNameLeftLower.GetComponent<TextMeshProUGUI>().text = usernames[2];
            //     labelOverNameRightLower.GetComponent<TextMeshProUGUI>().text = usernames[3];
            // }

            var panelOverTop = GameObject.Find("panel_over_top").GetComponent<Image>();
            var panelOverBottom = GameObject.Find("panel_over_bottom").GetComponent<Image>();

            panelOverTop.color = color;
            panelOverBottom.color = color;
        }
    }
}
