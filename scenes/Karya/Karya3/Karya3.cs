using Godot;
using System;
using System.Collections.Generic;

public class Karya3 : Node2D
{
	// Animation mode management
	private bool _isStoryboardMode = true; // Default to storyboard mode
	private Karya3Animasi2 _pathAnimation;
	
	private BentukDasar _bentukDasar = new BentukDasar();
	private float _animationTime = 0f;
	
	// Animation states
	private enum AnimationState { HOUSE_MOVING, MAN_EXITING, SATE_APPEARING, SATE_SMOKING }
	private AnimationState _currentState = AnimationState.HOUSE_MOVING;
	
	// Timing for animation phases (in seconds)
	private const float HOUSE_MOVE_DURATION = 3.0f;
	private const float MAN_EXIT_DURATION = 3.0f;
	private const float SATE_APPEAR_DURATION = 3.0f;
	
	// Screen properties
	private Vector2 _screenCenter;
	private float _screenWidth;
	private float _screenHeight;
	
	// Animation parameters
	private float _houseMoveProgress = 0f;
	private float _manExitProgress = 0f;
	private float _sateAppearProgress = 0f;
	private float _smokeIntensity = 0f;
	
	// Smoke particles
	private List<SmokeParticle> _smokeParticles = new List<SmokeParticle>();
	private Random _random = new Random();

	// Add TransformasiFast instance
	private TransformasiFast _transformasi = new TransformasiFast();

	// Color scheme similar to Karya3Animasi2
	private Color _sateTusukColor = new Color(0.36f, 0.25f, 0.2f);
	private Color _sateUlatColor = new Color(1.0f, 0.9f, 0.8f);
	private Color _sateDetailColor = new Color(0.3f, 0.15f, 0.05f);

	public override void _Ready()
	{
		GetNode<Button>("BackButton5").Connect("pressed", this, nameof(OnButtonBackPressed));
		UpdateButtonLabel("BackButton5", "Kembali");
		
		 // Connect animation buttons
		GetNode<Button>("Animasi1").Connect("pressed", this, nameof(OnAnimasi1Pressed));
		UpdateButtonLabel("Animasi1", "Animasi 1");
		
		GetNode<Button>("Animasi2").Connect("pressed", this, nameof(OnAnimasi2Pressed));
		UpdateButtonLabel("Animasi2", "Animasi 2");
		
		// Get screen properties
		Vector2 windowSize = GetViewportRect().Size;
		_screenCenter = new Vector2(windowSize.x / 2, windowSize.y / 2);
		_screenWidth = windowSize.x;
		_screenHeight = windowSize.y;
		
		 // Add instructions label
		AddInstructionsLabel();
		
		// Initialize the animation parameters to ensure animation starts properly
		ResetAnimation();
		
		// Ensure storyboard mode is set correctly
		_isStoryboardMode = true;
	}
	
	public override void _Process(float delta)
	{
		if (_isStoryboardMode)
		{
			_animationTime += delta;
			
			// Add debug message to confirm animation is running
			if (_animationTime % 1.0f < delta)  // Approximately once per second
				GD.Print($"Animation Time: {_animationTime}, State: {_currentState}, Progress: {_houseMoveProgress}");
			
			// Update animation state based on time
			UpdateAnimationState();
			
			// Update animation parameters based on state
			UpdateAnimationParameters(delta);
			
			Update(); // Request redraw
		}
		// Path animation mode is handled in its own class
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
		var label = button.GetNode<Label>("Label");
		label.Text = newText;
	}

	private void OnButtonBackPressed()
	{
		var menuScene = (PackedScene)GD.Load("res://scenes/Menu/Menu.tscn");
		GetTree().ChangeSceneTo(menuScene);
	}

	private void OnAnimasi1Pressed()
	{
		// Stay on current animation (Animasi1/Karya3)
		// Reset the animation state if needed
		ResetAnimation();
	}

	private void OnAnimasi2Pressed()
	{
		// Switch to Animasi2/Karya3Animasi2
		var pathAnimScene = (PackedScene)GD.Load("res://scenes/Karya/Karya3/Karya3Animasi2.tscn");
		GetTree().ChangeSceneTo(pathAnimScene);
	}

	private void ResetAnimation()
	{
		// Reset the animation to its starting state
		_animationTime = 0;
		_currentState = AnimationState.HOUSE_MOVING;
		_smokeParticles.Clear();
		_houseMoveProgress = 0f;
		_manExitProgress = 0f;
		_sateAppearProgress = 0f;
		_smokeIntensity = 0f;
	}

	
	private void UpdateAnimationState()
	{
		// Transition between states based on time
		if (_currentState == AnimationState.HOUSE_MOVING && _animationTime >= HOUSE_MOVE_DURATION)
		{
			_currentState = AnimationState.MAN_EXITING;
		}
		else if (_currentState == AnimationState.MAN_EXITING && 
				 _animationTime >= HOUSE_MOVE_DURATION + MAN_EXIT_DURATION)
		{
			_currentState = AnimationState.SATE_APPEARING;
		}
		else if (_currentState == AnimationState.SATE_APPEARING && 
				 _animationTime >= HOUSE_MOVE_DURATION + MAN_EXIT_DURATION + SATE_APPEAR_DURATION)
		{
			_currentState = AnimationState.SATE_SMOKING;
		}
	}
	
	private void UpdateAnimationParameters(float delta)
	{
		// Update parameters based on current animation state
		switch (_currentState)
		{
			case AnimationState.HOUSE_MOVING:
				// House moves from left to right
				_houseMoveProgress = Mathf.Min(1.0f, _animationTime / HOUSE_MOVE_DURATION);
				break;
				
			case AnimationState.MAN_EXITING:
				// Man moves from inside the house to outside
				_manExitProgress = Mathf.Min(1.0f, 
					(_animationTime - HOUSE_MOVE_DURATION) / MAN_EXIT_DURATION);
				break;
				
			case AnimationState.SATE_APPEARING:
				// Sate appears from the bag
				_sateAppearProgress = Mathf.Min(1.0f, 
					(_animationTime - HOUSE_MOVE_DURATION - MAN_EXIT_DURATION) / SATE_APPEAR_DURATION);
				break;
				
			case AnimationState.SATE_SMOKING:
				// Smoke intensity increases
				_smokeIntensity = Mathf.Min(1.0f, 
					(_animationTime - HOUSE_MOVE_DURATION - MAN_EXIT_DURATION - SATE_APPEAR_DURATION) * 0.5f);
				
				// Increase smoke generation frequency
				if (_random.NextDouble() < 0.2f * _smokeIntensity)
				{
					Vector2 satePos = CalculateSatePosition();
					
					// Create larger smoke particles with higher starting position
					_smokeParticles.Add(new SmokeParticle(
						satePos + new Vector2(0, -30), // Higher starting position for the larger sate
						new Vector2((float)(_random.NextDouble() * 2 - 1) * 8, -15 - (float)(_random.NextDouble() * 10)),
						1.0f + (float)_random.NextDouble() * 1.5f, // Much larger initial particles (was 0.2-1.0)
						1.5f + (float)_random.NextDouble() * 2.0f  // Longer lifetime for more visibility
					));
				}
				
				// Update existing smoke particles
				for (int i = _smokeParticles.Count - 1; i >= 0; i--)
				{
					_smokeParticles[i].Update(delta);
					if (_smokeParticles[i].IsDead)
					{
						_smokeParticles.RemoveAt(i);
					}
				}
				break;
		}
	}

	public override void _Draw()
	{
		// Draw objects based on current animation state
		switch (_currentState)
		{
			case AnimationState.HOUSE_MOVING:
				DrawMovingHouse();
				break;
				
			case AnimationState.MAN_EXITING:
				DrawStaticHouse();
				DrawExitingMan();
				break;
				
			case AnimationState.SATE_APPEARING:
				DrawStaticHouse();
				DrawManWithBags();
				DrawAppearingSate();
				break;
				
			case AnimationState.SATE_SMOKING:
				DrawStaticHouse();
				DrawManWithBags();
				DrawSateWithSmoke();
				break;
		}
	}
	
	private void DrawMovingHouse()
	{
		// Calculate house position moving from left to right using Transformasi
		float startX = -200; // Start off-screen left
		float endX = _screenCenter.x;
		
		 // Ensure progress is clamped between 0 and 1
		_houseMoveProgress = Mathf.Clamp(_houseMoveProgress, 0.0f, 1.0f);
		
		// Get interpolated position using Transformasi
		Vector2 startPos = new Vector2(startX, _screenCenter.y);
		Vector2 endPos = new Vector2(endX, _screenCenter.y);
		Vector2 currentPos = Transformasi.LinearInterpolate(startPos, endPos, _houseMoveProgress);
		
		// Use RumahHonai class with the transformed position
		RumahHonai rumahHonai = new RumahHonai(this, currentPos);
		rumahHonai.GambarBerwarna();
	}
	
	private void DrawStaticHouse()
	{
		// Draw house at its final position using RumahHonai class
		Vector2 housePosition = new Vector2(_screenCenter.x, _screenCenter.y);
		RumahHonai rumahHonai = new RumahHonai(this, housePosition);
		rumahHonai.GambarBerwarna();
	}
	
	private Vector2 GetHousePosition()
	{
		return new Vector2(_screenCenter.x, _screenCenter.y);
	}
	
	private Vector2 GetHouseDoorPosition()
	{
		// Door is offset from the house center
		return GetHousePosition() + new Vector2(75, 0);
	}
	
	private void DrawExitingMan()
	{
		// Start position is at the house door
		Vector2 doorPosition = GetHouseDoorPosition();
		// End position is a bit to the right of the house
		Vector2 finalPosition = doorPosition + new Vector2(150, 0);
		
		// Calculate current position using Transformasi
		Vector2 currentPosition = Transformasi.LinearInterpolate(
			doorPosition, finalPosition, _manExitProgress);
		
		// Draw stickman with bags
		DrawStickMan(currentPosition);
		DrawTwoBags(currentPosition);
	}
	
	private void DrawManWithBags()
	{
		// Man position is fixed after exiting the house
		Vector2 manPosition = GetHouseDoorPosition() + new Vector2(150, 0);
		
		// Draw stickman with bags
		DrawStickMan(manPosition);
		DrawTwoBags(manPosition);
	}
	
	private void DrawAppearingSate()
	{
		// Calculate sate position emerging from one of the bags
		Vector2 satePos = CalculateSatePosition();
		DrawSateUlatSagu(satePos, _sateAppearProgress);
	}
	
	private void DrawSateWithSmoke()
	{
		// Draw fully visible sate
		Vector2 satePos = CalculateSatePosition();
		DrawSateUlatSagu(satePos, 1.0f);
		
		// Draw more visible smoke particles
		foreach (var particle in _smokeParticles)
		{
			// Make smoke more visible
			DrawSmoke(particle);
		}
	}
	
	private Vector2 CalculateSatePosition()
	{
		// Get the position of the man
		Vector2 manPosition = GetHouseDoorPosition() + new Vector2(150, 0);
		
		// Position relative to the right bag
		Vector2 rightBagPosition = manPosition + new Vector2(30, 10);
		
		// Starting position is inside the bag, ending position is outside
		if (_currentState == AnimationState.SATE_APPEARING)
		{
			// During appearance, move from inside bag to outside
			return rightBagPosition + new Vector2(
				20 * _sateAppearProgress, 
				-20 * _sateAppearProgress
			);
		}
		else
		{
			// Final position
			return rightBagPosition + new Vector2(20, -20);
		}
	}
	
	private void DrawStickMan(Vector2 position)
	{
		// Draw a simple stickman
		float headRadius = 15;
		float bodyLength = 40;
		float limbLength = 30;
		
		// Head (circle)
		DrawCircle(position, headRadius, Colors.Black);
		
		// Body (line)
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
	
	private void DrawTwoBags(Vector2 manPos)
	{
		// Draw left bag using TasNoken class
		TasNoken tasKiri = new TasNoken(this, manPos + new Vector2(-30, 10), 0.6f);
		tasKiri.GambarBerwarna();
		
		// Draw right bag - this one will have the sate coming out
		TasNoken tasKanan = new TasNoken(this, manPos + new Vector2(30, 10), 0.6f, 
			_currentState == AnimationState.SATE_APPEARING || _currentState == AnimationState.SATE_SMOKING);
		tasKanan.GambarBerwarna();
	}
	
	private void DrawSateUlatSagu(Vector2 position, float visibility)
	{
		// Only draw if visibility is above threshold
		if (visibility > 0.1f)
		{
			// Use the same grub count (5) as in Karya3Animasi2 instead of 2
			// Remove scaling to match the regular size
			SateUlatSagu sate = new SateUlatSagu(this, position, 5);
			
			// Apply colors to match Karya3Animasi2
			sate.SetColors(_sateTusukColor, _sateUlatColor, _sateDetailColor);
			
			// Apply visibility by directly scaling the Node2D
			// Only scale if visibility is less than 1 (during appearance animation)
			if (visibility < 1.0f)
			{
				Vector2 originalScale = Scale;
				Scale = new Vector2(visibility, visibility);
				sate.GambarBerwarna();
				Scale = originalScale;
			}
			else
			{
				sate.GambarBerwarna();
			}
		}
	}
	
	// Helper function to get and set the global scale
	private float GetMasterScale()
	{
		return Scale.x; // Assuming uniform scaling
	}
	
	private void SetMasterScale(float scale)
	{
		Scale = new Vector2(scale, scale);
	}
	
	private void DrawSmoke(SmokeParticle particle)
	{
		// Draw smoke as a more visible translucent circle with higher opacity
		Color smokeColor = new Color(0.95f, 0.95f, 0.95f, particle.Alpha * 1.5f); // Increased opacity
		DrawCircle(particle.Position, particle.Size, smokeColor);
	}
	
	// Helper method to draw ellipse
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
		
	private void DrawRumahHonaiBerwarnaDirect(Vector2 position)
	{
		// Dimensions and offsets
		float tinggiDinding = 150;
		float radiusAtap = 120;
		Vector2 ukuranDinding = new Vector2(200, tinggiDinding);
		Vector2 posisiDinding = position + new Vector2(-ukuranDinding.x / 2, -ukuranDinding.y / 2 - 50);
		
		// Draw the wall using DrawPolygon instead of DrawRect
		Vector2[] wallPoints = new Vector2[] {
			posisiDinding,
			posisiDinding + new Vector2(ukuranDinding.x, 0),
			posisiDinding + new Vector2(ukuranDinding.x, ukuranDinding.y),
			posisiDinding + new Vector2(0, ukuranDinding.y)
		};
		DrawPolygon(wallPoints, new Color[] { new Color(0.5f, 0.25f, 0) });
		
		// Draw the roof directly (already using DrawPolygon)
		Vector2 posisiAtap = new Vector2(posisiDinding.x + ukuranDinding.x / 2, posisiDinding.y);
		DrawRoofDirect(posisiAtap, radiusAtap);
		
		// Draw zigzag pattern
		DrawZigzagDirect(posisiDinding + new Vector2(0, 40), ukuranDinding.x, 20, 10, new Color(0.6f, 0.3f, 0.1f));
		
		// Draw door using DrawPolygon instead of DrawRect
		Vector2 posisiPintu = posisiDinding + new Vector2(75, 80);
		Vector2[] doorPoints = new Vector2[] {
			posisiPintu,
			posisiPintu + new Vector2(50, 0),
			posisiPintu + new Vector2(50, 70),
			posisiPintu + new Vector2(0, 70)
		};
		DrawPolygon(doorPoints, new Color[] { Colors.Black });
		
		// Draw fence
		DrawFenceDirect(posisiDinding + new Vector2(-40, 150), new Vector2(280, 60));
	}

	private void DrawFenceDirect(Vector2 position, Vector2 size)
	{
		// Top bar using DrawPolygon
		Vector2[] topBarPoints = new Vector2[] {
			position,
			position + new Vector2(size.x, 0),
			position + new Vector2(size.x, 5),
			position + new Vector2(0, 5)
		};
		DrawPolygon(topBarPoints, new Color[] { Colors.LightGray });
		
		// Bottom bar using DrawPolygon
		Vector2[] bottomBarPoints = new Vector2[] {
			position + new Vector2(0, size.y - 5),
			position + new Vector2(size.x, size.y - 5),
			position + new Vector2(size.x, size.y),
			position + new Vector2(0, size.y)
		};
		DrawPolygon(bottomBarPoints, new Color[] { Colors.LightGray });
		
		// Vertical posts using DrawPolygon
		int posts = 9;
		float postWidth = size.x / 10;
		float spacing = size.x / posts;
		
		for (int i = 0; i < posts; i++)
		{
			Vector2 postPos = position + new Vector2(i * spacing, 5);
			Vector2[] postPoints = new Vector2[] {
				postPos,
				postPos + new Vector2(postWidth, 0),
				postPos + new Vector2(postWidth, size.y - 10),
				postPos + new Vector2(0, size.y - 10)
			};
			DrawPolygon(postPoints, new Color[] { Colors.Gray });
		}
	}
	private void DrawRoofDirect(Vector2 position, float radius)
	{
		// Draw the roof using DrawPolygon
		int segments = 36;
		Vector2[] roofPoints = new Vector2[segments];
		
		for (int i = 0; i < segments; i++)
		{
			float theta = i * Mathf.Tau / segments;
			float x = position.x + radius * Mathf.Cos(theta);
			float y = position.y + radius * Mathf.Sin(theta);
			roofPoints[i] = new Vector2(x, y);
		}
		
		DrawPolygon(roofPoints, new Color[] { Colors.Brown });
	}
	
	private void DrawZigzagDirect(Vector2 start, float width, float height, int segments, Color color)
	{
		for (int i = 0; i < segments; i++)
		{
			float x1 = start.x + i * width / segments;
			float y1 = start.y + (i % 2 == 0 ? 0 : -height);
			
			float x2 = start.x + (i + 1) * width / segments;
			float y2 = start.y + ((i + 1) % 2 == 0 ? 0 : -height);
			
			DrawLine(new Vector2(x1, y1), new Vector2(x2, y2), color, 3);
		}
	}
}

// Helper class for smoke particles
public class SmokeParticle
{
	public Vector2 Position { get; set; }
	public Vector2 Velocity { get; set; }
	public float Size { get; set; }
	public float Lifetime { get; set; }
	public float Age { get; private set; }
	// Modified Alpha property to maintain higher visibility
	public float Alpha => 0.85f * (1 - Age / Lifetime); // Increased from 0.7f to 0.85f
	public bool IsDead => Age >= Lifetime;
	
	public SmokeParticle(Vector2 position, Vector2 velocity, float size, float lifetime)
	{
		Position = position;
		Velocity = velocity;
		Size = size;
		Lifetime = lifetime;
		Age = 0;
	}
	
	public void Update(float delta)
	{
		// Use transformasi for position update
		TransformasiFast transform = new TransformasiFast();
		transform.Translasi(Velocity.x * delta, Velocity.y * delta);
		Position = transform.ApplySingle(Position);
		
		Size += 3 * delta; // Grow faster (was 2 * delta) for more visibility
		Age += delta;
	}
}
