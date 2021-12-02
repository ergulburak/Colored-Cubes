using UnityEngine;

namespace Utility
{
    /// <summary>
    /// Arkaplan müziğinin kesilmeden çalmasını sağlar.
    /// </summary>
    public class BackgroundMusic : MonoBehaviour
    {
        private void Awake()
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("Music");

            if (objs.Length > 1)
            {
                Destroy(this.gameObject);
            }

            DontDestroyOnLoad(this.gameObject);
        }
    }
}