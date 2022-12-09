using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace BlobbyVolleyWorld.Maps
{
    public class MapComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [Required]
        [TitleGroup("General")]
        List<MapAsset> Maps { get; set; } = new();

        [OdinSerialize]
        [Required]
        Dictionary<Map, List<GameObject>> MapObjects { get; set; } = new();
        
        public void SetMap(Map mapToSet)
        {
            foreach (var (map, mapObjects) in MapObjects)
            {
                foreach (var mapObject in mapObjects)
                {
                    mapObject.SetActive(map == mapToSet);
                    
                    var canvasGroup = mapObject.GetComponent<CanvasGroup>();
                    
                    if (!canvasGroup) continue;
                    
                    canvasGroup.alpha = map == mapToSet ? 1 : 0;
                    canvasGroup.interactable = map == mapToSet;
                    canvasGroup.blocksRaycasts = map == mapToSet;
                }
            }
        }
        
        public MapAsset FindMapAsset(Map map) => 
            Maps.Find(m => m.Map == map);
    }
}