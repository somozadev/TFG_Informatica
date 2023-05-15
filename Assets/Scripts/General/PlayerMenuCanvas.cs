using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace General
{
    public class PlayerMenuCanvas : MonoBehaviour
    {
        [SerializeField] private InputAction _menuAction;
        [SerializeField] private bool _menuOpened;
        [SerializeField] private Canvas _canvas;

        private Toggle _rotationToggle;


        private void OnValidate()
        {
            if (_canvas == null)
                _canvas = GetComponentInChildren<Canvas>();

            _rotationToggle = _canvas.GetComponentInChildren<Toggle>();

            SetupEvents();
        }

        private void SetupEvents()
        {
            _rotationToggle.onValueChanged.AddListener(ChangeTurningMode);
            _menuAction.performed += ctx => OpenCloseMenu();
        }

        private void ChangeTurningMode(bool value)
        {
            GetComponentInParent<Player>().PlayerMovement
                .UpadteRotationType(!value ? MovementVR.RotationType.Snap : MovementVR.RotationType.Continuous);
        }
        private void OpenCloseMenu()
        {
            if (!_menuOpened)
            {
                _menuOpened = true;
                _canvas.gameObject.SetActive(true);
            }
            else
            {
                _menuOpened = false;
                _canvas.gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            _menuAction.Enable();
        }

        private void OnDisable()
        {
            _menuAction.Disable();
        }


       
    }
}