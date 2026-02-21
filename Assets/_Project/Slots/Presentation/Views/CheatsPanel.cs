using Project.Slots.Domain.Cheats;
using Project.Slots.Presentation.Controllers;
using TMPro;
using UnityEngine;

namespace Project.Slots.Presentation.Views
{
    public class CheatsPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _Root;
        [SerializeField] private TMP_InputField _Input;
        [SerializeField] private TMP_Text _Message;

        private void Awake()
        {
            if (_Root != null)
            {
                _Root.SetActive(false);
            }

            if (_Message != null)
            {
                _Message.text = string.Empty;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Toggle();
            }
        }

        public void Toggle()
        {
            if (_Root == null)
            {
                return;
            }

            bool next = !_Root.activeSelf;
            _Root.SetActive(next);

            if (_Message != null)
            {
                _Message.text = string.Empty;
            }

            if (next)
            {
                if (_Input != null)
                {
                    _Input.text = string.Empty;
                    _Input.ActivateInputField();
                }
            }
        }

        public void Close()
        {
            if (_Root == null)
            {
                return;
            }

            string text = string.Empty;

            if (_Input != null)
            {
                text = _Input.text;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                _Root.SetActive(false);

                if (_Message != null)
                {
                    _Message.text = string.Empty;
                }

                return;
            }

            bool ok = TryApply(text, out string message);

            if (_Message != null)
            {
                _Message.text = message;
            }

            if (ok)
            {
                _Root.SetActive(false);

                if (_Input != null)
                {
                    _Input.text = string.Empty;
                }

                return;
            }

            if (_Input != null)
            {
                _Input.ActivateInputField();
            }
        }

        private bool TryApply(string text, out string message)
        {
            message = string.Empty;

            if (!CheatParser.TryParse(text, out CheatRequest request, out string error))
            {
                message = error;
                return false;
            }

            if (GameManager.Instance == null)
            {
                message = "GameManager not available.";
                return false;
            }

            var provider = GameManager.Instance.CheatProvider;
            if (provider == null)
            {
                message = "CheatProvider not available.";
                return false;
            }

            provider.Queue(request);
            return true;
        }
    }
}