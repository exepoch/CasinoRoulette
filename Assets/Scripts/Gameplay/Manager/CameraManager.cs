using System;
using System.Collections;
using UnityEngine;

namespace Gameplay.Manager
{
    // Manages smooth camera transitions between roulette and betting table views using easing curves.
    public class CameraManager : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private Transform camTransform; // Camera to move
        [SerializeField] private AnimationCurve animationEase; // Easing curve for smooth animation

        [Header("Target Transforms")]
        [SerializeField] private Transform betRotation; // Target transform for betting table view
        [SerializeField] private Transform rouletteRotation; // Target transform for roulette view

        private Coroutine _rotationCoroutine; // Reference to the running rotation coroutine
        private Action _animationCallBack; // Callback after animation ends

        // Rotate camera to roulette position
        public void RotateToRoulette(float duration = 1f, float delay = 0f, Action callBack = null)
        {
            StartRotation(rouletteRotation, duration, delay, callBack);
        }

        // Rotate camera to betting table position
        public void RotateToBetTable(float duration = 1f, float delay = 0f, Action callBack = null)
        {
            StartRotation(betRotation, duration, delay, callBack);
        }

        // Starts the rotation animation
        private void StartRotation(Transform target, float duration, float delay, Action callBack)
        {
            if (target == null || camTransform == null)
            {
                Debug.LogWarning("Camera target is NULL!");
                if (_rotationCoroutine != null)
                    StopCoroutine(_rotationCoroutine);
                return;
            }

            _animationCallBack?.Invoke(); // Clear any previous callbacks
            _animationCallBack = callBack;

            if (_rotationCoroutine != null)
                StopCoroutine(_rotationCoroutine); // Stop ongoing animation

            duration = Mathf.Max(0f, duration);
            delay = Mathf.Max(0f, delay);

            _rotationCoroutine = StartCoroutine(RotateCoroutine(target, duration, delay));
        }

        // Coroutine that smoothly rotates and moves the camera
        private IEnumerator RotateCoroutine(Transform target, float duration, float delay)
        {
            yield return new WaitForSeconds(delay);

            var elapsed = 0f;
            var startRot = camTransform.rotation;
            var startPos = camTransform.position;

            var endRot = target.rotation;
            var endPos = target.position;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var easedT = animationEase.Evaluate(t);

                // Smoothly interpolate position and rotation
                camTransform.SetPositionAndRotation(
                    Vector3.Lerp(startPos, endPos, easedT),
                    Quaternion.Lerp(startRot, endRot, easedT)
                );

                yield return null;
            }

            // Ensure final position is exact
            camTransform.SetPositionAndRotation(endPos, endRot);
            _animationCallBack?.Invoke();
            _animationCallBack = null;
        }
    }
}
