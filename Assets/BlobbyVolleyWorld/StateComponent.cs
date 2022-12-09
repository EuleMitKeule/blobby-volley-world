using System;
using Sirenix.OdinInspector;

namespace BlobbyVolleyWorld
{
    public abstract class StateComponent : SerializedMonoBehaviour
    {
        public event EventHandler<Type> ChangeRequested;
        
        protected void SetState<T>() where T : StateComponent
        {
            ChangeRequested?.Invoke(this, typeof(T));
        }
        
        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void StateUpdate() { }
    }
}