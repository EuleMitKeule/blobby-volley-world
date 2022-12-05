using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace BlobbyVolleyWorld.UserInterface.Animation
{
    public class ButtonAnimationComponent : SerializedMonoBehaviour
    {
        Animator Animator { get; set; }
        Button Button { get; set; }
        
        static readonly int Hover = Animator.StringToHash("hover");
        static readonly int Click = Animator.StringToHash("click");

        bool IsInteractable =>
            !Button || Button.interactable;
        
        void Awake()
        {
            Animator = GetComponent<Animator>();
            Button = GetComponent<Button>();
        }
        
        public void SetHover(bool value)
        {
            Animator.SetBool(Hover, IsInteractable && value);
        }

        public void SetClick(bool value)
        {
            Animator.SetBool(Click, IsInteractable && value);
        }
    }
}