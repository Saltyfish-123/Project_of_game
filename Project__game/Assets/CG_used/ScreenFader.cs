using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CGUsed
{
    /// <summary>
    /// Fullscreen black fade using a CanvasGroup (recommended) or Image.
    /// </summary>
    public class ScreenFader : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Prefer a CanvasGroup on a full-screen black Image.")]
        public CanvasGroup canvasGroup;

        [Tooltip("Optional. If CanvasGroup is not assigned, will use this Image alpha.")]
        public Image fallbackImage;

        [Header("Behavior")]
        public bool blocksRaycastsWhenVisible = true;

        Coroutine _fadeRoutine;

        public float Alpha
        {
            get
            {
                if (canvasGroup != null) return canvasGroup.alpha;
                if (fallbackImage != null) return fallbackImage.color.a;
                return 0f;
            }
            set
            {
                value = Mathf.Clamp01(value);
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = value;
                    if (blocksRaycastsWhenVisible)
                        canvasGroup.blocksRaycasts = value > 0.001f;
                    return;
                }

                if (fallbackImage != null)
                {
                    var c = fallbackImage.color;
                    c.a = value;
                    fallbackImage.color = c;
                    if (blocksRaycastsWhenVisible)
                        fallbackImage.raycastTarget = value > 0.001f;
                }
            }
        }

        void Reset()
        {
            canvasGroup = GetComponentInChildren<CanvasGroup>(true);
            fallbackImage = GetComponentInChildren<Image>(true);
        }

        void Awake()
        {
            // Ensure a known starting state.
            Alpha = Alpha;
        }

        public void SetAlpha(float alpha) => Alpha = alpha;

        public void FadeTo(float targetAlpha, float duration) =>
            FadeTo(targetAlpha, duration, null);

        public void FadeTo(float targetAlpha, float duration, System.Action onComplete)
        {
            if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
            _fadeRoutine = StartCoroutine(FadeRoutine(Mathf.Clamp01(targetAlpha), Mathf.Max(0f, duration), onComplete));
        }

        IEnumerator FadeRoutine(float targetAlpha, float duration, System.Action onComplete)
        {
            float start = Alpha;
            if (duration <= 0f)
            {
                Alpha = targetAlpha;
                onComplete?.Invoke();
                _fadeRoutine = null;
                yield break;
            }

            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float u = Mathf.Clamp01(t / duration);
                Alpha = Mathf.Lerp(start, targetAlpha, u);
                yield return null;
            }

            Alpha = targetAlpha;
            onComplete?.Invoke();
            _fadeRoutine = null;
        }
    }
}

