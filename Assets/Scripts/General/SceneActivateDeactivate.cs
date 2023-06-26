﻿using Enemies.BT;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace General
{
    public class SceneActivateDeactivate : MonoBehaviour
    {
        [SerializeField] private GameObject scene;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private WavesManager _wavesManager;

        private void OnEnable()
        {
            if (GameManager.Instance.players.Count > 0)
                GameManager.Instance.players[0].PlayerMovement.ResetTo(_spawnPoint);
            else
            {
                FindObjectOfType<MovementVR>().ResetTo(_spawnPoint);
            }

            scene.SetActive(false);
            EventManager.LoadingStarts += HideScene;
            EventManager.LoadingEnds += ShowScene;
        }

        private void OnDisable()
        {
            EventManager.LoadingStarts -= HideScene;
            EventManager.LoadingEnds -= ShowScene;
        }

        private void ShowScene()
        {
            scene.SetActive(true);
            if (SceneManager.GetActiveScene().name == "E_SinglePlayerScene")
                GameManager.Instance.players[0].PlayerIngameCanvas.EnableCanvas();
            else
                GameManager.Instance.players[0].PlayerIngameCanvas.DisableCanvas();
        }

        private void HideScene() => scene.SetActive(false);
    }
}