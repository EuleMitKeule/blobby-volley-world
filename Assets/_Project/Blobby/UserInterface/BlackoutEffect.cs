using BeardedManStudios.Forge.Networking.Unity;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Blobby.UserInterface
{
    public class BlackoutEffect
    {
        Action _blackCallback;
        Action _whiteCallback;

        public BlackoutEffect(Action blackCallback = null, Action whiteCallback = null)
        {
            _blackCallback = blackCallback;
            _whiteCallback = whiteCallback;
        }

        public async void Blackout()
        {
            Debug.Log("Blackout");
            GameObject.Find("panel_black").GetComponent<Animator>().SetBool("black", true);

            SetBlock(false);
            
            await Task.Delay(1800); 

            _blackCallback?.Invoke();
        }

        public async void Whiteout()
        {
            GameObject.Find("panel_black").GetComponent<Animator>().SetBool("black", false);

            SetBlock(true);

            await Task.Delay(2000);

            _whiteCallback?.Invoke();
        }

        void SetBlock(bool interactable)
        {
            var ui = GameObject.Find("ui");

            foreach (var canvasGroup in ui.GetComponentsInChildren<CanvasGroup>())
            {
                canvasGroup.interactable = interactable;
            }
        }
    }
}
