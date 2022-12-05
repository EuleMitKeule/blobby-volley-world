using System;
using System.Collections;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace BlobbyVolleyWorld.UserInterface.Animation
{
    public class ModeSelectionAnimationComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        float Timestep { get; set; } = 0.01f;
        
        float TargetX { get; set; }
        
        Coroutine SlideCoroutine { get; set; }

        Transform Transform { get; set; }
        
        void Awake()
        {
            Transform = transform;
        }
        
        public void MoveToPosition(float x)
        {
            if (Math.Abs(TargetX - x) < float.Epsilon)
            {
                return;
            }
            
            TargetX = x;
            
            if (SlideCoroutine != null)
            {
                StopCoroutine(SlideCoroutine);
            }
            
            SlideCoroutine = StartCoroutine(Slide());
        }

        IEnumerator Slide()
        {
            var time = 0f;

            while (Mathf.Abs(transform.position.x - TargetX) > float.Epsilon)
            {
                time += Timestep;

                var newX = Mathf.Lerp(transform.position.x, TargetX, time);
                var newPosition = new Vector3(newX, Transform.position.y);
                
                Transform.position = newPosition;

                yield return new WaitForSeconds(Timestep);
            }
            
            SlideCoroutine = null;
        }
    }
}