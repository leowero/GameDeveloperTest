using Project.Slots.Domain.Engine;
using Project.Slots.Presentation.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Slots.Presentation.Views
{
    public class SpinButtonController : MonoBehaviour
    {
        private Button _SpinButton;

        private void Awake()
        {
            _SpinButton = GetComponent<Button>();
        }

        private void Start()
        {
            GameManager.Instance.OnSpinResolved += HandleSpinStarted;
            GameManager.Instance.OnReelsStopped += HandleSpinFinished;
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnSpinResolved -= HandleSpinStarted;
                GameManager.Instance.OnReelsStopped -= HandleSpinFinished;
            }
        }

        private void HandleSpinStarted(SpinResult _)
        {
            GameManager.Instance.OnSpinStarted += () => _SpinButton.interactable = false;
        }

        private void HandleSpinFinished()
        {
            _SpinButton.interactable = true;
        }
    }
}