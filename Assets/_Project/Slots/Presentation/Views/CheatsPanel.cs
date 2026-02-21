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

        private void OnEnable()
        {
            if (_Input != null)
            {
                _Input.onSubmit.AddListener(OnSubmit);
            }
        }

        private void OnDisable()
        {
            if (_Input != null)
            {
                _Input.onSubmit.RemoveListener(OnSubmit);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha2))
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

            if (next && _Input != null)
            {
                _Input.text = string.Empty;
                _Input.ActivateInputField();
            }
        }

        public void Close()
        {
            if (_Root == null)
            {
                return;
            }

            _Root.SetActive(false);
        }

        private void OnSubmit(string text)
        {
            bool ok = TryApply(text, out string message);

            if (_Message != null)
            {
                _Message.text = message;
            }

            if (!ok && _Input != null)
            {
                _Input.ActivateInputField();
            }
        }

        private bool TryApply(string text, out string message)
        {
            message = string.Empty;

            if (!CheatParser.TryParse(text, out CheatRequest req, out string err))
            {
                message = err;
                return false;
            }

            if (GameManager.Instance == null)
            {
                message = "GameManager not available.";
                return false;
            }

            var provider = GameManager.Instance.GetCheatProvider();
            if (provider == null)
            {
                message = "CheatProvider not available.";
                return false;
            }

            provider.Queue(req);
            message = "Cheat enqueued for next spin.";
            return true;
        }
    }
}