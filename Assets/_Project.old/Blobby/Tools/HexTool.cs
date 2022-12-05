using UnityEngine;

namespace Blobby.Tools
{
    public class HexTool : MonoBehaviour
    {
        public float speed;

        private void FixedUpdate()
        {
            transform.position -= new Vector3(0f, speed * Time.fixedDeltaTime);
        }
    }
}
