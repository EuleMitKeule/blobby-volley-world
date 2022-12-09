using System;
using BlobbyVolleyWorld.Game;
using BlobbyVolleyWorld.Maps;
using BlobbyVolleyWorld.Match;
using Sirenix.OdinInspector;

namespace BlobbyVolleyWorld.Entities
{
    public class PlayerComponent : SerializedMonoBehaviour
    {
        [TitleGroup("General")]
        [ShowInInspector]
        [ReadOnly]
        public FieldPosition FieldPosition { get; private set; }
        
        [TitleGroup("Geometry")]
        [ShowInInspector]
        [ReadOnly]
        public float LeftBorder { get; private set; }
        
        [ShowInInspector]
        [ReadOnly]
        public float RightBorder { get; private set; }
        MatchComponent MatchComponent { get; set; }
        MapGeometryComponent MapGeometryComponent { get; set; }

        public event EventHandler<FieldPosition> FieldPositionChanged;
        
        void Awake()
        {
            MatchComponent = FindObjectOfType<MatchComponent>();
            MapGeometryComponent = FindObjectOfType<MapGeometryComponent>();
        }
        
        public void SetFieldPosition(FieldPosition fieldPosition)
        {
            FieldPosition = fieldPosition;
            FieldPositionChanged?.Invoke(this, fieldPosition);
        }
    }
}