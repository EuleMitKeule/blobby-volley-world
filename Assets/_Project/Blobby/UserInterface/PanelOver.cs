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
            var labelOverScoreLeft = GameObject.Find("label_over_score_left");
            var labelOverScoreRight = GameObject.Find("label_over_score_right");

            var labelOverNameLeft = GameObject.Find("label_over_name_left");
            var labelOverNameLeftOther = GameObject.Find("label_over_name_left_other");
            var labelOverNameRight = GameObject.Find("label_over_name_right");
            var labelOverNameRightOther = GameObject.Find("label_over_name_right_other");

            var labelOverTime = GameObject.Find("label_over_time");

            labelOverScoreLeft.GetComponent<TextMeshProUGUI>().text = scores[0].ToString();
            labelOverScoreRight.GetComponent<TextMeshProUGUI>().text = scores[1].ToString();

            if (usernames != null && usernames.Length > 0)
            {
                labelOverNameLeft.GetComponent<TextMeshProUGUI>().text = usernames[0];
                if (usernames.Length > 1)
                {
                    labelOverNameRight.GetComponent<TextMeshProUGUI>().text = usernames[1];
                }
            }

            labelOverTime.GetComponent<TextMeshProUGUI>().text = time.ToString();

            if (usernames.Length == 4)
            {
                labelOverNameLeftOther.GetComponent<TextMeshProUGUI>().text = usernames[2];
                labelOverNameRightOther.GetComponent<TextMeshProUGUI>().text = usernames[3];
            }
        }
    }
}
