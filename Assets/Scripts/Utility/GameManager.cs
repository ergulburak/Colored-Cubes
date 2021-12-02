using System;
using System.Collections.Generic;
using Entities;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Utility
{
    /// <summary>
    /// Bu sınıf oyun durumunu yönetir.
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        public enum GameState
        {
            Menu,
            Start,
            Win,
            Lose
        }

        public PlayerController.ActiveColor activeColor = PlayerController.ActiveColor.Blue;
        public int currentLevel;

        private GameState _state = GameState.Menu;


        /// <summary>
        /// Description:
        /// Güncel seviyeyi çağırır ve durum metodunu çağırır.
        /// Inputs: 
        /// none
        /// Returns: 
        /// void
        /// </summary>
        private void Start()
        {
            currentLevel = PlayerPrefs.GetInt("currentLevel", 1);
            Manager();
        }

        /// <summary>
        /// Description:
        /// Ekrana basıldığında oyunu başlatır.
        /// Inputs: 
        /// none
        /// Returns: 
        /// void
        /// </summary>
        private void Update()
        {
            if (FindObjectOfType<DynamicJoystick>().IsPointerDown && _state == GameState.Menu)
            {
                _state = GameState.Start;
                Manager();
            }
        }

        /// <summary>
        /// Description:
        /// Hangi durumda ne yapılması gerektiğine karar verir.
        /// Inputs: 
        /// none
        /// Returns: 
        /// void
        /// </summary>
        private void Manager()
        {
            switch (_state)
            {
                case GameState.Win:
                    currentLevel += 1;
                    PlayerPrefs.SetInt("currentLevel", currentLevel);
                    break;
                case GameState.Menu:
                    LevelGenerator.Instance.GenerateLevel(PlayerPrefs.GetInt("activeCube", 0));
                    break;
                case GameState.Lose:
                    PlayerController.Instance.enabled = false;
                    UIManager.Instance.LoseGame();
                    break;
                case GameState.Start:
                    PlayerController.Instance.enabled = true;
                    UIManager.Instance.StartGame();
                    break;
                default:
                    Debug.Log("GameStateError");
                    break;
            }
        }

        /// <summary>
        /// Description:
        /// Oyun durumunu günceller ve işleme sokar.
        /// Inputs: 
        /// GameState gameState
        /// Returns: 
        /// void
        /// </summary>
        public void SetState(GameState gameState)
        {
            _state = gameState;
            Manager();
        }

        /// <summary>
        /// Description:
        ///  Oyun durumunu döndürür.
        /// Inputs: 
        /// none
        /// Returns: 
        /// GameState
        /// </summary>
        /// <returns>GameState: Oyun durumu</returns>
        public GameState GetState()
        {
            return _state;
        }
    }
}