using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Entities;
using UnityEngine;
using Utility;

/// <summary>
/// Bu sınıf kameranın takip edeceği objeyi belirliyor ve damping ayarını yapıyor.
/// </summary>
[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraConroller : MonoBehaviour
{
    private CinemachineVirtualCamera camera;
    private CinemachineTransposer _transposer;
    [SerializeField] private Vector3 offset;

    private void Start()
    {
        camera = GetComponent<CinemachineVirtualCamera>();
        _transposer = camera.GetCinemachineComponent<CinemachineTransposer>();
    }

    private void Update()
    {
        //Takip edilecek küpü seçiyor.
        if (GameManager.Instance.GetState() == GameManager.GameState.Win & PlayerController.Instance.CubeCount() > 0)
        {
            camera.LookAt = PlayerController.Instance.LastCube();
            camera.Follow = PlayerController.Instance.LastCube();
        }

        //Küplerin sayısına göre Z-axis Damping değerini değiştiriyor.
        _transposer.m_ZDamping = PlayerController.Instance.CubeCount();
    }
}