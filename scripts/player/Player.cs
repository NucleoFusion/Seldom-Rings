using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed = 200f;
	[Export] public float AttackCooldown = 0.5f;
	[Export] public float DashCooldown = 0.1f;
	[Export] public float DashSpeed = 1000f;
	[Export] public float AttackPower = 50f;
	[Export] public float Health = 250f;
	[Export] public float MaxHeals = 3;
	[Export] public float HealsLeft = 6;
	
	private AnimatedSprite2D _animatedSprite2D;
	
	// Attack Handlers
	private bool _isAttacking;
	private float _attackTimer;
	
	//Dash Handlers
	private bool _isDashing;
	private float _dashTimer;
	
	//Block Handlers
	private bool _isBlocking;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		HandleDash(delta);
		if (_isDashing)
		{
			MoveAndSlide();
			return;
		}
		
		HandleBlock();
		
		HandleAttack(delta);
		HandleMovement();
		if (!_isAttacking) HandleAnimations();
		
		MoveAndSlide();
	}

	private void HandleBlock()
	{
		if (Input.IsActionPressed("block") && !_isAttacking)
		{
			GD.Print("Blocking");
			//TODO: Block Animation
			_isBlocking = true;
		}
		else
		{
			_isBlocking = false;
		}
	}
	
	private void HandleDash(double delta)
	{
			Vector2 dashDirection = Velocity;
	
			if (Input.IsActionJustPressed("dash") && !_isDashing && !_isAttacking && _dashTimer <= 0)
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
				//TODO: Animation for dash
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
				_animatedSprite2D.Play("idle");
			}
		}

		if (Input.IsActionJustPressed("attack") && !_isAttacking)
		{
			_isAttacking = true;
			_attackTimer = AttackCooldown;
			_animatedSprite2D.Play("attack");
		}
	}

	private void HandleAnimations()
	{
		if (Velocity.Length() > 0)
		{
			_animatedSprite2D.Play("walk");
			_animatedSprite2D.FlipH = Velocity.X < 0;
		}
		else
		{
			_animatedSprite2D.Play("idle");
		}
	}

	public bool IsInvincible()
	{
		return _isBlocking || _isDashing;
	}
}
