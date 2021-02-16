using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Blobby.Components
{
    public class AnimTool : MonoBehaviour
    {
        Animator _animator;

        public static event Action<Mode, float> ModeChanged;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        public void SetHover(bool value)
        {
            var button = GetComponent<Button>();
            var interactable = !button || button.interactable;
            
            _animator.SetBool("hover", interactable && value);
        }

        public void SetClick(bool value)
        {
            var button = GetComponent<Button>();
            var interactable = !button || button.interactable;
            
            _animator.SetBool("click", interactable && value);
        }

        public void SetRegister(bool value)
        {
            _animator.SetBool("register", value);
        }

        public void SetLeft(bool value)
        {
            _animator.SetBool("left", value);
        }

        public void SetRight(bool value)
        {
            _animator.SetBool("right", value);
        }

        public void ToggleShow()
        {
            _animator.SetBool("show", !_animator.GetBool("show"));
        }

        public void ToggleSelected()
        {
            _animator.SetBool("selected", !_animator.GetBool("selected"));
        }

        public void SetSelected(bool value)
        {
            _animator.SetBool("selected", value);
        }

        public void OnButtonMode(int mode)
        {
            ModeChanged?.Invoke((Mode)mode, transform.position.x);
        }
    }
}
