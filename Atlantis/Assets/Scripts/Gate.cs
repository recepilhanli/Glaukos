using System.Collections;
using System.Collections.Generic;
using MainCharacter;
using UnityEngine;

/// <summary>
/// gate class for opening and closing the gate
/// </summary>
public class Gate : MonoBehaviour
{
    private Vector3 _CurrentPos;

    [SerializeField] Vector3 _OpenedPos = Vector3.zero;

    [SerializeField] Vector3 _ClosedPos = Vector3.zero;
    [SerializeField] float _SmoothTime = .25f;

    private Vector3 m_Velocity = Vector3.zero;
    public void OpenTheGate()
    {
        _CurrentPos = _OpenedPos;
    }

    public void CloseTheGate()
    {
        _CurrentPos = _ClosedPos;
    }


    void Start()
    {
        _CurrentPos = transform.position;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, _CurrentPos) < 0.1f)
        {
            m_Velocity = Vector3.zero;
            return;
        }
        if (m_Velocity != Vector3.zero) Player.Instance.CameraShake(1f, .6f,.2f);
        transform.position = Vector3.SmoothDamp(transform.position, _CurrentPos, ref m_Velocity, _SmoothTime);
    }
}
