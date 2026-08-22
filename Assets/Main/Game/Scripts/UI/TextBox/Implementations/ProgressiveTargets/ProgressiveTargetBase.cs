using System;
using UnityEngine;

namespace TextBox
{
    public abstract class ProgressiveTargetBase : MonoBehaviour, IProgressiveTarget
    {
        [SerializeField] private ProgressiveTargetId _id;

        public ProgressiveTargetId Id => _id;

        public float CurrentProgress { get; private set; }

        public event Action<float> ProgressChanged;

        public void SetProgress(float progress)
        {
            CurrentProgress = progress;
            ProgressChanged?.Invoke(progress);
            UpdateActiveState(progress);
            OnProgress(progress);
        }

        protected virtual void UpdateActiveState(float progress)
        {
            bool wasActive = gameObject.activeSelf;
            bool shouldBeActive = progress > 0f && progress < 1f;

            if (wasActive != shouldBeActive)
                gameObject.SetActive(shouldBeActive);
        }

        protected abstract void OnProgress(float progress);
    }
}
