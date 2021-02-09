using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Blobby.UserInterface
{
    public static class PanelOver
    {
        public static void Populate(string[] usernames, int[] scores, int time, Side winner)
        {
            var labelOverScoreLeft = GameObject.Find("label_over_score_left").GetComponent<TextMeshProUGUI>();
            var labelOverScoreRight = GameObject.Find("label_over_score_right").GetComponent<TextMeshProUGUI>();

            var labelOverNameLeft = GameObject.Find("label_over_name_left").GetComponent<TextMeshProUGUI>();
            var labelOverNameLeftOther = GameObject.Find("label_over_name_left_other").GetComponent<TextMeshProUGUI>();
            var labelOverNameRight = GameObject.Find("label_over_name_right").GetComponent<TextMeshProUGUI>();
            var labelOverNameRightOther = GameObject.Find("label_over_name_right_other").GetComponent<TextMeshProUGUI>();

            var labelOverNames = new[]
                {labelOverNameLeft, labelOverNameLeftOther, labelOverNameRight, labelOverNameRightOther};

            var labelOverTimeLeft = GameObject.Find("label_over_time_left").GetComponent<TextMeshProUGUI>();
            var labelOverTimeRight = GameObject.Find("label_over_time_right").GetComponent<TextMeshProUGUI>();

            labelOverScoreLeft.text = scores[0].ToString();
            labelOverScoreRight.text = scores[1].ToString();

            foreach (var label in labelOverNames)
            {
                label.text = "";
            }
            
            for (int i = 0; i < usernames.Length; i++)
            {
                labelOverNames[i].text = usernames[i];
            }

            labelOverTimeLeft.text = (time / 10 / 60).ToString("00");
            labelOverTimeRight.text = ((time / 10) % 60).ToString("00");

            if (usernames.Length == 4)
            {
                labelOverNameLeftOther.GetComponent<TextMeshProUGUI>().text = usernames[2];
                labelOverNameRightOther.GetComponent<TextMeshProUGUI>().text = usernames[3];
            }
        }
    }
}
