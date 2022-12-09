using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BlobbyVolleyWorld.UserInterface.Animation
{
    public class BlackoutAnimationComponent : SerializedMonoBehaviour
    {
        Animator Animator { get; set; }

        public event EventHandler BlackoutStarted;
        public event EventHandler WhiteoutStarted;
        public event EventHandler BlackoutFinished;
        public event EventHandler WhiteoutFinished;

        static readonly int Black = Animator.StringToHash("black");
        
        void Awake()
        {
            Animator = GetComponent<Animator>();
        }
        
        public void Blackout()
        {
            BlackoutStarted?.Invoke(this, EventArgs.Empty);
            Animator.SetBool(Black, true);
        }

        public void Whiteout()
        {
            WhiteoutStarted?.Invoke(this, EventArgs.Empty);
            Animator.SetBool(Black, false);
        }

        public void OnBlackoutAnimationFinished()
        {
            BlackoutFinished?.Invoke(this, EventArgs.Empty);
        }
        
        public void OnWhiteoutAnimationFinished()
        {
            WhiteoutFinished?.Invoke(this, EventArgs.Empty);
        }
    }
}