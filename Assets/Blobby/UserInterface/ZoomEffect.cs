using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Components;
using Blobby.Models;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Blobby.UserInterface
{
    public class ZoomEffect : IDisposable
    {
        Transform _target;

        Action _inCallback;
        Action _outCallback;

        Vector2 _curCamPos;

        Vector2 clampMin { get { return new Vector2(
            -19.2f + Camera.main.orthographicSize * 16 / 9, 
            -10.8f + Camera.main.orthographicSize); } }

        Vector2 clampMax { get { return new Vector2(
            19.2f - Camera.main.orthographicSize * 16 / 9,
            10.8f - Camera.main.orthographicSize); } }

        Vector2? _targetPos
        {
            get
            {
                if (_target != null)
                {
                    var targetPosX = Mathf.Clamp(_target.position.x, clampMin.x, clampMax.x);
                    var targetPosY = Mathf.Clamp(_target.position.y, clampMin.y, clampMax.y);
                    return new Vector2(targetPosX, targetPosY);
                }
                else
                {
                    var targetPosX = Mathf.Clamp(0f, clampMin.x, clampMax.x);
                    var targetPosY = Mathf.Clamp(0f, clampMin.y, clampMax.y);
                    return new Vector2(targetPosX, targetPosY);
                }
            }
        }

        public ZoomEffect(Action inCallback, Action outCallback)
        {
            _inCallback = inCallback;
            _outCallback = outCallback;
        }

        public void ZoomIn(Transform target)
        {
            _target = target;

            MainThreadManager.Run(() =>
            {
                TimeComponent.LateUpdateTicked -= LateUpdate;
                TimeComponent.LateUpdateTicked += LateUpdate;

                var camAnimator = Camera.main.GetComponent<Animator>();

                var canvasOver = GameObject.Find("canvas_over");
                var canvasOverAnimator = canvasOver.GetComponent<Animator>();
                var canvasOverGroup = canvasOver.GetComponent<CanvasGroup>();

                canvasOverGroup.alpha = 1;
                canvasOverGroup.blocksRaycasts = true;
                canvasOverGroup.interactable = true;

                camAnimator.SetBool("zoom", true);
                canvasOverAnimator.SetBool("show", true);

                _inCallback?.Invoke();
            });
        }

        public async void ZoomOut()
        {
            _target = null;

            MainThreadManager.Run(() =>
            {
                TimeComponent.LateUpdateTicked -= LateUpdate;
                TimeComponent.LateUpdateTicked += LateUpdate;

                var camAnimator = Camera.main.GetComponent<Animator>();

                var canvasOver = GameObject.Find("canvas_over");
                var canvasOverAnimator = canvasOver.GetComponent<Animator>();

                camAnimator.SetBool("zoom", false);
                canvasOverAnimator.SetBool("show", false);
            });

            await Task.Delay(1100);

            MainThreadManager.Run(() =>
            {
                var canvasOver = GameObject.Find("canvas_over");
                var canvasOverGroup = canvasOver.GetComponent<CanvasGroup>();

                canvasOverGroup.alpha = 0;
                canvasOverGroup.blocksRaycasts = false;
                canvasOverGroup.interactable = false;

                _outCallback?.Invoke();
            });

            Dispose();
        }

        public void Dispose()
        {
            TimeComponent.LateUpdateTicked -= LateUpdate;
        }

        void LateUpdate()
        {
            if (!_targetPos.HasValue) return;

            _curCamPos = Vector2.Lerp(_curCamPos, _targetPos.Value, 0.05f);
            Camera.main.transform.position = new Vector3(_curCamPos.x, _curCamPos.y, -10f);

            Camera.main.transform.position = new Vector3(
                Mathf.Clamp(Camera.main.transform.position.x, clampMin.x, clampMax.x),
                Mathf.Clamp(Camera.main.transform.position.y, clampMin.y, clampMax.y), -10f);
        }
    }
}
