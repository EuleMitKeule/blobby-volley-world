using UnityEngine;

public class BlobTool : MonoBehaviour
{
    public float speed, angularSpeed;

    public float zRotation;

    private void FixedUpdate()
    {
        transform.position -= new Vector3(speed * Time.fixedDeltaTime, 0f);

        zRotation += angularSpeed * Time.fixedDeltaTime;
        transform.rotation = Quaternion.Euler(0f, 0f, zRotation);
    }
}
