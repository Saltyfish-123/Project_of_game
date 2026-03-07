using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace CGUsed
{
    /// <summary>
    /// Simple linear dialogue. Assign a list of lines in Inspector.
    /// Call StartDialogue(), then hook Next() to a UI Button.
    /// </summary>
    public class LinearDialogue : MonoBehaviour
    {
        [Header("UI")]
        [Tooltip("Optional root panel to show/hide.")]
        public GameObject dialogueRoot;

        [Tooltip("Optional: blocks clicks when dialogue is active.")]
        public CanvasGroup dialogueCanvasGroup;

        [Tooltip("Dialogue text (TMP).")]
        public TMP_Text dialogueText;

        [Tooltip("Optional Next button to auto-enable/disable.")]
        public Button nextButton;

        [Header("Dialogue Content")]
        [TextArea(2, 6)]
        public List<string> lines = new List<string>();

        [Header("Events")]
        public UnityEvent onDialogueStarted;
        public UnityEvent onDialogueFinished;

        int _index = -1;
        bool _active;

        void Awake()
        {
            SetActive(false);
        }

        public void StartDialogue()
        {
            if (lines == null || lines.Count == 0)
            {
                Finish();
                return;
            }

            _active = true;
            _index = -1;
            SetActive(true);
            onDialogueStarted?.Invoke();
            Next();
        }

        public void Next()
        {
            if (!_active)
                return;

            _index++;
            if (_index >= (lines?.Count ?? 0))
            {
                Finish();
                return;
            }

            if (dialogueText != null)
                dialogueText.text = lines[_index];
        }

        void Finish()
        {
            _active = false;
            SetActive(false);
            onDialogueFinished?.Invoke();
        }

        void SetActive(bool value)
        {
            if (dialogueRoot != null)
                dialogueRoot.SetActive(value);

            if (dialogueCanvasGroup != null)
            {
                dialogueCanvasGroup.alpha = value ? 1f : 0f;
                dialogueCanvasGroup.blocksRaycasts = value;
                dialogueCanvasGroup.interactable = value;
            }

            if (nextButton != null)
                nextButton.interactable = value;
        }
    }
}

