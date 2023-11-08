using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

namespace MainCharacter
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
    public partial class Player : Entity
    {

        [Space]
        [Header("Movement Settings")]

        [SerializeField, Range(0, 1500), Tooltip("Maximum Speed of the Player")] float _Speed = 10f;

        [Tooltip("Rigidbody of the Player")] public Rigidbody2D _Rigidbody;

        [SerializeField, Tooltip("Rigidbody of the Player")] SpriteRenderer _PlayerRenderer;

        [SerializeField, Tooltip("Grounded Function's Layer Mask")] LayerMask _GroundMask;

        [SerializeField, Tooltip("Grounded Function's Box' Bounds")] BoxCollider2D _PlayerCollider;

        private Vector3 m_Velocity = Vector3.zero;

        void Movement()
        {
            float x = Input.GetAxisRaw("Horizontal") * Time.fixedDeltaTime * _Speed;
            float y = Input.GetAxisRaw("Vertical") * Time.fixedDeltaTime * _Speed;

            bool grounded = isGrounded();

            if (Input.GetKeyDown(_KeybindTable.JumpKey) && LevelManager.Instance.GravityScale != 0 && grounded)
            {
                Jump();
            }

            Vector2 targetVelocity = new Vector2(x * 10f, (LevelManager.Instance.GravityScale == 0) ? y * 10 : _Rigidbody.velocity.y);
            if (!grounded && LevelManager.Instance.GravityScale != 0) targetVelocity.x *= 0.75f;

            Move(targetVelocity);
        }


        public override void Move(Vector2 pos)
        {
            _Rigidbody.velocity = Vector3.SmoothDamp(_Rigidbody.velocity, pos, ref m_Velocity, .05f);
            _Rigidbody.velocity = Vector2.ClampMagnitude(_Rigidbody.velocity, 25f);
            if (pos.x != 0)
            {
                Vector3 euler = transform.localEulerAngles;
                euler.y = (pos.x < 0) ? 180 : 0;
                transform.localEulerAngles = euler;
            }

        }

        void Jump()
        {
            _Rigidbody.AddForce(new Vector2(0, 1000), ForceMode2D.Impulse);
        }


        bool isGrounded() => Physics2D.BoxCast(_PlayerCollider.bounds.center, _PlayerCollider.bounds.size, 0, Vector2.down, 0.1f, _GroundMask);


    }

}