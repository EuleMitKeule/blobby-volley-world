using BlobbyVolleyWorld.Maps;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace BlobbyVolleyWorld.Entities.Ball
{
    public class BallGeometryComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [Required]
        CircleCollider2D Collider { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        public float Radius { get; private set; }
        
        [ShowInInspector]
        [ReadOnly]
        public float GroundLimit { get; private set; }

        MapGeometryComponent MapGeometryComponent { get; set; }
        
        void Awake()
        {
            MapGeometryComponent = FindObjectOfType<MapGeometryComponent>();
            
            Radius = Collider.radius;
            GroundLimit = MapGeometryComponent.GroundHeight + Radius;
        }
    }
}