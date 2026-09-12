using UnityEngine;

namespace TextBox
{
    public class TextBoxPageIndicator : MonoBehaviour
    {
        [SerializeField] private TextBoxFacadeMono _facadeMono;
        [SerializeField] private GameObject _indicator;
        [SerializeField] private Animator _animator;

        private ITextBoxFacade _facade;
        private bool _isSubscribed;

        private void Awake()
        {
            if (_indicator == null)
            {
                Debug.LogWarning("[TextBoxPageIndicator] Indicator object is not assigned.", this);
                return;
            }

            if (_animator == null)
                _animator = _indicator.GetComponentInChildren<Animator>(true);

            _indicator.SetActive(false);
        }

        private void OnEnable()
        {
            if (_facade != null)
                Subscribe();
        }

        private void Start()
        {
            if (_facadeMono == null)
                _facadeMono = TextBoxFacadeMono.Instance;

            _facade = _facadeMono != null ? _facadeMono.Facade : null;
            if (_facade == null)
            {
                Debug.LogError("[TextBoxPageIndicator] TextBoxFacade is not initialized.", this);
                return;
            }

            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
            HideIndicator();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            if (_isSubscribed)
                return;

            _facade.OnPageStarted += HideIndicator;
            _facade.OnPageFinished += ShowIndicator;
            _facade.OnCurrentTextEnded += HideIndicator;
            _isSubscribed = true;
        }

        private void Unsubscribe()
        {
            if (!_isSubscribed || _facade == null)
                return;

            _facade.OnPageStarted -= HideIndicator;
            _facade.OnPageFinished -= ShowIndicator;
            _facade.OnCurrentTextEnded -= HideIndicator;
            _isSubscribed = false;
        }

        private void ShowIndicator()
        {
            if (_indicator == null)
                return;

            _indicator.SetActive(true);
            if (_animator != null)
                _animator.Rebind();
        }

        private void HideIndicator()
        {
            if (_indicator != null)
                _indicator.SetActive(false);
        }
    }
}
