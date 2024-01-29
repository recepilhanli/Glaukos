using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MainCharacter
{
    /// <summary>
    /// This class is used to manage the camera
    /// </summary>
    public partial class Player : Entity
    {
        Cinemachine.CinemachineVirtualCamera _VirtualCamera;


        Cinemachine.CinemachineBasicMultiChannelPerlin _Perlin;

        Coroutine ShakeCoroutine;

        float _LensSize = 8;
        bool _WaitForCameraShake = false;

        public bool LockLensSize = false;

        [SerializeField] Volume _PostProcess;

        private ChromaticAberration _ChromaticAberration;


        private ColorAdjustments _ColorAdjustments;

        private FilmGrain _FilmGrain;

        void InitCamera()
        {
            _VirtualCamera = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
            _Perlin = _VirtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
            _PostProcess.profile.TryGet(out _ChromaticAberration);
            _PostProcess.profile.TryGet(out _ColorAdjustments);
            _PostProcess.profile.TryGet(out _FilmGrain);


        }


        public void SetLensSize(float _size, bool force = false)
        {
            _LensSize = _size;
            if (force) _VirtualCamera.m_Lens.OrthographicSize = _size;
        }

        void CameraSize()
        {
            if (LockLensSize) _LensSize = 10f;
            else if (!LockLensSize)
            {
                if (Input.GetKey(_KeybindTable.ThrowKey) && !isDeath && !_Rage) _LensSize = 9;
                else if (!_Rage && !isDeath) _LensSize = 8;
            }
            _VirtualCamera.m_Lens.OrthographicSize = Mathf.MoveTowards(_VirtualCamera.m_Lens.OrthographicSize, _LensSize, Time.deltaTime * 10);
        }

        void CameraUpdate()
        {
            if (TutorialDialogHandler.TutBlockThrow == false) CameraSize();
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