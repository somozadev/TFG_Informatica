using System;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace General.UI_StartScene
{
    public class UIController_StartScene_Multiplayer : MonoBehaviour
    {
        [SerializeField] private Button _FindGame;
        [SerializeField] private Button _CreateGame;
        [SerializeField] private Button _JoinGame;

        [SerializeField] private Button[] _backButtons;

        [SerializeField] private GameObject _MainInnerCanvas;
        [SerializeField] private GameObject _JoinGameInnerCanvas;
        [SerializeField] private GameObject _FindGameInnerCanvas;
        [SerializeField] private GameObject _CreateGameInnerCanvas;

        private void Awake()
        {
            SetupButtonEvents();
        }

        #region ButtonTransitionsMethods

        private void SetupButtonEvents()
        {
            _JoinGame.onClick.AddListener(JoinGameUI);
            _FindGame.onClick.AddListener(FindGameUI);
            _CreateGame.onClick.AddListener(CreateGameUI);
            foreach (Button backButton in _backButtons)
                backButton.onClick.AddListener(BackToMainCanvas);
        }

        private async void JoinGameUI()
        {
            // _MainInnerCanvas.SetActive(false);
            // _JoinGameInnerCanvas.SetActive(true);
            await DebuggingMultiplayerJoin();

        }

        private async void CreateGameUI()
        {
            // _MainInnerCanvas.SetActive(false);
            // _CreateGameInnerCanvas.SetActive(true);
            await DebuggingMultiplayer();
        }

        private void FindGameUI()
        {
            _MainInnerCanvas.SetActive(false);
            _FindGameInnerCanvas.SetActive(true);
        }

        private void BackToMainCanvas()
        {
            _MainInnerCanvas.SetActive(true);
            _JoinGameInnerCanvas.SetActive(false);
            _FindGameInnerCanvas.SetActive(false);
            _CreateGameInnerCanvas.SetActive(false);
        }

        #endregion

        private async Task DebuggingMultiplayer()
        {
            await GameManager.Instance.sceneController.LoadScene("TestingMultiplayer", LoadSceneMode.Additive);
            Destroy(GameManager.Instance.players[0].gameObject);
            GameManager.Instance.players.TrimExcess();
            GameManager.Instance.IsHost = true;
        }

        private async Task DebuggingMultiplayerJoin()
        {
            await GameManager.Instance.sceneController.LoadScene("TestingMultiplayer", LoadSceneMode.Additive);
            Destroy(GameManager.Instance.players[0].gameObject);
            GameManager.Instance.players.TrimExcess();
            GameManager.Instance.IsHost = false;
        }
        
        
        
    }
}