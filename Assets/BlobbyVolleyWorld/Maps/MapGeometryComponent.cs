using System;
using BlobbyVolleyWorld.Game;
using BlobbyVolleyWorld.Match;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace BlobbyVolleyWorld.Maps
{
    public class MapGeometryComponent : SerializedMonoBehaviour
    {
        [TitleGroup("Geometry")]
        [OdinSerialize]
        [Required]
        EdgeCollider2D GroundCollider { get; set; }
        
        [OdinSerialize]
        [Required]
        EdgeCollider2D LeftWallCollider { get; set; }
        
        [OdinSerialize]
        [Required]
        EdgeCollider2D RightWallCollider { get; set; }
        
        [OdinSerialize]
        [Required]
        CircleCollider2D NetEdgeCollider { get; set; }
        
        [OdinSerialize]
        [Required]
        EdgeCollider2D LeftNetCollider { get; set; }
        
        [OdinSerialize]
        [Required]
        EdgeCollider2D RightNetCollider { get; set; }

        [ShowInInspector]
        [ReadOnly]
        public float GroundHeight { get; private set; }
        
        [ShowInInspector]
        [ReadOnly]
        public float NetHeight { get; private set; }
        
        [ShowInInspector]
        [ReadOnly]
        public float LeftFieldCenter { get; private set; }
        
        [ShowInInspector]
        [ReadOnly]
        public float RightFieldCenter { get; private set; }
        
        GameComponent GameComponent { get; set; }
        
        void Awake()
        {
            GameComponent = FindObjectOfType<GameComponent>();
            
            GroundHeight = GroundCollider.offset.y;
            NetHeight = NetEdgeCollider.offset.y + NetEdgeCollider.radius;
            LeftFieldCenter = -Mathf.Abs(LeftWallCollider.offset.x - LeftNetCollider.offset.x);
            RightFieldCenter = Mathf.Abs(RightWallCollider.offset.x - RightNetCollider.offset.x);
        }

        public float GetLeftLimit(FieldPosition fieldPosition, bool isAboveNet, Side currentSide)
        {
            var leftWallLimit = LeftWallCollider.offset.x;
            var rightNetLimit = RightNetCollider.offset.x;

            switch (fieldPosition)
            {
                case FieldPosition.LeftOuter:

                    if (currentSide == Side.Right && !isAboveNet)
                    {
                        return rightNetLimit;
                    }

                    return leftWallLimit;
                case FieldPosition.RightOuter:

                    if (GameComponent.MatchSettings.PlayerMode == PlayerMode.DoubleFixed)
                    {
                        return RightFieldCenter;
                    }

                    if (currentSide == Side.Left || isAboveNet)
                    {
                        return leftWallLimit;
                    }
                    
                    return rightNetLimit;
                    
                case FieldPosition.LeftInner:

                    if (currentSide == Side.Right && !isAboveNet)
                    {
                        return rightNetLimit;
                    }
                    
                    if (GameComponent.MatchSettings.PlayerMode == PlayerMode.DoubleFixed)
                    {
                        return LeftFieldCenter;
                    }

                    return leftWallLimit;
                    
                case FieldPosition.RightInner:
                    
                    if (currentSide == Side.Right && !isAboveNet)
                    {
                        return rightNetLimit;
                    }
                    
                    if (GameComponent.MatchSettings.PlayerMode == PlayerMode.DoubleFixed)
                    {
                        return LeftFieldCenter;
                    }
                    
                    return leftWallLimit; 
                    
                default:
                    throw new ArgumentOutOfRangeException(nameof(fieldPosition), fieldPosition, null);
            }
        }

        public float GetRightLimit(FieldPosition fieldPosition, bool isAboveNet, Side currentSide)
        {
            var rightWallLimit = RightWallCollider.offset.x;
            var leftNetLimit = LeftNetCollider.offset.x;

            switch (fieldPosition)
            {
                case FieldPosition.LeftOuter:
                    
                    if (GameComponent.MatchSettings.PlayerMode == PlayerMode.DoubleFixed)
                    {
                        return RightFieldCenter;
                    }

                    if (currentSide == Side.Right)
                    {
                        return rightWallLimit;
                    }
                    
                    return leftNetLimit;
                    
                case FieldPosition.RightOuter:
                    
                    if (currentSide == Side.Left && !isAboveNet)
                    {
                        return leftNetLimit;
                    }
                    
                    return rightWallLimit;
                    
                case FieldPosition.LeftInner:

                    if (currentSide == Side.Left && !isAboveNet)
                    {
                        return leftNetLimit;
                    }
                    
                    if (GameComponent.MatchSettings.PlayerMode == PlayerMode.DoubleFixed)
                    {
                        return RightFieldCenter;
                    }
                    
                    return rightWallLimit;
                    
                case FieldPosition.RightInner:
                    
                    if (currentSide == Side.Left && !isAboveNet)
                    {
                        return leftNetLimit;
                    }
                    
                    if (GameComponent.MatchSettings.PlayerMode == PlayerMode.DoubleFixed)
                    {
                        return RightFieldCenter;
                    }
                    
                    return rightWallLimit;
                    
                default:
                    throw new ArgumentOutOfRangeException(nameof(fieldPosition), fieldPosition, null);
            }
        }
    }
}