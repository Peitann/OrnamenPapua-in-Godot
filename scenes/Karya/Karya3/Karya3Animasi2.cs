using Godot;
using System;
using System.Collections.Generic;

public class Karya3Animasi2 : Node2D
{
	private BentukDasar _bentukDasar = new BentukDasar();
	private float _animationTime = 0f;
	private float _animationSpeed = 0.5f;
	private float _amplitude = 100f;  // Size of the animation path
	
	// Original positions for each cultural object
	private Vector2 _rumahHonaiPos;
	private Vector2 _sateUlatSaguPos;
	private Vector2 _tasNokenPos;
	private Vector2 _screenCenter;

	// Color scheme for consistency
	private Color _rumahDindingColor = new Color(0.5f, 0.25f, 0);
	private Color _rumahAtapColor = new Color(1.0f, 0.89f, 0.77f);
	private Color _rumahZigzagColor = new Color(0.6f, 0.3f, 0.1f);
	private Color _rumahPagarColor = Colors.Gray;
	private Color _rumahPintuColor = Colors.Black;
	
	private Color _sateTusukColor = new Color(0.36f, 0.25f, 0.2f);
	private Color _sateUlatColor = new Color(1.0f, 0.9f, 0.8f);
	private Color _sateDetailColor = new Color(0.3f, 0.15f, 0.05f);
	
	private Color _tasBadanColor = new Color(0.6f, 0.3f, 0.1f);
	private Color _tasTaliColor = new Color(0.4f, 0.2f, 0.1f);
	private Color _tasMotifColor = Colors.DarkRed;
	private Color _tasOrnamenColor = new Color(1f, 0.7f, 0.2f);

	public override void _Ready()
	{
		 // Connect back button
		if (HasNode("BackButton7"))
		{
			GetNode<Button>("BackButton7").Connect("pressed", this, nameof(OnButtonBackPressed));
			UpdateButtonLabel("BackButton7", "Kembali");
		}
		
		// Connect animation buttons
		if (HasNode("Animasi1"))
		{
			GetNode<Button>("Animasi1").Connect("pressed", this, nameof(OnAnimasi1Pressed));
			UpdateButtonLabel("Animasi1", "Animasi 1");
		}
		
		if (HasNode("Animasi2"))
		{
			GetNode<Button>("Animasi2").Connect("pressed", this, nameof(OnAnimasi2Pressed));
			UpdateButtonLabel("Animasi2", "Animasi 2");
		}
		
		// Get screen dimensions
		Vector2 windowSize = GetViewportRect().Size;
		_screenCenter = new Vector2(windowSize.x / 2, windowSize.y / 2);
		
		// Initialize object positions
		InitializeObjects();
		
		// Add instructions label
		AddInstructionsLabel();
	}

	private void AddInstructionsLabel()
	{
		Label instructionsLabel = new Label();
		instructionsLabel.Text = "Use buttons below to switch animation modes";
		instructionsLabel.RectPosition = new Vector2(10, 10);
		instructionsLabel.AddColorOverride("font_color", Colors.White);
		AddChild(instructionsLabel);
	}

	private void UpdateButtonLabel(string buttonPath, string newText)
	{
		var button = GetNode<Button>(buttonPath);
		if (button != null && button.HasNode("Label"))
		{
			var label = button.GetNode<Label>("Label");
			label.Text = newText;
		}
	}

	private void OnButtonBackPressed()
	{
		var menuScene = (PackedScene)GD.Load("res://scenes/Menu/Menu.tscn");
		GetTree().ChangeSceneTo(menuScene);
	}

	// Button click handlers
	private void OnAnimasi1Pressed()
	{
		// Switch to Animasi1/Karya3
		var storyboardScene = (PackedScene)GD.Load("res://scenes/Karya/Karya3/Karya3.tscn");
		GetTree().ChangeSceneTo(storyboardScene);
	}
	
	private void OnAnimasi2Pressed()
	{
		// Stay on current animation (Animasi2/Karya3Animasi2)
		// Reset the animation
		ResetAnimation();
	}

	private void InitializeObjects()
	{
		// Set positions similar to Karya2
		_rumahHonaiPos = new Vector2(_screenCenter.x - 200, _screenCenter.y - 100);
		_sateUlatSaguPos = new Vector2(_screenCenter.x, _screenCenter.y);
		_tasNokenPos = new Vector2(_screenCenter.x + 200, _screenCenter.y + 100);
	}

	
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			if (keyEvent.Scancode == (uint)KeyList.Key1)
			{
				// Switch to storyboard animation
				SwitchToStoryboardMode();
			}
			else if (keyEvent.Scancode == (uint)KeyList.Key2)
			{
				// Stay on current path animation scene - no action needed
			}
		}
	}
	
	private void SwitchToStoryboardMode()
	{
		// Switch back to the main Karya3 scene
		var storyboardScene = (PackedScene)GD.Load("res://scenes/Karya/Karya3/Karya3.tscn");
		GetTree().ChangeSceneTo(storyboardScene);
	}

	public void ResetAnimation()
	{
		_animationTime = 0f;
	}

	public override void _Process(float delta)
	{
		_animationTime += delta * _animationSpeed;
		Update(); // Request redraw
	}

	public override void _Draw()
	{
		// Calculate positions based on animation time
		float normalizedTime = (_animationTime % 2.0f) / 2.0f; // Value between 0 and 1
		
		// Draw paths
		DrawAnimationPaths();
		
		// 1. Draw Rumah Honai with triangle path animation
		Vector2 rumahHonaiOffset = GetTrianglePath(normalizedTime);
		Vector2 currentRumahHonaiPos = _rumahHonaiPos + rumahHonaiOffset;
		
		// Use RumahHonai class instead of direct drawing
		RumahHonai rumahHonai = new RumahHonai(this, currentRumahHonaiPos);
		rumahHonai.SetColors(_rumahDindingColor, _rumahAtapColor, _rumahZigzagColor, _rumahPagarColor, _rumahPintuColor);
		rumahHonai.GambarBerwarna();
		
		// 2. Draw Sate Ulat Sagu with circle path
		Vector2 sateUlatSaguOffset = GetCirclePath(normalizedTime);
		Vector2 currentSateUlatSaguPos = _sateUlatSaguPos + sateUlatSaguOffset;
		
		// Use SateUlatSagu class instead of direct drawing
		SateUlatSagu sate = new SateUlatSagu(this, currentSateUlatSaguPos, 5);
		sate.SetColors(_sateTusukColor, _sateUlatColor, _sateDetailColor);
		sate.GambarBerwarna();
		
		// 3. Draw Tas Noken with trapezium path
		Vector2 tasNokenOffset = GetTrapeziumPath(normalizedTime);
		Vector2 currentTasNokenPos = _tasNokenPos + tasNokenOffset;
		
		// Use TasNoken class instead of direct drawing
		TasNoken tas = new TasNoken(this, currentTasNokenPos, 0.8f);
		tas.SetColors(_tasBadanColor, _tasTaliColor, _tasMotifColor, _tasOrnamenColor);
		tas.GambarBerwarna();
	}
	
	private void DrawAnimationPaths()
	{
		// Draw Triangle Path for Rumah Honai
		List<Vector2> trianglePath = new List<Vector2>();
		for (int i = 0; i < 100; i++) // Changed from <= 100 to < 100 to avoid t = 1.0
		{
			float t = i / 99.0f; // Changed from 100.0f to 99.0f to avoid t = 1.0
			trianglePath.Add(_rumahHonaiPos + GetTrianglePath(t));
		}
		// Close the path by adding the first point again
		trianglePath.Add(trianglePath[0]);
		DrawPathLine(trianglePath, Colors.Yellow);
		
		// Draw Circle Path for Sate Ulat Sagu
		List<Vector2> circlePath = new List<Vector2>();
		for (int i = 0; i < 100; i++) // Changed from <= 100 to < 100
		{
			float t = i / 99.0f; // Changed from 100.0f to 99.0f
			circlePath.Add(_sateUlatSaguPos + GetCirclePath(t));
		}
		DrawPathLine(circlePath, Colors.Cyan);
		
		// Draw Trapezium Path for Tas Noken
		List<Vector2> trapeziumPath = new List<Vector2>();
		for (int i = 0; i < 100; i++) // Changed from <= 100 to < 100
		{
			float t = i / 99.0f; // Changed from 100.0f to 99.0f
			trapeziumPath.Add(_tasNokenPos + GetTrapeziumPath(t));
		}
		// Close the path by adding the first point again
		trapeziumPath.Add(trapeziumPath[0]);
		DrawPathLine(trapeziumPath, Colors.Magenta);
	}
	
	private void DrawPathLine(List<Vector2> path, Color color)
	{
		for (int i = 0; i < path.Count - 1; i++)
		{
			DrawLine(path[i], path[i + 1], color, 1);
		}
	}
	
	private Vector2 GetTrianglePath(float t)
	{
		// Fix: Ensure t is between 0 and just under 1 to prevent array out of bounds
		t = Mathf.Clamp(t, 0f, 0.999f);
		
		// Triangle path - 3 segments
		int segment = (int)(t * 3); // Which segment (0, 1, or 2)
		
		// Fix: Add explicit bounds check to prevent array index out of bounds
		segment = Mathf.Clamp(segment, 0, 2);
		
		float segmentT = (t * 3) % 1.0f; // Position within segment (0 to 1)
		
		// Define triangle vertices
		Vector2[] vertices = new Vector2[]
		{
			new Vector2(-_amplitude, -_amplitude/2), // Top left
			new Vector2(_amplitude, -_amplitude/2), // Top right
			new Vector2(0, _amplitude/2) // Bottom
		};
		
		// Interpolate between vertices based on segment
		Vector2 start = vertices[segment];
		Vector2 end = vertices[(segment + 1) % 3];
		
		return start.LinearInterpolate(end, segmentT);
	}
	
	private Vector2 GetCirclePath(float t)
	{
		// Circle/O path using polar coordinates
		float angle = t * Mathf.Pi * 2 - Mathf.Pi/2; // Start from top
		return new Vector2(
			_amplitude * Mathf.Cos(angle),
			_amplitude * Mathf.Sin(angle)
		);
	}
	
	private Vector2 GetTrapeziumPath(float t)
	{
		// Fix: Ensure t is between 0 and just under 1 to prevent array out of bounds
		t = Mathf.Clamp(t, 0f, 0.999f);
		
		// Trapezium path - 4 segments
		int segment = (int)(t * 4); // Which segment (0, 1, 2, or 3)
		
		// Fix: Add explicit bounds check to prevent array index out of bounds
		segment = Mathf.Clamp(segment, 0, 3);
		
		float segmentT = (t * 4) % 1.0f; // Position within segment (0 to 1)
		
		// Define trapezium vertices
		Vector2[] vertices = new Vector2[]
		{
			new Vector2(-_amplitude * 0.7f, -_amplitude * 0.7f), // Top left
			new Vector2(_amplitude * 0.7f, -_amplitude * 0.7f),  // Top right
			new Vector2(_amplitude, _amplitude * 0.7f),         // Bottom right
			new Vector2(-_amplitude, _amplitude * 0.7f)         // Bottom left
		};
		
		// Interpolate between vertices based on segment
		Vector2 start = vertices[segment];
		Vector2 end = vertices[(segment + 1) % 4];
		
		return start.LinearInterpolate(end, segmentT);
	}
	
	// Removing the direct drawing methods as we now use the classes
	// Removing: DrawRumahHonaiBerwarna, DrawRoofDirect, DrawZigzagDirect, DrawFenceDirect
	// Removing: DrawSateUlatSaguBerwarna 
	// Removing: DrawTasNokenBerwarna
	
	// Only keep the helper method for ellipses if needed elsewhere
	private void GambarElips(Vector2 pusat, float radiusX, float radiusY, Color warna)
	{
		int segments = 36;
		List<Vector2> titik = new List<Vector2>();
		
		for (int i = 0; i < segments; i++)
		{
			float theta = i * Mathf.Tau / segments;
			float x = pusat.x + radiusX * Mathf.Cos(theta);
			float y = pusat.y + radiusY * Mathf.Sin(theta);
			titik.Add(new Vector2(x, y));
		}
		
		DrawPolygon(titik.ToArray(), new Color[] { warna });
	}

	public override void _ExitTree()
	{
		if (_bentukDasar != null)
		{
			_bentukDasar.Dispose();
			_bentukDasar = null;
		}
		base._ExitTree();
	}
}
