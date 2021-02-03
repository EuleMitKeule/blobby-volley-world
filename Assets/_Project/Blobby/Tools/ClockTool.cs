using System;
using System.Collections;
using UnityEngine;

public class ClockTool : MonoBehaviour
{
    public GameObject hourHand, minuteHand, secondHand;
    private int timeZone = 28800, time = 0;

    private void Start()
    {
        time = (DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second + timeZone) % 86400;
        StartCoroutine(Clock());
    }

    private IEnumerator Clock()
    {
        hourHand.transform.Rotate(0f, 0f, -0.004f * time);
        minuteHand.transform.Rotate(0f, 0f, -0.1f * (time % 3600));
        secondHand.transform.Rotate(0f, 0f, -6f * (time % 216000));

        while (true)
        {
            hourHand.transform.Rotate(0f, 0f, -0.004f);
            minuteHand.transform.Rotate(0f, 0f, -0.1f);
            secondHand.transform.Rotate(0f, 0f, -6f);

            yield return new WaitForSecondsRealtime(1f);
            time++;
        }
    }
}
