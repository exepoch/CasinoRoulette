using System.Collections;
using Data;
using Events.EventTypes.Audio;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utils
{
    public class HoverScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float scaleMultiplier = 1.1f;
        [SerializeField] private float animDuration = 0.15f;

        private Vector3 _originalScale;
        private Coroutine _scaleCoroutine;

        private void Awake()
        {
            _originalScale = transform.localScale;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            StartScaleAnim(_originalScale * scaleMultiplier);
            AudioEvents.RequestSound(SoundType.Hover);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StartScaleAnim(_originalScale);
        }

        private void StartScaleAnim(Vector3 targetScale)
        {
            if (_scaleCoroutine != null)
                StopCoroutine(_scaleCoroutine);

            _scaleCoroutine = StartCoroutine(ScaleAnim(targetScale));
        }

        private IEnumerator ScaleAnim(Vector3 targetScale)
        {
            var startScale = transform.localScale;
            var time = 0f;

            while (time < animDuration)
            {
                transform.localScale = Vector3.Lerp(startScale, targetScale, time / animDuration);
                time += Time.unscaledDeltaTime;
                yield return null;
            }

            transform.localScale = targetScale;
        }
    }
}