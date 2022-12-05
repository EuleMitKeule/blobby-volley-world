using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace BlobbyVolleyWorld.Maps
{
    [CreateAssetMenu(menuName = nameof(MapAsset), fileName = nameof(MapAsset))]
    public class MapAsset : SerializedScriptableObject
    {
        [OdinSerialize]
        public Map Map { get; set; }
        
        [OdinSerialize]
        public Sprite Icon { get; set; }
        
        [OdinSerialize]
        public string Name { get; set; }
    }
}