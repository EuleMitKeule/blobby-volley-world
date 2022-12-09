using System;
using BlobbyVolleyWorld.Match;
using Sirenix.OdinInspector;

namespace BlobbyVolleyWorld.Entities
{
    public class SideComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        [ReadOnly]
        public Side Side { get; private set; }
        
        public event EventHandler<Side> SideChanged;

        void FixedUpdate()
        {
            var newSide = transform.position.x > 0 ? Side.Right : Side.Left;
            
            if (newSide == Side) return;

            SetSide(newSide);
        }

        public void SetSide(Side side)
        {
            Side = side;
            SideChanged?.Invoke(this, Side);
        }
    }
}