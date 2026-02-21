using Project.Slots.Presentation.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Slots.Presentation.Views
{
    /// <summary>
    /// Controls the interactable state of the Spin button based on the spin lifecycle events.
    /// </summary>
    /// <remarks>
    /// Disables the button when a spin starts and enables it again once the reels have visually stopped.
    /// </remarks>
    [RequireComponent(typeof(Button))]
    public sealed class SpinButtonController : MonoBehaviour
    {
        private Button _SpinButton;

        private void Awake()
        {
            _SpinButton = GetComponent<Button>();
        }

        private void OnEnable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnSpinStarted += HandleSpinStarted;
                GameManager.Instance.OnReelsStopped += HandleSpinFinished;
            }
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnSpinStarted -= HandleSpinStarted;
                GameManager.Instance.OnReelsStopped -= HandleSpinFinished;
            }
        }

        private void HandleSpinStarted()
        {
            if (_SpinButton != null)
            {
                _SpinButton.interactable = false;
            }
        }

        private void HandleSpinFinished()
        {
            if (_SpinButton != null)
            {
                _SpinButton.interactable = true;
            }
        }
    }
}