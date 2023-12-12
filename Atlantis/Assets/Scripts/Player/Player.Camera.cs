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

        Coroutine ShakeCoroutine;

        float _LensSize = 8;
        bool _WaitForCameraShake = false;

        public bool LockLensSize = false;

        void InitCamera()
        {
            _VirtualCamera = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
            _Perlin = _VirtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();

        }

        void CameraSize()
        {
            if (LockLensSize) _LensSize = 10f;
            else if (!LockLensSize)
            {
                if (Input.GetKey(_KeybindTable.HeavyAttack) && !isDeath && !_Rage) _LensSize = 9;
                else if (!_Rage && !isDeath) _LensSize = 8;
            }
            _VirtualCamera.m_Lens.OrthographicSize = Mathf.MoveTowards(_VirtualCamera.m_Lens.OrthographicSize, _LensSize, Time.deltaTime * 10);
        }

        void CameraUpdate()
        {
            CameraSize();
        }


        public void CameraShake(float _a, float _f, float _t = 1f, bool _wait = false)
        {
            if (ShakeCoroutine != null && _WaitForCameraShake == true) return;

            if (ShakeCoroutine != null) StopCoroutine(ShakeCoroutine);
            _Perlin.m_AmplitudeGain = _a;
            _Perlin.m_FrequencyGain = _f;
            _WaitForCameraShake = _wait;
            ShakeCoroutine = StartCoroutine(Shake(_t));
        }

        IEnumerator Shake(float _t = 1f)
        {
            yield return new WaitForSeconds(_t);
            _Perlin.m_AmplitudeGain = 0.75f;
            _Perlin.m_FrequencyGain = 0.05f;
            _WaitForCameraShake = false;
            ShakeCoroutine = null;
            yield return null;
        }

    }

}