using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace CGUsed
{
    /// <summary>
    /// Attach to the EnterPlayerTrigger (a Trigger Collider).
    /// When Player enters, do a black fade in/out and then fire an event
    /// (e.g. start CG video).
    /// </summary>
    public class EnterPlayerTriggerFade : MonoBehaviour
    {
        [Header("Trigger")]
        public string playerTag = "Player";
        public bool triggerOnce = true;

        [Header("Fade")]
        public ScreenFader fader;
        public float fadeToBlackDuration = 0.6f;
        public float fadeFromBlackDuration = 0.6f;
        public float holdBlackSeconds = 0.0f;

        [Header("Events")]
        public UnityEvent onFadeFromBlackFinished;

        bool _triggered;

        void Reset()
        {
            // Make sure the collider is set as Trigger.
            var col = GetComponent<Collider>();
            if (col != null) col.isTrigger = true;
        }

        void OnTriggerEnter(Collider other)
        {
            if (triggerOnce && _triggered) return;
            if (!other.CompareTag(playerTag)) return;

            _triggered = true;
            StartCoroutine(Run());
        }

        IEnumerator Run()
        {
            if (fader == null)
            {
                onFadeFromBlackFinished?.Invoke();
                yield break;
            }

            bool done = false;
            fader.FadeTo(1f, fadeToBlackDuration, () => done = true);
            while (!done) yield return null;

            if (holdBlackSeconds > 0f)
                yield return new WaitForSecondsRealtime(holdBlackSeconds);

            done = false;
            fader.FadeTo(0f, fadeFromBlackDuration, () => done = true);
            while (!done) yield return null;

            onFadeFromBlackFinished?.Invoke();
        }
    }
}

