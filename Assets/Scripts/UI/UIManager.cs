using System;
using Entities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;
using UnityEngine.SceneManagement;


namespace UI
{
    /// <summary>
    /// Bu sınıf ui sayfalarını ve bileşenlerini kontrol eder.
    /// </summary>
    public class UIManager : Singleton<UIManager>
    {
        [Header("Pages")] [SerializeField] private GameObject mainPage;
        [SerializeField] private GameObject gamePage;
        [SerializeField] private GameObject winPage;
        [SerializeField] private GameObject losePage;
        [SerializeField] private GameObject selectionPage;

        [Header("Game Page Components")] [SerializeField]
        private TextMeshProUGUI currentLevelText;

        [SerializeField] private Slider levelProgressSlider;
        [SerializeField] private TextMeshProUGUI scoreText;

        [Header("Main Page Components")] [SerializeField]
        private TextMeshProUGUI totalScoreText;

        [SerializeField] private GameObject swipeImage;

        [Header("Win Page Components")] [SerializeField]
        private TextMeshProUGUI winScoreText;

        [Header("Selection Page Components")] [SerializeField]
        private Toggle toggle1;

        [SerializeField] private Toggle toggle2;
        [SerializeField] private Button selectionButton;


        /// <summary>
        /// Description:
        /// Anasayfayı açar, toggle dinleyicilerini atar ve ayarlar, skoru getirir.
        /// Inputs: 
        /// none
        /// Returns: 
        /// void (no return)
        /// </summary>
        private void Start()
        {
            mainPage.SetActive(true);
            winPage.SetActive(false);
            losePage.SetActive(false);
            gamePage.SetActive(false);
            swipeImage.SetActive(true);
            totalScoreText.text = PlayerPrefs.GetInt("Score", 0).ToString();
            if (PlayerPrefs.GetInt("activeCube", 0) == 0)
            {
                toggle1.isOn = true;
                toggle2.isOn = false;
            }
            else
            {
                toggle2.isOn = true;
                toggle1.isOn = false;
            }

            toggle1.onValueChanged.AddListener(delegate { Toggle1(toggle1); });
            toggle2.onValueChanged.AddListener(delegate { Toggle2(toggle2); });
        }

        /// <summary>
        /// Description:
        /// Aktif küpü değiştirip sahneyi yeniler.
        /// Inputs: 
        /// Toggle
        /// Returns: 
        /// void (no return)
        /// </summary>
        private void Toggle1(Toggle isOn)
        {
            if (isOn.isOn)
            {
                PlayerPrefs.SetInt("activeCube", 0);
                NextLevel();
            }
        }

        /// <summary>
        /// Description:
        /// Aktif küpü değiştirip sahneyi yeniler.
        /// Inputs: 
        /// Toggle
        /// Returns: 
        /// void (no return)
        /// </summary>
        private void Toggle2(Toggle isOn)
        {
            if (isOn.isOn)
            {
                PlayerPrefs.SetInt("activeCube", 1);
                NextLevel();
            }
        }

        /// <summary>
        /// Description:
        /// Sayfaları oyun pozisyonuna getirir, progress barı ve şuanki seviyeyi ayarlar.
        /// Inputs: 
        /// none
        /// Returns: 
        /// void (no return)
        /// </summary>
        public void StartGame()
        {
            mainPage.SetActive(false);
            winPage.SetActive(false);
            losePage.SetActive(false);
            gamePage.SetActive(true);
            swipeImage.SetActive(false);
            levelProgressSlider.maxValue = LevelGenerator.Instance.GetDistance();
            currentLevelText.text = GameManager.Instance.currentLevel.ToString();
        }

        /// <summary>
        /// Description:
        /// UI'ları kazanma durumuna göre hazırlar.
        /// Inputs: 
        /// none
        /// Returns: 
        /// void (no return)
        /// </summary>
        public void WinGame()
        {
            mainPage.SetActive(false);
            winPage.SetActive(true);
            losePage.SetActive(false);
            gamePage.SetActive(false);
            winScoreText.text = ScoreManager.Instance.GetScore().ToString();
        }

        /// <summary>
        /// Description:
        /// UI'ları kaybetme durumuna göre hazırlar.
        /// Inputs: 
        /// none
        /// Returns: 
        /// void (no return)
        /// </summary>
        public void LoseGame()
        {
            mainPage.SetActive(false);
            winPage.SetActive(false);
            losePage.SetActive(true);
            gamePage.SetActive(false);
        }

        /// <summary>
        /// Description:
        /// Sahneyi tekrar yükler.
        /// Inputs: 
        /// none
        /// Returns: 
        /// void (no return)
        /// </summary>
        public void NextLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// Description:
        /// UI'ları seçim ekranına göre hazırlar.
        /// Inputs: 
        /// none
        /// Returns: 
        /// void (no return)
        /// </summary>
        public void OpenSelection()
        {
            selectionButton.gameObject.SetActive(false);
            selectionPage.SetActive(true);
            swipeImage.SetActive(false);
        }

        /// <summary>
        /// Description:
        /// UI'ları ana ekrana göre hazırlar.
        /// Inputs: 
        /// none
        /// Returns: 
        /// void (no return)
        /// </summary>
        public void CloseSelection()
        {
            selectionButton.gameObject.SetActive(true);
            selectionPage.SetActive(false);
            swipeImage.SetActive(true);
        }

        /// <summary>
        /// Description:
        /// Seviye durumunu gösterir ve kazanma durumunda skoru anime eder.
        /// Inputs: 
        /// none
        /// Returns: 
        /// void (no return)
        /// </summary>
        private void Update()
        {
            if (GameManager.Instance.GetState() == GameManager.GameState.Start)
            {
                levelProgressSlider.value = PlayerController.Instance.transform.position.z;
                scoreText.text = ScoreManager.Instance.GetScore().ToString();
            }

            // Skoru çarpan değerine yaklaştırır.
            if (winPage.activeSelf)
            {
                if (Convert.ToInt32(winScoreText.text) <
                    (ScoreManager.Instance.GetScore() * ScoreManager.Instance.GetMultiplier()))
                    winScoreText.text = (Convert.ToInt32(winScoreText.text) + Time.deltaTime * 100).ToString("000");
            }
        }
    }
}