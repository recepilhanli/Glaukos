using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

namespace MainCharacter
{

    public partial class Player : Entity
    {
        Cinemachine.CinemachineVirtualCamera _VirtualCamera;


        Cinemachine.CinemachineBasicMultiChannelPerlin _Perlin;

        float _LensSize = 8;
        void InitCamera()
        {
            _VirtualCamera = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
            _Perlin = _VirtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();

        }

        void CameraSize()
        {
            _VirtualCamera.m_Lens.OrthographicSize = Mathf.MoveTowards(_VirtualCamera.m_Lens.OrthographicSize, _LensSize, Time.deltaTime * 10);
            if (Input.GetKey(_KeybindTable.HeavyAttack) && !isDeath && !_Rage) _LensSize = 9;
            else if (!_Rage && !isDeath) _LensSize = 7;
        }

        void CameraUpdate()
        {
            CameraSize();
        }

    }

}