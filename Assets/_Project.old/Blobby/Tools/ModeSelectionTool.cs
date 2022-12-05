using System.Collections;
using UnityEngine;

namespace Blobby.Tools
{
    public class ModeSelectionTool : MonoBehaviour
    {
        [SerializeField] Mode _mode;

        [SerializeField] float _timestep = 0.01f;

        void Start()
        {
            AnimTool.ModeChanged += OnModeChanged;
        }

        void OnModeChanged(Mode mode, float x)
        {
            if (mode != _mode) return;

            StopAllCoroutines();
            StartCoroutine(SelectionSlide(x));
        }

        IEnumerator SelectionSlide(float x)
        {
            float time = 0;

            while (Mathf.Abs(transform.position.x - x) > 0.01f)
            {
                time += _timestep;
                transform.position = new Vector2(Mathf.Lerp(transform.position.x, x, time), transform.position.y);
                yield return new WaitForSeconds(_timestep);
            }
        }
    }
}
