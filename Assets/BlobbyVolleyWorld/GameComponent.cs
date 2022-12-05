using System.Collections.Generic;
using BlobbyVolleyWorld.Maps;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace BlobbyVolleyWorld
{
    public abstract class GameComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        List<MapAsset> Maps { get; set; }

        public MapAsset FindMapAsset(Map map) => 
            Maps.Find(m => m.Map == map);
    }
}