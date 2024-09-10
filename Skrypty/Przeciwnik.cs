using Godot;
using System;

public partial class Przeciwnik : CharacterBody3D
{
	float speed = 2.0f;
	float accel = 2.0f;
	bool CzyJestemWidziany = false;
	private int Licznik = 0;
	NavigationAgent3D agent;

	// Parametry oscylacji góra-dół
	float oscillation_amplitude = 0.006f; // wysokość oscylacji
	float oscillation_speed = 3.0f; // szybkość oscylacji
	float oscillation_time = 0.01f; // czas oscylacji

	public override void _Ready()
	{
		GD.Print("GOTOWY");
		agent = GetNode<NavigationAgent3D>("NavigationAgent3D");
	}
	public void SchowajSie()
{
	Random random = new Random();
	int kierunek = random.Next(0, 2); // 0 - lewo, 1 - prawo

	// Wektor kierunku ruchu względem obecnej pozycji przeciwnika
	Vector3 move_direction = new Vector3();

	// Określenie kierunku (lewo lub prawo) na podstawie wektora obrotu względem gracza
	if (kierunek == 0)
	{
		// Ruch w lewo
		move_direction = new Vector3(-GlobalTransform.Basis.Z.Z, 0, GlobalTransform.Basis.Z.X) * 5.0f; // Skala 5 dla odległości
		GD.Print("Przeciwnik rusza się w lewo.");
	}
	else
	{
		// Ruch w prawo
		move_direction = new Vector3(GlobalTransform.Basis.Z.Z, 0, -GlobalTransform.Basis.Z.X) * 5.0f;
		GD.Print("Przeciwnik rusza się w prawo.");
	}

	// Ustawienie nowego celu nawigacyjnego w odpowiednim kierunku
	agent.TargetPosition = GlobalPosition + move_direction;
}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Oscylacja góra-dół
		oscillation_time += (float)delta * oscillation_speed;
		float vertical_offset = Mathf.Sin(oscillation_time) * oscillation_amplitude; // Oblicza pozycję Y

		if (CzyJestemWidziany)
		{
			speed = 0.5f;
			GD.Print("AAAA ZAUWAZONO MNIE" + delta);
			Licznik++;
			if (Licznik > 5)
			{
				Licznik = 0;
				SchowajSie();
				CzyJestemWidziany = false;
			}
		}
		else
		{
			speed = 3.0f;
		

		Vector3 dir = new Vector3();
		Gracz gracz = GetNode<Gracz>("../Gracz");

		agent.TargetPosition = gracz.GlobalPosition;
		dir = agent.GetNextPathPosition() - GlobalPosition;
		dir = dir.Normalized();
		Velocity = Velocity.Lerp(dir * speed, accel * (float)delta);

		// Dodanie oscylacji do pozycji Y
		Vector3 new_position = GlobalPosition;
		new_position[1] += vertical_offset;
		GlobalPosition = new_position; // Ustaw nową pozycję z uwzględnieniem ruchu góra-dół

		// Obrót przeciwnika bokiem o 90 stopni do gracza
		Vector3 look_direction = (gracz.GlobalPosition - GlobalPosition).Normalized();
		look_direction[1] = 0; // Ignoruj obrót w osi Y, aby tylko obracać się w poziomie

		// Obrót o 90 stopni w lewo względem gracza
		Vector3 side_direction = new Vector3(-look_direction[2], 0, look_direction[0]); // Obrót o 90 stopni w osi Y

		// Obrót przeciwnika bokiem względem gracza
		LookAt(GlobalPosition + side_direction, Vector3.Up);
		}
		MoveAndSlide();
	}

	public void zauwazony()
	{
		CzyJestemWidziany = true;
	}
}
