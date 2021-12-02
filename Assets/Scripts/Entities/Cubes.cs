using UnityEngine;

namespace Entities
{
    /// <summary>
    /// Bu sınıf küpler etkileşime girdiğinde kontrolcüye yönlendiriyor.
    /// </summary>
    public class Cubes : MonoBehaviour
    {
        public PlayerController.ActiveColor activeColor = PlayerController.ActiveColor.Blue;

        private void Start()
        {
            //Zar şeklindeki modeller için rastgele rotasyon atamasını yapıyor.
            transform.GetChild(transform.childCount - 1).transform.eulerAngles =
                new Vector3(RandomAngle(), RandomAngle(), 0f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (activeColor == PlayerController.ActiveColor.Player)
            {
                PlayerController.Instance.OnChildTriggerEnter(other);
            }
        }

        /// <summary>
        /// Description:
        /// 0, 90, 180, 270 değerlerinden bir tanesini rastgele döndürür.
        /// Inputs:
        /// None
        /// Returns:
        /// int
        /// </summary>
        /// <returns>int: 0, 90, 180, 270 değerlerinden bir tanesini rastgele döndürür.</returns>
        private int RandomAngle()
        {
            int temp = Random.Range(0, 4);
            return temp == 0 ? 0 : temp == 1 ? 90 : temp == 2 ? 180 : 270;
        }
    }
}