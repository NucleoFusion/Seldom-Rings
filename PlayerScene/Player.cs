using Godot;
using System;
using SeldomRings.PlayerScene;

enum PlayerAction
{
	IdleOrWalking,
	Dashing,
	Blocking,
	Attacking
}

public partial class Player : CharacterBody2D
{
	// Action Exports
	[Export] public float Speed = 200f;
	[Export] public float AttackCooldown = 0.5f;
	[Export] public float DashCooldown = 0.1f;
	[Export] public float DashSpeed = 1000f;
	[Export] public float AttackPower = 50f;
	
	//Inventory
	private Inventory _inventory;
	
	private PlayerAction _playerAction = PlayerAction.IdleOrWalking;
	private AnimatedSprite2D _animatedSprite2D;
	
	// Attack Handlers
	private bool _isAttacking;
	private float _attackTimer;
	
	//Dash Handlers
	private bool _isDashing;
	private float _dashTimer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		
		if(_playerAction == PlayerAction.IdleOrWalking || _playerAction == PlayerAction.Blocking) _playerAction = GetPlayerAction();
		
		switch (_playerAction)
		{
			case PlayerAction.Dashing:
				HandleDash(delta);
				break;
			case PlayerAction.Attacking:
				HandleAttack(delta);
				HandleMovement();
				break;
			case PlayerAction.Blocking:
				HandleBlock();
				HandleMovement();
				break;
			case PlayerAction.IdleOrWalking:
				HandleMovement();
				break;
		}

		HandleAnimations();
		MoveAndSlide();
	}

	private PlayerAction GetPlayerAction()
	{
		if (Input.IsActionJustPressed("dash")) return PlayerAction.Dashing;
		if (Input.IsActionPressed("attack")) return PlayerAction.Attacking;
		if (Input.IsActionPressed("block")) return PlayerAction.Blocking;

		return PlayerAction.IdleOrWalking;
	}
	
	private void HandleBlock()
	{
		// If we add stuff in future, redundant for now	
	}
	
	private void HandleDash(double delta)
	{
		Vector2 dashDirection = Velocity;

		if (_playerAction == PlayerAction.Dashing && !_isDashing )
		{
			_isDashing = true;
			_dashTimer = DashCooldown;

			if (dashDirection.IsEqualApprox(Vector2.Zero)) dashDirection = Vector2.Right;
			dashDirection = dashDirection.Normalized();
			
			Velocity = dashDirection * DashSpeed;
		}
		
		if (_isDashing)
		{
			
			_dashTimer -= (float)delta;
			Velocity = dashDirection.Normalized() * DashSpeed  ;
			
			if (_dashTimer <= 0)
			{
				_isDashing = false;
				_playerAction = PlayerAction.IdleOrWalking;
			}
		}
	}
	
	private void HandleMovement()
	{
		Vector2 direction = Vector2.Zero;
			
		if (Input.IsActionPressed("move_right")) direction.X += 1;
		if (Input.IsActionPressed("move_left")) direction.X -= 1;
		if (Input.IsActionPressed("move_down")) direction.Y += 1;
		if (Input.IsActionPressed("move_up")) direction.Y -= 1;

		if (direction.Length() > 1) direction = direction.Normalized();

		Velocity = direction * Speed;
	}

	private void HandleAttack(double delta)
	{
		if (_isAttacking)
		{
			_attackTimer -= (float)delta;
			if (_attackTimer <= 0)
			{
				_isAttacking = false;
				_playerAction = PlayerAction.IdleOrWalking;
			}
		}

		if (_playerAction == PlayerAction.Attacking && !_isAttacking)
		{
			_isAttacking = true;
			_attackTimer = AttackCooldown;
			_animatedSprite2D.Play("attack");
		}
	}

	private void HandleAnimations()
	{
		switch (_playerAction)
		{
			case PlayerAction.Blocking:
				_animatedSprite2D.Play("block");
				break;
			case PlayerAction.IdleOrWalking:
				if (Velocity.Length() > 1)
				{
					_animatedSprite2D.Play("walk");
					
					GD.Print(Velocity);
					if (Velocity.X < 0) _animatedSprite2D.FlipH = true;
					else _animatedSprite2D.FlipH = false; // Check if left facing
				}
				else _animatedSprite2D.Play("idle");
				break;	
		}
	}

	public bool IsInvincible()
	{
		return _playerAction == PlayerAction.Blocking || _playerAction == PlayerAction.Dashing;
	}
}
