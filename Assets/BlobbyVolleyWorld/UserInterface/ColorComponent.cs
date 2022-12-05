using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace BlobbyVolleyWorld.UserInterface
{
    public class ColorComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [Required]
        List<SpriteRenderer> SpriteRenderers { get; set; } = new();
        
        [OdinSerialize]
        [Required]
        List<Image> Images { get; set; } = new();
        
        public void SetColor(Color color)
        {
            foreach (var spriteRenderer in SpriteRenderers)
            {
                spriteRenderer.color = color;
            }
            
            foreach (var image in Images)
            {
                image.color = color;
            }
        }
    }
}