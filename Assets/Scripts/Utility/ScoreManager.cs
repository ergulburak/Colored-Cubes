using UnityEngine;

namespace Utility
{
    /// <summary>
    /// Skor hesabını tutan sınıf
    /// </summary>
    public class ScoreManager : Singleton<ScoreManager>
    {
        [SerializeField] private int score;
        [SerializeField] private int totalScore;
        [SerializeField] private int multiplier;
        
        /// <summary>
        /// Description:
        /// Başlangıç tamalarını yapar.
        /// Inputs:
        /// None
        /// Returns:
        /// void
        /// </summary>
        void Start()
        {
            score = 0;
            multiplier = 0;
            totalScore = PlayerPrefs.GetInt("Score", 0);
        }

        /// <summary>
        /// Description:
        /// Çarpanı yükseltir.
        /// Inputs:
        /// None
        /// Returns:
        /// void
        /// </summary>
        public void IncreaseMultiplier()
        {
            multiplier += 1;
        }

        /// <summary>
        /// Description:
        /// Skoru yükseltir.
        /// Inputs:
        /// None
        /// Returns:
        /// void
        /// </summary>
        public void IncreaseScore()
        {
            score += 1;
        }

        /// <summary>
        /// Description:
        /// Skoru düşürür.
        /// Inputs:
        /// None
        /// Returns:
        /// void
        /// </summary>
        public void DecreaseScore()
        {
            score -= 1;
        }

        /// <summary>
        /// Description:
        /// Skor değerini döndürür.
        /// Inputs:
        /// None
        /// Returns:
        /// int
        /// </summary>
        /// /// <returns>int: Skor değeri</returns>
        public int GetScore()
        {
            return score;
        }

        /// <summary>
        /// Description:
        /// Çarpan değerini döndürür.
        /// Inputs:
        /// None
        /// Returns:
        /// int
        /// </summary>
        /// /// <returns>int: Bitiş etkinliğindeki çarpan değeri</returns>
        public int GetMultiplier()
        {
            return multiplier;
        }

        /// <summary>
        /// Description:
        /// Oyun bitince skoru kayıt eder.
        /// Inputs:
        /// None
        /// Returns:
        /// void
        /// </summary>
        public void EndGame()
        {
            if (multiplier > 0)
            {
                totalScore += score * multiplier;
            }
            else
            {
                totalScore += score;
            }

            PlayerPrefs.SetInt("Score", totalScore);
        }
    }
}