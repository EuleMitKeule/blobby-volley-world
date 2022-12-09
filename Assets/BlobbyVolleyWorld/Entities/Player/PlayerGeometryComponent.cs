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
        public Vector2 UpperCenterOffset { get; private set; }
        
        [ShowInInspector]
        [ReadOnly]
        public float UpperRadius { get; private set; }

        [ShowInInspector]
        [ReadOnly]
        public Vector2 TopOffset { get; private set; }
        
        [ShowInInspector]
        [ReadOnly]
        public Vector2 LowerCenterOffset { get; private set; }
        
        [ShowInInspector]
        [ReadOnly]
        public float LowerRadius { get; private set; }

        [ShowInInspector]
        [ReadOnly]
        public Vector2 BottomOffset { get; private set; }

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
        SideComponent SideComponent { get; set; }
        
        void Awake()
        {
            MapGeometryComponent = FindObjectOfType<MapGeometryComponent>();
            PlayerComponent = GetComponent<PlayerComponent>();
            SideComponent = GetComponent<SideComponent>();
            
            PlayerComponent.FieldPositionChanged += OnFieldPositionChanged;
            
            UpperCenterOffset = UpperCollider.offset;
            UpperRadius = UpperCollider.radius;
            LowerCenterOffset = LowerCollider.offset;
            LowerRadius = LowerCollider.radius;
            TopOffset = UpperCenterOffset + Vector2.up * UpperRadius;
            BottomOffset = LowerCenterOffset + Vector2.down * LowerRadius;
        }

        void OnFieldPositionChanged(object sender, FieldPosition fieldPosition)
        {
            Debug.Log($"GEOMETRY");
            
            var position = transform.position;
            var isAboveNet = position.y + BottomOffset.y > MapGeometryComponent.NetHeight;
            var currentSide = SideComponent.Side;
            
            LeftLimit = MapGeometryComponent.GetLeftLimit(fieldPosition, isAboveNet, currentSide) + LowerRadius;
            RightLimit = MapGeometryComponent.GetRightLimit(fieldPosition, isAboveNet, currentSide) - LowerRadius;
            GroundLimit = MapGeometryComponent.GroundHeight + LowerRadius;
        }
    }
}