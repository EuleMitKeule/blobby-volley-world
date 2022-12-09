using BlobbyVolleyWorld.Maps;
using BlobbyVolleyWorld.Match;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace BlobbyVolleyWorld.Entities.Physics
{
    public class PlayerGeometryComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [Required]
        CircleCollider2D UpperCollider { get; set; }
        
        [OdinSerialize]
        [Required]
        CircleCollider2D LowerCollider { get; set; }
        
        [OdinSerialize]
        [Required]
        EdgeCollider2D GroundCollider { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        public Vector2 BottomCenterOffset { get; private set; }

        [ShowInInspector]
        [ReadOnly]
        public Vector2 BottomOffset { get; private set; }
        
        [ShowInInspector]
        [ReadOnly]
        public float BottomRadius { get; private set; }

        [ShowInInspector]
        [ReadOnly]
        public float LeftLimit { get; private set; }

        [ShowInInspector]
        [ReadOnly]
        public float RightLimit { get; private set; }

        [ShowInInspector]
        [ReadOnly]
        public float GroundLimit { get; private set; }
        
        PlayerComponent PlayerComponent { get; set; }
        MapGeometryComponent MapGeometryComponent { get; set; }
        
        void Awake()
        {
            MapGeometryComponent = FindObjectOfType<MapGeometryComponent>();
            PlayerComponent = GetComponent<PlayerComponent>();
            
            PlayerComponent.FieldPositionChanged += OnFieldPositionChanged;
            
            BottomCenterOffset = LowerCollider.offset;
            BottomRadius = LowerCollider.radius;
            BottomOffset = BottomCenterOffset + Vector2.down * BottomRadius;
        }

        void OnFieldPositionChanged(object sender, FieldPosition fieldPosition)
        {
            var position = transform.position;
            var isAboveNet = position.y + BottomOffset.y > MapGeometryComponent.NetHeight;
            var currentSide = position.x > 0 ? Side.Right : Side.Left;
            
            LeftLimit = MapGeometryComponent.GetLeftLimit(fieldPosition, isAboveNet, currentSide) + BottomRadius;
            RightLimit = MapGeometryComponent.GetRightLimit(fieldPosition, isAboveNet, currentSide) - BottomRadius;
            GroundLimit = MapGeometryComponent.GroundHeight + BottomRadius;
        }
    }
}