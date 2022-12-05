using Blobby.UserInterface;
using UnityEngine;

namespace Blobby.Tools
{
    public class HexSpawnerTool : MonoBehaviour, IColorable
    {
        public GameObject hexPrefab;

        public bool Colored { get; set; }

        public float hexSpeed, hexSpawnTime, hexSpawnRand, hexLifeTime;

        public float spawnX, spawnOffY;

        public float lastSpawn;

        [Range(0f, 1f)]
        public float hexAlpha;

        public bool flipped;

        [SerializeField] float StartY = 11f;

        private void FixedUpdate()
        {
            if (Time.fixedTime > lastSpawn + hexSpawnTime)
            {
                lastSpawn = Time.fixedTime;

                if (Random.Range(0f, 1f) <= hexSpawnRand)
                {
                    GameObject newHex = Instantiate(hexPrefab, new Vector3(spawnX, (flipped ? -1 : 1) * StartY + (flipped ? -1 : 1) * spawnOffY, 3f), Quaternion.identity);
                    newHex.transform.SetParent(transform);

                    newHex.GetComponent<HexTool>().speed = hexSpeed;

                    if (Colored)
                        newHex.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), hexAlpha);
                    else
                    {
                        newHex.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, hexAlpha);
                    }

                    Destroy(newHex, hexLifeTime);
                }
            }
        }
    }
}
