using System;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using Utility;

namespace Entities
{
    /// <summary>
    /// Bu sınıf oyuncunun başına gelebilecek bütün olayları kontrol eder.
    /// </summary>
    public class PlayerController : Singleton<PlayerController>
    {
        /// <summary>
        /// Renk ve durumu kontrol eden enum
        /// </summary>
        public enum ActiveColor
        {
            Blue = 0,
            Red = 1,
            Green = 2,
            Player,
            Destroyed
        }

        [SerializeField] private DynamicJoystick joystick;

        [Header("Color Materials")]
        [Tooltip("Sıralaması enum sırası ile aynı olmalıdır. Yani Blue = 0, Red = 1, Green = 2 olmalıdır.")]
        [SerializeField]
        private List<Material> colors = new List<Material>(3);

        private float _roadMultiplier;
        private Rigidbody _rb;
        private bool _crash = false; //Engele çarptıktan sonraki fiziği kontrol eden boolean
        private AudioSource _audioSource;
        private List<GameObject> _childCubes = new List<GameObject>();//Oyuncudaki küpleri tutan liste

        [Header("Movement Settings")] public float speed = 8.0f;
        public float jumpForce = 10.0f;
        public float fallMultiplier = 2.5f;

        [Header("Road Settings")] public float roadScale;

        [Header("Audio Clips")] public AudioClip cubeAdd;
        public AudioClip cubeReduce;
        public AudioClip cubeWinReduce;
        public AudioClip winSound;
        public AudioClip loseSound;

        [Header("Confetti Prefabs")] public GameObject confetti;
        public GameObject confettiWin;
        
        /// <summary>
        /// Description:
        /// Oyuncudaki küp sayısını döndürür.
        /// Inputs: 
        /// none
        /// Returns: 
        /// void (no return)
        /// </summary>
        public int CubeCount()
        {
            return _childCubes.Count;
        }

        /// <summary>
        /// Bileşen atamaları
        /// </summary>
        void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _roadMultiplier = (roadScale - 1) / 2;
            joystick = FindObjectOfType<DynamicJoystick>();
            _childCubes.Add(transform.GetChild(0).gameObject);
            _audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Oyuncu fonksiyonlarını çağırır.
        /// </summary>
        private void FixedUpdate()
        {
            MoveForward();
            MoveSideways();
            FallSetting();
            DropTheCubes();
        }
        
        /// <summary>
        /// Description:
        /// Oyuncuyu bitiş etkinliği çizgisine gelmediği sürece ivme ile haretket ettirir.
        /// Eğer bitiş etkinliği çizgisine gelmişse Translate ile sabit hızda harekete geçer.
        /// Inputs: 
        /// none
        /// Returns: 
        /// void (no return)
        /// </summary>
        private void MoveForward()
        {
            if (_rb.isKinematic)
            {
                if (_childCubes.Count > 0)
                {
                    transform.position = new Vector3(0.0f, transform.position.y, transform.position.z);
                    transform.Translate(0.0f, 0.0f, speed * Time.deltaTime);
                }
            }
            else
                _rb.velocity = new Vector3(_rb.velocity.x, _rb.velocity.y, speed);
        }
        
        /// <summary>
        /// Description:
        /// Oyuncuyu joystickten aldığı değere göre sağa veya sola doğru hareket ettirir.
        /// Inputs: 
        /// joystick.Horizontal
        /// Returns: 
        /// void (no return)
        /// </summary>
        private void MoveSideways()
        {
            if ((transform.position.x < roadScale / 2 ||
                 transform.position.x > Math.Abs(roadScale / 2)) &&
                joystick.IsPointerDown && !_rb.isKinematic)
            {
                var position = transform.position;
                position = new Vector3(joystick.Horizontal * _roadMultiplier, position.y,
                    position.z);
                
                //Hareketin yumuşak olması için Vector3.Lerp metodundan geçirerek hareket ettirir.
                transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * speed);
            }
        }
        
        /// <summary>
        /// Description:
        /// Engele çarptıktan sonra her küpü sırasıyla zemine yaklaştırır.
        /// Inputs: 
        /// none
        /// Returns: 
        /// void (no return)
        /// </summary>
        private void DropTheCubes()
        {
            if (_crash && !_rb.isKinematic)
            {
                if (_childCubes.Last().transform.position.y == 0)
                    _crash = false;

                else
                {
                    int temp = 0;
                    for (int i = _childCubes.Count - 1; i >= 0; i--)
                    {
                        _childCubes[i].transform.localPosition = Vector3.Lerp(_childCubes[i].transform.localPosition,
                            new Vector3(0, temp, 0), Time.deltaTime * 3f);
                        temp++;
                    }
                }
            }
        }

        /// <summary>
        /// Description:
        /// Oyuncunun küp kaybettiğinde düşüşünü gerçekçi göstermek için y eksenindeki ivmesini manipüle eder.
        /// Inputs:
        /// none
        /// Returns: 
        /// void (no return)
        /// </summary>
        private void FallSetting()
        {
            if (_rb.velocity.y < 0)
            {
                _rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
        }
        
        /// <summary>
        /// Description:
        /// Küplerden gelen tetiklenmeleri yönlendirir.
        /// Inputs:
        /// Collider
        /// Returns: 
        /// void (no return)
        /// </summary>
        public void OnChildTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out Cubes cube))
            {
                if (cube.activeColor == GameManager.Instance.activeColor)
                {
                    OnAddingCube(other);
                }
                else if (cube.activeColor == ActiveColor.Player)
                {
                }
                else
                {
                    OnChildDestroy(_childCubes.Last());
                }
            }

            //Bitişe gelince kendini kapatır, 2 tane konfeti klonlar ve diğer sınıflara bildirir.
            if (other.CompareTag("Finish"))
            {
                Instance.enabled = false;
                Instantiate(confettiWin,
                    new Vector3(transform.position.x, transform.position.y + 11,
                        transform.position.z),
                    Quaternion.identity);
                Instantiate(confettiWin,
                    new Vector3(transform.position.x, transform.position.y + 11,
                        transform.position.z),
                    Quaternion.identity);
                _audioSource.PlayOneShot(winSound);
                UIManager.Instance.WinGame();
                ScoreManager.Instance.EndGame();
            }
            
            if (other.CompareTag("Barrier"))
            {
                ScoreManager.Instance.IncreaseMultiplier();
                OnChildDestroy(_childCubes.Last());
            }
        }
        /// <summary>
        /// Description:
        /// Yeni küp geldiğinde en alda koyar, diğerlerini sırasıyla yukarıya kaydırır.
        /// Inputs: 
        /// Collider
        /// Returns: 
        /// void (no return)
        /// </summary>
        private void OnAddingCube(Collider cube)
        {
            ScoreManager.Instance.IncreaseScore();
            cube.tag = "Player";
            cube.isTrigger = true;
            cube.transform.SetParent(transform);
            cube.gameObject.GetComponent<Cubes>().activeColor = ActiveColor.Player;
            _childCubes.Add(cube.gameObject);
            
            //Sırasıyla küpleri olması gerektiği yere getirir.
            int temp = 0;
            for (int i = _childCubes.Count - 1; i >= 0; i--)
            {
                _childCubes[i].transform.localPosition = new Vector3(0, temp, 0);
                temp++;
            }

            //Eklendi sesini oynatır.
            _audioSource.PlayOneShot(cubeAdd);
        }

        /// <summary>
        /// Description:
        /// Oyuncu çarpınca en alttaki küpü atar ve küp sayısına göre sınıflara haber verir.
        /// Inputs: 
        /// GameObject
        /// Returns: 
        /// void (no return)
        /// </summary>
        public void OnChildDestroy(GameObject child)
        {
            if (_childCubes.Count == 1 && GameManager.Instance.GetState() != GameManager.GameState.Win)
            {
                ScoreManager.Instance.DecreaseScore();
                GameManager.Instance.SetState(GameManager.GameState.Lose);
                _audioSource.PlayOneShot(loseSound);
                RemoveCube(child);
            }

            else
            {
                RemoveCube(child);
                _crash = true;
                if (GameManager.Instance.GetState() == GameManager.GameState.Win)
                {
                    //Küpler bitene kadar sağında ve solunda konfeti klonlar.
                    Instantiate(confetti,
                        new Vector3(child.transform.position.x - 2, child.transform.position.y,
                            child.transform.position.z),
                        Quaternion.identity);
                    Instantiate(confetti,
                        new Vector3(child.transform.position.x + 2, child.transform.position.y,
                            child.transform.position.z),
                        Quaternion.identity);
                    
                    if (_childCubes.Count < 1)
                    {
                        //Oyuncu kazandıysa 2 tane konfeti klonlar.
                        Instantiate(confettiWin,
                            new Vector3(child.transform.position.x, child.transform.position.y,
                                child.transform.position.z),
                            Quaternion.identity);

                        _audioSource.PlayOneShot(winSound);

                        //Oyunun bitişini diğer sınıflara bildirir.
                        UIManager.Instance.WinGame();
                        ScoreManager.Instance.EndGame();
                    }
                    else
                    {
                        _audioSource.PlayOneShot(cubeWinReduce);
                    }
                }
                else
                {
                    //Bitişte değilse skor düşür ve sesi oynat.
                    ScoreManager.Instance.DecreaseScore();
                    _audioSource.PlayOneShot(cubeReduce);
                }
            }
        }
        
        /// <summary>
        /// Description:
        /// Gelen küp objesini atar ve oyuncuyu zıplatır.
        /// Inputs: 
        /// GameObject
        /// Returns: 
        /// void (no return)
        /// </summary>
        private void RemoveCube(GameObject child)
        {
            child.tag = "Destroyed";
            child.transform.parent = null;
            child.GetComponent<Collider>().enabled = false;
            _childCubes.Remove(child);
            _rb.velocity += new Vector3(0.0f, jumpForce, 0.0f);
        }

        /// <summary>
        /// Description:
        /// Renk değiştiricilerini ve bitiş etkinliğini algılayıp diğer sınıflara bildirir.
        /// Inputs: 
        /// Collider
        /// Returns: 
        /// void (no return)
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            //Bitiş etkinliğine gelindiğinde fiziği kapatır ve oyunu Win durumuna getirir.
            if (other.CompareTag("Multiplier"))
            {
                GameManager.Instance.SetState(GameManager.GameState.Win);
                other.enabled = false;
                _rb.isKinematic = true;
            }

            // Değiştiriciye geldiyse küplerin rengini değiştirir, sesi oynatır ve GameManager'ın
            // aktif rengini değiştirir.
            if (other.gameObject.TryGetComponent(out Changers changers))
            {
                foreach (var cube in _childCubes)
                {
                    cube.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial =
                        colors[(int)changers.activeColor];
                }

                other.gameObject.GetComponent<Collider>().enabled = false;
                _audioSource.PlayOneShot(cubeWinReduce);
                GameManager.Instance.activeColor = changers.activeColor;
            }
        }

        /// <summary>
        /// Description:
        /// Küp sayısı 0'dan büyük olduğu sürece en alttaki küpün transformunu,
        /// eşit veya küçükse oyuncunun transformunu döndürür.
        /// Inputs: 
        /// none
        /// Returns: 
        /// Transform
        /// </summary>
        /// <returns>Transform: Kameranın takip etmesi için transform</returns>
        public Transform LastCube()
        {
            if (_childCubes.Count > 0)
                return _childCubes.Last().transform;
            else
            {
                return transform;
            }
        }
    }
}