using UnityEngine;

public class BlobSpawnerTool : MonoBehaviour, IColorable
{
    public GameObject blobPrefab;

    [Range(0f, 10f)]
    public float blobSpawnRate;

    [Range(0f, 1f)]
    public float blobSpawnChance;

    [Range(0f, 200f)]
    public float blobLifeDistance;

    [Range(0f, 15f)]
    public float blobSpeed;

    [Range(0f, 35f)]
    public float blobDrag;

    [Range(0f, 5f)]
    public float blobScale;

    [Range(-11f, 11f)]
    public float spawnY;

    [Range(-5.5f, 5.5f)]
    public float randY;

    [Range(0f, 1f)]
    public float randomizer;

    [Range(0f, 1f)]
    public float blobAlpha;

    public float lastSpawn;

    public bool Colored { get; set; }
    public bool shading;
    public bool glow;

    private void FixedUpdate()
    {
        if (Time.fixedTime > lastSpawn + blobSpawnRate)
        {
            lastSpawn = Time.fixedTime;

            if (Random.Range(0f, 1f) <= blobSpawnChance)
            {
                float rand = Random.Range(1f - randomizer, 1f);
                float randSign = Random.Range(0f, 1f) > 0.5f ? 1 : -1;

                GameObject newBlob = Instantiate(blobPrefab, new Vector3(transform.position.x, spawnY + Random.Range(-randY, randY), 1f - rand), Quaternion.identity);
                newBlob.transform.SetParent(transform);

                float randScale = rand * blobScale;
                float randSpeed = rand * blobSpeed;
                float randDrag = randSign * rand * blobDrag;

                newBlob.transform.localScale = new Vector3(randScale, randScale, 1f);
                newBlob.GetComponent<BlobTool>().zRotation = Random.Range(0, 360);
                newBlob.GetComponent<BlobTool>().angularSpeed = randDrag;
                newBlob.GetComponent<BlobTool>().speed = randSpeed;

                if (Colored)
                    newBlob.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), blobAlpha);
                else
                { 
                    newBlob.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, blobAlpha);
                }

                if (!shading)
                {
                    newBlob.GetComponentsInChildren<SpriteRenderer>()[1].enabled = false;
                }

                if (!glow)
                {
                    newBlob.GetComponentsInChildren<SpriteRenderer>()[2].enabled = false;
                }

                Destroy(newBlob, 150f / randSpeed);
            }
        }
    }
}
