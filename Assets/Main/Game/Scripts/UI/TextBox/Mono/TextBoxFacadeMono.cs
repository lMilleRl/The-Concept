using System.Collections;
using UnityEngine;

namespace TextBox
{
    public class TextBoxFacadeMono : MonoBehaviour, ICoroutineRunner
    {
        public static TextBoxFacadeMono Instance { get; private set; }

        private TextBoxFacade _facade;

        public TextBoxFacade Facade => _facade;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void Init(TextBoxFacade facade)
        {
            _facade = facade;
        }

        public void Show(TextBoxData data) => _facade.Show(data);
        public void Hide() => _facade.Hide();
    }
}
