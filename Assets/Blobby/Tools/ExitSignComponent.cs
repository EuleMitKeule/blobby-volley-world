using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ExitSignComponent : MonoBehaviour
{
    static int OnAnim { get; set; }
    Animator Animator { get; set; }

    enum FlickerState { On, Off }
    FlickerState CurFlickerState { get; set; }

    public void Start()
    {
        Animator = gameObject.GetComponent<Animator>();
        OnAnim = Animator.StringToHash("on");

        StartCoroutine(FlickerToggle());
    }

    IEnumerator FlickerToggle()
    {
        while (true)
        {
            switch (CurFlickerState)
            {
                case FlickerState.Off:

                    Animator.SetBool(OnAnim, false);

                    var shouldSwitchToOff = Random.Range(0, 5) == 0;
                    if (shouldSwitchToOff) CurFlickerState = FlickerState.On;

                    yield return new WaitForSeconds(0.5f);
                    
                    break;
                case FlickerState.On:
                    
                    var shouldSwitchToOn = Random.Range(0, 4) == 0;
                    if (shouldSwitchToOn) CurFlickerState = FlickerState.Off;

                    var isOn = Animator.GetBool(OnAnim);
                    Animator.SetBool(OnAnim, !isOn);

                    var time = Random.Range(0f, 0.2f);
                    yield return new WaitForSeconds(time);

                    break;
            }


        }
    }
}
