﻿using System;
using Unity.VisualScripting;
using UnityEngine;

namespace _Project.RGScripts.Player
{
    [SelectionBase]
    [RequireComponent(typeof(PlayerStateMachine), typeof(PlayerAnimation), typeof(PlayerInput))]
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
    public class Player : MonoBehaviour
    {
        [SerializeField] public PlayerConfig _playerConfig;

        // Script
        private PlayerStateMachine _playerStateMachine;
        private PlayerAnimation _playerAnimation;
        private PlayerInput _playerInput;

        // Componenet
        private Rigidbody2D _rigidbody2D;
        private Collider2D _collider2D;
        private SpriteRenderer _spriteRenderer;
        [SerializeField] private BoxCollider2D _groundCollider2D;
        
        public bool IsFacingRight;
        public bool IsGrounded;
        public Vector2 MoveVelocity { get; private set; }
        public bool HasDoubleJumped;

        private void Awake()
        {
            _playerStateMachine = GetComponent<PlayerStateMachine>();
            _playerAnimation = GetComponent<PlayerAnimation>();
            _playerInput = GetComponent<PlayerInput>();

            _rigidbody2D = GetComponent<Rigidbody2D>();
            _collider2D = GetComponent<Collider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            GroundCheck();
        }

        public void HandleHorizontalMovement(float acceleration, float deceleration, Vector2 moveInput)
        {
            if (moveInput != Vector2.zero)
            {
                TurnCheck(moveInput);
                MoveVelocity = Vector2.Lerp(MoveVelocity, new Vector2(moveInput.x, 0) * _playerConfig.RunSpeed, acceleration * Time.deltaTime);
            }
            else
            {
                MoveVelocity = Vector2.Lerp(MoveVelocity, Vector2.zero, deceleration * Time.deltaTime);
            }
            
            SetVelocity(new Vector2(MoveVelocity.x, Velocity.y));
        }
        
        private void GroundCheck()
        {
            Vector2 boxCastOrigin = new Vector2(GroundCollider.bounds.center.x, GroundCollider.bounds.min.y);
            Vector2 boxCastSize = new Vector2(GroundCollider.bounds.size.x, _playerConfig.RayDetectionLenght);

            var groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, _playerConfig.RayDetectionLenght,
                _playerConfig.WhatIsGround);

            if (groundHit.collider != null)
            {
                // groundOffTime = 0;
                IsGrounded = true;
            }
            else
            {
                // groundOffTime += Time.deltaTime;
                IsGrounded = false;
            }
            
            Debug.DrawRay(boxCastOrigin, Vector2.down * _playerConfig.RayDetectionLenght);
        }
        
        private void TurnCheck(Vector2 moveInput)
        {
            if (moveInput.x > 0)
                FlipPlayer(true);
            else if (moveInput.x < 0)
                FlipPlayer(false);
        }
        
        private void FlipPlayer(bool isFacingRight)
        {
            _spriteRenderer.flipX = !isFacingRight;
            this.IsFacingRight = isFacingRight;
        }

        public Vector3 Position => transform.position;
        public Vector2 Velocity => _rigidbody2D.velocity;

        public BoxCollider2D GroundCollider => _groundCollider2D;

        public void SetPosition(float x, float y)
        {
            transform.position = new Vector3(x, y);
        }
        public void SetPosition(Vector2 vector2)
        {
            SetPosition(vector2.x, vector2.y);
        }
        public void SetVelocity(float x, float y)
        {
            _rigidbody2D.velocity = new Vector2(x, y);
        }
        public void SetVelocity(Vector2 vector2)
        {
            SetVelocity(vector2.x, vector2.y);
        }
        public void SetGravity(float gravityScale)
        {
            _rigidbody2D.gravityScale = gravityScale;
        }
        public void SetAnimation(PlayerStateType stateType)
        {
            _playerAnimation.SetAnimationSpeed(1);
            _playerAnimation.PlayAnimation(stateType);
        }
        public void SetAnimationSpeed(float speed)
        {
            _playerAnimation.SetAnimationSpeed(speed);
        }
        
    }
}