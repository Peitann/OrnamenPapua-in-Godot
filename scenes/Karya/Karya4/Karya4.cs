using Godot;
using System;
using System.Collections.Generic;

public class Karya4 : Node2D
{
	private Vector2 pusatKartesius = new Vector2(640, 360);
	
	// Animation parameters
	private float _animationTime = 0f;
	private float _animationSpeed = 0.5f; // Base animation speed
	private float _amplitude = 100f;  // Size of the animation path
	
	// Rumah Honai animation parameters
	private Vector2 _rumahHonaiPos;
	
	// Sate Ulat Sagu parameters
	private Vector2 _sateUlatSaguPos;
	private int _grubsPerSkewer = 5; // Default number of grubs per skewer
	private int _sateCount = 1; // Default number of skewers
	private float _sateRotationSpeed = 1.0f; // Rotation speed multiplier
	private float _sateSpacing = 70f; // Spacing between skewers
	
	// Tas Noken parameters
	private Vector2 _tasNokenPos;
	private Vector2 _stickmanPos; // Position of the stickman
	private Vector2 _stickmanTargetPos; // Target position for stickman movement
	private float _stickmanMoveSpeed = 150f; // Units per second
	private bool _stickmanIsMoving = false;
	private float _tasScale = 0.4f; // Reduced from 0.6f to make the bag smaller

	// Add TransformasiFast instance
	private TransformasiFast _transformasi = new TransformasiFast();

	// Color scheme variables
	private int _currentColorScheme = 0;
	private readonly ColorScheme[] _colorSchemes = new ColorScheme[]
	{
		// Original color scheme
		new ColorScheme 
		{
			// RumahHonai colors
			RumahDindingColor = new Color(0.5f, 0.25f, 0),
			RumahAtapColor = new Color(1.0f, 0.89f, 0.77f),
			RumahZigzagColor = new Color(0.6f, 0.3f, 0.1f),
			RumahPagarColor = Colors.Gray,
			RumahPintuColor = Colors.Black,
			
			// SateUlatSagu colors
			SateTusukColor = new Color(0.36f, 0.25f, 0.2f),
			SateUlatColor = new Color(1.0f, 0.9f, 0.8f),
			SateDetailColor = new Color(0.3f, 0.15f, 0.05f),
			
			// TasNoken colors
			TasBadanColor = new Color(0.6f, 0.3f, 0.1f),
			TasTaliColor = new Color(0.4f, 0.2f, 0.1f),
			TasMotifColor = Colors.DarkRed,
			TasOrnamenColor = new Color(1f, 0.7f, 0.2f),
		},
		
		// Alternative color scheme - Papua-inspired, more vibrant
		new ColorScheme
		{
			// RumahHonai colors - warmer, more reddish
			RumahDindingColor = new Color(0.6f, 0.3f, 0.1f),
			RumahAtapColor = new Color(0.9f, 0.8f, 0.6f),
			RumahZigzagColor = new Color(0.7f, 0.35f, 0.2f),
			RumahPagarColor = new Color(0.5f, 0.3f, 0.2f),
			RumahPintuColor = new Color(0.2f, 0.1f, 0.05f),
			
			// SateUlatSagu colors - more pronounced
			SateTusukColor = new Color(0.45f, 0.3f, 0.15f),
			SateUlatColor = new Color(0.95f, 0.85f, 0.65f),
			SateDetailColor = new Color(0.4f, 0.2f, 0.1f),
			
			// TasNoken colors - traditional elements enhanced
			TasBadanColor = new Color(0.75f, 0.4f, 0.15f),
			TasTaliColor = new Color(0.5f, 0.25f, 0.1f),
			TasMotifColor = new Color(0.8f, 0.2f, 0.1f), // Brighter red for motifs
			TasOrnamenColor = new Color(0.95f, 0.8f, 0.3f), // Brighter yellow
		}
	};

	public override void _Ready()
	{
		GetNode<Button>("BackButton6").Connect("pressed", this, nameof(OnButtonBackPressed));
		UpdateButtonLabel("BackButton6", "Kembali");
		
		// Initialize positions
		_rumahHonaiPos = new Vector2(pusatKartesius.x - 200, pusatKartesius.y - 150);
		// Move Sate to quadrant 3 (bottom-left)
		_sateUlatSaguPos = new Vector2(pusatKartesius.x - 200, pusatKartesius.y + 150);
		_tasNokenPos = new Vector2(pusatKartesius.x + 200, pusatKartesius.y + 100);
		
		// Initialize stickman position near the tas noken
		_stickmanPos = _tasNokenPos + new Vector2(-30, 0);
		_stickmanTargetPos = _stickmanPos;
		
		Update(); // trigger _Draw
		
		// Add instructions label
		AddInstructionsLabel();
	}
	
	private void AddInstructionsLabel()
	{
		Label instructionsLabel = new Label();
		instructionsLabel.Text = "Up Arrow/Down Arrow: Change Rumah Honai speed\nM/N: Add/Remove Sate skewers\nClick: Move Stickman\n1/2: Switch color schemes";
		instructionsLabel.RectPosition = new Vector2(10, 10);
		instructionsLabel.AddColorOverride("font_color", Colors.White);
		AddChild(instructionsLabel);
	}
	
	private void UpdateButtonLabel(string buttonPath, string newText)
	{
		var button = GetNode<Button>(buttonPath);
		var label = button.GetNode<Label>("Label");
		label.Text = newText;
	}

	private void OnButtonBackPressed()
	{
		var menuScene = (PackedScene)GD.Load("res://scenes/Menu/Menu.tscn");
		GetTree().ChangeSceneTo(menuScene);
	}
	
	public override void _Input(InputEvent @event)
	{
		// Handle keyboard input
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			// 1. Arrow keys for Rumah Honai speed
			if (keyEvent.Scancode == (uint)KeyList.Up)
			{
				// Increase speed
				_animationSpeed += 0.1f;
				GD.Print($"Speed increased to {_animationSpeed}");
			}
			else if (keyEvent.Scancode == (uint)KeyList.Down)
			{
				// Decrease speed, but not below a minimum
				_animationSpeed = Mathf.Max(0.1f, _animationSpeed - 0.1f);
				GD.Print($"Speed decreased to {_animationSpeed}");
			}
			
			// 2. M/N keys for entire Sate Ulat Sagu skewers (not just grubs)
			if (keyEvent.Scancode == (uint)KeyList.M)
			{
				// Increase the number of skewers, with a reasonable maximum
				_sateCount = Mathf.Min(5, _sateCount + 1); // Maximum 5 skewers
				GD.Print($"Sate skewers: {_sateCount}");
			}
			else if (keyEvent.Scancode == (uint)KeyList.N)
			{
				// Decrease the number of skewers, but not below 1
				_sateCount = Mathf.Max(1, _sateCount - 1);
				GD.Print($"Sate skewers: {_sateCount}");
				}
			
			// Add new handlers for color scheme switching
			else if (keyEvent.Scancode == (uint)KeyList.Key1)
			{
				// Switch to original color scheme
				_currentColorScheme = 0;
				GD.Print("Switched to original colors");
				Update(); // Request redraw
			}
			else if (keyEvent.Scancode == (uint)KeyList.Key2)
			{
				// Switch to alternative color scheme
				_currentColorScheme = 1;
				GD.Print("Switched to alternative colors");
				Update(); // Request redraw
			}
		}
		
		// 3. Handle mouse clicks for stickman movement
		if (@event is InputEventMouseButton mouseEvent && 
			mouseEvent.Pressed && mouseEvent.ButtonIndex == 1) // Left click
		{
			_stickmanTargetPos = mouseEvent.Position;
			_stickmanIsMoving = true;
		}
	}

	public override void _Process(float delta)
	{
		// Update animation time for Rumah Honai
		_animationTime += delta * _animationSpeed;
		
		// Update stickman movement towards target position using Transformasi
		if (_stickmanIsMoving)
		{
			// Use MoveTowards from Transformasi
			_stickmanPos = Transformasi.MoveTowards(
				_stickmanPos, 
				_stickmanTargetPos, 
				_stickmanMoveSpeed * delta
			);
			
			// Check if we're close enough to stop moving
			float distance = (_stickmanTargetPos - _stickmanPos).Length();
			if (distance <= 5)
			{
				_stickmanIsMoving = false;
			}
			
			// Update TasNoken position to follow stickman with offset
			_tasNokenPos = _stickmanPos + new Vector2(20, 0);
		}
		
		Update(); // Request redraw
	}

	public override void _Draw()
	{
		Vector2 windowSize = GetViewportRect().Size;
		
			// Get current color scheme
		ColorScheme colors = _colorSchemes[_currentColorScheme];
		
		// 1. Draw Rumah Honai with triangle path animation using TransformasiFast
		float normalizedTime = (_animationTime % 2.0f) / 2.0f;
		
		// Get triangle path using TransformasiFast
		Vector2 currentRumahHonaiPos = TransformasiFast.GetTrianglePath(
			_rumahHonaiPos, _amplitude, normalizedTime);
		
		// Use RumahHonai class with transformed position and custom colors
		RumahHonai rumahHonai = new RumahHonai(this, currentRumahHonaiPos);
		// Pass the current color scheme to the RumahHonai
		rumahHonai.SetColors(
			colors.RumahDindingColor, 
			colors.RumahAtapColor, 
			colors.RumahZigzagColor, 
			colors.RumahPagarColor,
			colors.RumahPintuColor
		);
		rumahHonai.GambarBerwarna();

		// 2. Draw multiple Sate Ulat Sagu with circular animation using TransformasiFast
		float baseAngle = (_animationTime * _sateRotationSpeed) % (Mathf.Pi * 2);
		
		// Draw multiple sate skewers
		for (int skewer = 0; skewer < _sateCount; skewer++)
		{
			// Calculate position for this skewer with some spacing
			float offsetAngle = (float)skewer * (Mathf.Pi / 6);
			float currentAngle = baseAngle + offsetAngle;
			
			// Get circular path using Transformasi
			Vector2 sateOffset = Transformasi.GetCirclePath(currentAngle/(Mathf.Pi*2), _amplitude * 0.7f);
			Vector2 currentSatePos = _sateUlatSaguPos + sateOffset;
			
			// Draw sate with transformed position and custom colors
			SateUlatSagu sate = new SateUlatSagu(this, currentSatePos, _grubsPerSkewer);
			// Pass the current color scheme to the SateUlatSagu
			sate.SetColors(
				colors.SateTusukColor,
				colors.SateUlatColor,
				colors.SateDetailColor
			);
			sate.GambarBerwarna();
		}

		// 3. Draw stickman and Tas Noken using their current positions
		DrawStickMan(_stickmanPos);
		
		// Use TasNoken class with custom colors
		TasNoken tas = new TasNoken(this, _tasNokenPos, _tasScale);
		// Pass the current color scheme to the TasNoken
		tas.SetColors(
			colors.TasBadanColor,
			colors.TasTaliColor,
			colors.TasMotifColor,
			colors.TasOrnamenColor
		);
		tas.GambarBerwarna();
	}
	
	private void DrawStickMan(Vector2 position)
	{
		// Draw a simple stickman
		float headRadius = 15;
		float bodyLength = 40;
		float limbLength = 30;
		
		// Head
		DrawCircle(position, headRadius, Colors.Black);
		
		// Body
		Vector2 neckPos = position + new Vector2(0, headRadius);
		Vector2 pelvisPos = neckPos + new Vector2(0, bodyLength);
		DrawLine(neckPos, pelvisPos, Colors.Black, 2);
		
		// Arms
		Vector2 shoulderPos = neckPos + new Vector2(0, bodyLength * 0.3f);
		Vector2 leftHand = shoulderPos + new Vector2(-limbLength, 0);
		Vector2 rightHand = shoulderPos + new Vector2(limbLength, 0);
		DrawLine(shoulderPos, leftHand, Colors.Black, 2);
		DrawLine(shoulderPos, rightHand, Colors.Black, 2);
		
		// Legs
		Vector2 leftFoot = pelvisPos + new Vector2(-limbLength * 0.7f, limbLength);
		Vector2 rightFoot = pelvisPos + new Vector2(limbLength * 0.7f, limbLength);
		DrawLine(pelvisPos, leftFoot, Colors.Black, 2);
		DrawLine(pelvisPos, rightFoot, Colors.Black, 2);
	}
}

// Color scheme structure to hold all colors for each object
public struct ColorScheme
{
	// RumahHonai colors
	public Color RumahDindingColor;
	public Color RumahAtapColor; 
	public Color RumahZigzagColor;
	public Color RumahPagarColor;
	public Color RumahPintuColor;
	
	// SateUlatSagu colors
	public Color SateTusukColor;
	public Color SateUlatColor;
	public Color SateDetailColor;
	
	// TasNoken colors
	public Color TasBadanColor;
	public Color TasTaliColor;
	public Color TasMotifColor;
	public Color TasOrnamenColor;
}
