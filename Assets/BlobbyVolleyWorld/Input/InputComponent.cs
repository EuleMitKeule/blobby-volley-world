using BlobbyVolleyWorld.Game;
using Sirenix.OdinInspector;

namespace BlobbyVolleyWorld.Entities.Input
{
    public abstract class InputComponent : SerializedMonoBehaviour
    {
        protected GameComponent GameComponent { get; private set; }
        
        public bool IsUpPressed { get; set; }
        public float JumpCharge { get; set; }
        public virtual float HorizontalInput { get; set; }

        protected virtual void Awake()
        {
            GameComponent = FindObjectOfType<GameComponent>();
        }
    }
}