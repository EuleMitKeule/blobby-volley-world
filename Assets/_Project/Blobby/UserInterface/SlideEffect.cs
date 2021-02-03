using BeardedManStudios.Forge.Networking.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Blobby.UserInterface
{
    public class SlideEffect
    {
        Action _leftCallback;
        Action _rightCallback;

        public SlideEffect(Action leftCallback, Action rightCallback)
        {
            _leftCallback = leftCallback;
            _rightCallback = rightCallback;
        }

        public async void Slide(Side side)
        {
            //var time = 0.01;
            //var camPos = Camera.main.transform.position;
            //var leftPos = Vector2.left * 1080;

            if (side == Side.Left)
            {
                MainThreadManager.Run(() => Camera.main.GetComponent<Animator>().SetBool("left", true));

                //camPos = (Vector3)Vector2.Lerp(Vector2.zero, leftPos, time) - Vector3.forward * 10;

                await Task.Delay(1000);

                _leftCallback?.Invoke();
            }
            else if (side == Side.Right)
            {
                MainThreadManager.Run(() => Camera.main.GetComponent<Animator>().SetBool("left", false));

                await Task.Delay(1000);

                _rightCallback?.Invoke();
            }
        }
    }
}
