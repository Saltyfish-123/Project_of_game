using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

namespace CGUsed
{
    /// <summary>
    /// Play a CG video (VideoClip) after being triggered.
    /// When the video ends, it fades to black, hides the video, fades back,
    /// then invokes an event (e.g. show dialogue).
    /// </summary>
    public class CGVideoSequence : MonoBehaviour
    {
        [Header("References")]
        public ScreenFader fader;
        public VideoPlayer videoPlayer;

        [Tooltip("Optional root to enable/disable while playing (e.g. RawImage UI).")]
        public GameObject videoRoot;

        [Header("Video")]
        public VideoClip clip;
        public bool playOnAwake = false;

        [Header("Post-video fade")]
        public float fadeToBlackDuration = 0.6f;
        public float holdBlackSeconds = 0.0f;
        public float fadeFromBlackDuration = 0.6f;

        [Header("Events")]
        public UnityEvent onVideoStarted;
        public UnityEvent onAfterVideoFadeFinished;

        bool _ending;
        Coroutine _playRoutine;

        void Reset()
        {
            videoPlayer = GetComponentInChildren<VideoPlayer>(true);
        }

        void Awake()
        {
            if (videoPlayer != null)
            {
                videoPlayer.playOnAwake = false;
                videoPlayer.isLooping = false;
                videoPlayer.loopPointReached -= OnVideoEnded;
                videoPlayer.loopPointReached += OnVideoEnded;
            }

            if (videoRoot != null)
                videoRoot.SetActive(false);
        }

        void OnDestroy()
        {
            if (videoPlayer != null)
                videoPlayer.loopPointReached -= OnVideoEnded;
        }

        public void Play()
        {
            if (_playRoutine != null)
                StopCoroutine(_playRoutine);

            _playRoutine = StartCoroutine(PlayRoutine());
        }

        IEnumerator PlayRoutine()
        {
            _ending = false;

            if (videoPlayer == null)
                yield break;

            if (clip != null)
                videoPlayer.clip = clip;

            if (videoRoot != null)
                videoRoot.SetActive(true);

            videoPlayer.Stop();
            videoPlayer.Play();
            onVideoStarted?.Invoke();

            // Safety: wait until it actually starts, then wait until it stops
            // (loopPointReached should handle normal endings).
            float startTimeout = 2f;
            while (startTimeout > 0f && !videoPlayer.isPlaying)
            {
                startTimeout -= Time.unscaledDeltaTime;
                yield return null;
            }

            while (!_ending && videoPlayer.isPlaying)
                yield return null;

            // If it stopped without loopPointReached (rare), still end the sequence.
            if (!_ending)
                yield return EndSequence();
        }

        void OnVideoEnded(VideoPlayer vp)
        {
            if (_ending) return;
            StartCoroutine(EndSequence());
        }

        IEnumerator EndSequence()
        {
            _ending = true;

            if (fader != null)
            {
                bool done = false;
                fader.FadeTo(1f, fadeToBlackDuration, () => done = true);
                while (!done) yield return null;

                if (holdBlackSeconds > 0f)
                    yield return new WaitForSecondsRealtime(holdBlackSeconds);
            }

            if (videoPlayer != null)
                videoPlayer.Stop();

            if (videoRoot != null)
                videoRoot.SetActive(false);

            if (fader != null)
            {
                bool done = false;
                fader.FadeTo(0f, fadeFromBlackDuration, () => done = true);
                while (!done) yield return null;
            }

            onAfterVideoFadeFinished?.Invoke();
        }
    }
}

