using System.Collections.Generic;
using System.Linq;
using Entities;
using UnityEngine;

namespace Utility
{
    /// <summary>
    /// Bu sınıf seviye oluşturulmasını sağlar.
    /// </summary>
    public class LevelGenerator : Singleton<LevelGenerator>
    {
        [Header("Settings")] [SerializeField] private List<GameObject> cubes;
        [SerializeField] private List<Material> cubeColors = new List<Material>(3);
        [SerializeField] private List<Material> changerColors = new List<Material>(3);
        [SerializeField] private List<GameObject> barriersAndGates;
        [SerializeField] private float distance = 240f; // 30~ saniye
        [SerializeField] private float offset = 24f; // 3~ saniye
        [SerializeField] private float frequency = 10f;
        [SerializeField] private float roadScale = 6f;
        [SerializeField] private GameObject parent;
        private int _lastBarrierIndex = -1; //Son bariyer işaretçisi

        public float GetDistance()
        {
            return distance;
        }

        /// <summary>
        /// Description:
        /// Seviyeyi oluşturur.
        /// Inputs: 
        /// int diceSet
        /// Returns: 
        /// void (no return)
        /// </summary>
        /// <param name="diceSet">Küp seti numarası</param>
        public void GenerateLevel(int diceSet)
        {
            GeneratePlayer(diceSet);

            //Yol uzunluğuna göre kaç bileşen koyulacağını belirler ardından dizi içerisini
            //rastgele doldurur.
            float[] levelItems = new float[(int)((distance - offset) / frequency)];
            for (int i = 0; i < levelItems.Length; i++)
            {
                levelItems[i] = Random.Range(0, 2);
                if (i > 1)
                {
                    if (levelItems[i - 1] == 1 && levelItems[i] == 1)
                    {
                        levelItems[i] = 0;
                    }
                }
                else
                    levelItems[i] = 0;
            }

            //Oluşan dizinin öğelerine göre objeleri klonlar.
            float currentDistance = offset;
            for (int k = 0; k < levelItems.Length; k++)
            {
                switch (levelItems[k])
                {
                    case 0:
                        GenerateCubeLine(diceSet, currentDistance);
                        break;
                    case 1:
                        GenerateBarrier(currentDistance);
                        break;
                    default:
                        Debug.Log("Generate Level Error");
                        break;
                }

                currentDistance += 10;
            }
        }

        /// <summary>
        /// Description:
        /// Oyuncu objesini oluşturur ve renk ayarlarını yapar.
        /// Inputs: 
        /// int diceSet
        /// Returns: 
        /// void (no return)
        /// </summary>
        /// <param name="diceSet">Küp seti numarası</param>
        private void GeneratePlayer(int diceSet)
        {
            List<int> colorIndex = UniqueNumberList(3);
            GameObject temp = Instantiate(cubes[diceSet],
                new Vector3(0, 0, 0), Quaternion.identity);
            temp.GetComponent<Cubes>().activeColor = (PlayerController.ActiveColor)3;
            temp.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial =
                cubeColors[colorIndex.First()];
            temp.transform.parent = PlayerController.Instance.transform;
            GameManager.Instance.activeColor = (PlayerController.ActiveColor)colorIndex.First();
            _lastBarrierIndex = colorIndex.First();
        }

        /// <summary>
        /// Description:
        /// Bariyer klonlar ve renk ayarını yapar.
        /// Inputs: 
        /// float currentDistance
        /// Returns: 
        /// void (no return)
        /// </summary>
        /// <param name="currentDistance">Bariyerin koyulacağı mesafe</param>
        private void GenerateBarrier(float currentDistance)
        {
            List<int> colorIndex = UniqueNumberList(3);
            if (_lastBarrierIndex == colorIndex.First())
                colorIndex.RemoveAt(0);
            _lastBarrierIndex = colorIndex.First();
            GameObject temp = Instantiate(barriersAndGates[Random.Range(0, barriersAndGates.Count)],
                new Vector3(0, 0, currentDistance), Quaternion.identity);
            temp.transform.GetChild(0).GetComponent<Changers>().activeColor =
                (PlayerController.ActiveColor)colorIndex.First();
            temp.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial =
                changerColors[(int)temp.transform.GetChild(0).GetComponent<Changers>().activeColor];
            temp.transform.parent = parent.transform;
        }

        /// <summary>
        /// Description:
        /// 3 tane küp klonlar ve renk ayarını yapar.
        /// Inputs:
        /// int diceSet
        /// float currentDistance
        /// Returns: 
        /// void (no return)
        /// </summary>
        /// <param name="diceSet">Küp seti numarası</param>
        /// /// <param name="currentDistance">Küplerin koyulacağı mesafe</param>
        private void GenerateCubeLine(int diceSet, float currentDistance)
        {
            List<int> colorIndex = UniqueNumberList(3);
            float xPosition = -(roadScale / 3);
            for (int j = 0; j < 3; j++)
            {
                GameObject temp = Instantiate(cubes[diceSet],
                    new Vector3(xPosition, 0, currentDistance), Quaternion.identity);
                temp.GetComponent<Cubes>().activeColor = (PlayerController.ActiveColor)colorIndex.IndexOf(j);
                temp.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial =
                    cubeColors[(int)temp.GetComponent<Cubes>().activeColor];
                temp.transform.parent = parent.transform;
                xPosition += roadScale / 3;
            }
        }

        /// <summary>
        /// Description:
        /// Verilen aralıkta, aralık büyüklüğünde karışmış liste verir.
        /// Inputs:
        /// int range
        /// Returns: 
        /// List<int />
        /// </summary>
        /// <returns>List<int />: Eşsiz rastgele sayı döndürür.</returns>
        /// <param name="range">Liste ve sayı büyüklüğü</param>
        private List<int> UniqueNumberList(int range)
        {
            List<int> randomList = new List<int>();
            int temp;
            for (int i = 0; i < range; i++)
            {
                do
                {
                    temp = Random.Range(0, range);
                } while (randomList.Contains(temp));

                randomList.Add(temp);
            }

            return randomList;
        }
    }
}