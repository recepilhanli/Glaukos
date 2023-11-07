using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{

    /// <summary>
    /// 
    /// 
    /// Player.Movement.cs:
    /// Functions:
    /// Move();
    /// Gravity();
    /// 
    /// 
    /// </summary>

    [RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(BoxCollider2D))]
    public partial class Player : MonoBehaviour
    {

        [Space]
        [Header("Movement Settings")]

        [SerializeField, Range(0, 1500), Tooltip("Maximum Speed of the Player")] float _Speed = 10f;

        [SerializeField, Tooltip("Rigidbody of the Player")] Rigidbody2D _Rigidbody;

        private Vector3 m_Velocity = Vector3.zero;


        void Move()
        {
            float x = Input.GetAxis("Horizontal") * Time.fixedDeltaTime * _Speed;
            float y = Input.GetAxis("Vertical") * Time.fixedDeltaTime * _Speed;


            Vector3 targetVelocity = new Vector2(x * 10f, _Rigidbody.velocity.y + y * 10);
            _Rigidbody.velocity = Vector3.SmoothDamp(_Rigidbody.velocity, targetVelocity, ref m_Velocity, .05f);

        }

    }

}