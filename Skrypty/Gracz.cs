using Godot;
using System;

public partial class Gracz : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	// Czułość myszy
	private const float mouseSens = 0.1f;

	// Referencja do głowy postaci
	private Node3D head;
	private RayCast3D ray;
	private Area3D detectionArea;

   public override void _Ready()
	{
	 head = GetNode<Node3D>("Head2");
	 ray = GetNode<RayCast3D>("RayCast3D");
	 detectionArea = GetNode<Area3D>("Area3D");
		// Ustawienie trybu przechwytywania myszy
		Input.MouseMode = Input.MouseModeEnum.Captured;
		
		detectionArea.BodyEntered += OnBodyEntered;

	}

	public override void _Input(InputEvent @event)
	{
	 	if (@event is InputEventMouseMotion eventMouseMotion)
		{
			RotateY(-eventMouseMotion.Relative.X * Mathf.Pi * mouseSens / 180.0f);
			head.RotateX(-eventMouseMotion.Relative.Y * Mathf.Pi * mouseSens / 180.0f);

			// Ograniczenie rotacji głowy wzdłuż osi X
			Vector3 headRotation = head.Rotation;
			headRotation.X = Mathf.Clamp(headRotation.X, Mathf.DegToRad(-89), Mathf.DegToRad(89));
			head.Rotation = headRotation;

		}

	}
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
		widzenie();
		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("Lewo", "Prawo", "Przód", "Tył");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
			MoveAndSlide();
			

	}
	//Sprawdza czy entity jest widziane
	private void widzenie(){
		if(ray.IsColliding() && ray.GetCollider().HasMethod("zauwazony")){
			GD.Print("cos widze");
			Przeciwnik a = (Przeciwnik) ray.GetCollider();
			a.zauwazony();
		}
	}
		private void OnBodyEntered(Node body)
	{
					GD.Print("Koliduje z "+body);
		if (body is Przeciwnik)
		{
			GD.Print("Koniec gry!");
			GetTree().Quit(); // Zamyka grę
		}
	}
}
