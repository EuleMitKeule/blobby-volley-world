using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace BlobbyVolleyWorld.UserInterface.Animation
{
    public class ModeButtonComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        ModeSelectionAnimationComponent ModeSelectionAnimationComponent { get; set; }

        public void OnButton()
        {
            ModeSelectionAnimationComponent.MoveToPosition(transform.position.x);
        }
    }
}