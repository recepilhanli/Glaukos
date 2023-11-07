using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
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

        [SerializeField, Tooltip("Layer Mask")] LayerMask groundMask;

        [SerializeField, Tooltip("For Is Grounded Function")] Vector2 groundBox = new Vector2(0.5f, 0.2f);

        void Movement()
        {
            float x = Input.GetAxisRaw("Horizontal") * Time.fixedDeltaTime * _Speed;
            float y = Input.GetAxisRaw("Vertical") * Time.fixedDeltaTime * _Speed;

            if (Input.GetKeyDown(_KeybindTable.JumpKey) && isGrounded() && LevelManager.Instance.GravityScale != 0)
            {
                Jump();
            }

            Vector3 targetVelocity = new Vector2(x * 10f,(LevelManager.Instance.GravityScale == 0) ? y * 10: _Rigidbody.velocity.y);
            _Rigidbody.velocity = Vector3.SmoothDamp(_Rigidbody.velocity, targetVelocity, ref m_Velocity, .05f);
            _Rigidbody.velocity = Vector3.ClampMagnitude(_Rigidbody.velocity, 25f);

        }

        void Jump()
        {
            _Rigidbody.AddForce(new Vector2(0, 120), ForceMode2D.Impulse);
        }


        bool isGrounded() => Physics2D.BoxCast(transform.position, groundBox, 0, -transform.up, 0.5f, groundMask);


    }

}