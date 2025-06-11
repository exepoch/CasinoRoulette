using System;
using System.Collections;
using UnityEngine;

namespace Utils
{
    public class SceneHideCG : MonoBehaviour
    {
        public static SceneHideCG Instance;
        [SerializeField] private CanvasGroup cg;
        private IEnumerator _co;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }


        public void FadeIn(float duration,Action callBack = null)
        {
            if (_co != null)
            {
                StopCoroutine(_co);
            }
            
            _co = LoadUpAnimation(0,1,duration,callBack);
            StartCoroutine(_co);
        }
    
        public void FadeOut(float duration,Action callBack = null)
        {
            if (_co != null)
            {
                StopCoroutine(_co);
            }

            _co = LoadUpAnimation(1,0,duration,callBack);
            StartCoroutine(_co);
        }
    
        // Coroutine to fade out the loading UI smoothly
        private IEnumerator LoadUpAnimation(float begin,float end, float duration,Action callBack)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                cg.alpha = Mathf.Lerp(begin, end, Mathf.Clamp01(elapsed / duration));
                yield return null;
            }
            cg.alpha = end;

            _co = null;
            callBack?.Invoke();
        }
    }
}
