using System.Collections;
using UnityEngine;

namespace Blobby.Tools
{
    public class DropTool : MonoBehaviour
    {
        private Animator animator;

        public bool offset = false;
        private int spot = 0;
        private float velocity = 0f;

        private delegate void Drop();
        private event Drop OnDrop;

        [SerializeField] float StartY = 11f;

        [SerializeField]
        float[,] _dropSpots = { { -3.25f, -13.85f, -12.3f }, { -3.5f, -11.5f, -9.4f },
            { -3.25f, -11.3f, -9f }, { -3.7f, -8.7f, -6.66f } };

        [SerializeField] float _dropRate = 2f;
        [SerializeField] float gravity = 30f;

        private void Start()
        {
            animator = GetComponent<Animator>();
            StartCoroutine(Drip(offset ? _dropRate / 2f + 1f : 1f));
        }

        private void FixedUpdate()
        {
            OnDrop?.Invoke();
        }

        private void DropPhysics()
        {
            velocity -= gravity * Time.fixedDeltaTime;

            if (transform.position.y + velocity * Time.fixedDeltaTime > _dropSpots[spot, 0])
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + velocity * Time.fixedDeltaTime, 0f);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, _dropSpots[spot, 0], 0f);
                OnDrop -= DropPhysics;
                velocity = 0f;
                if (spot != 0) animator.SetBool("splashing", true);
                StartCoroutine(Drip());
            }
        }

        private IEnumerator Drip(float offset = 1f)
        {
            yield return new WaitForSeconds(offset);
            animator.SetBool("splashing", false);

            spot = Random.Range(0, 4);
            float randX = Random.Range(_dropSpots[spot, 1], _dropSpots[spot, 2]);

            transform.position = new Vector3(randX, StartY);

            float randWait = Random.Range(0.1f, 1f) * _dropRate;

            yield return new WaitForSeconds(randWait);
            OnDrop += DropPhysics;
        }
    }
}
