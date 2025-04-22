using Godot;
using System;
using System.Collections.Generic;

public class Karya2 : Node2D
{
	private BentukDasar _bentukDasar = new BentukDasar();
	private const int MarginLeft = 50;
	private const int MarginTop = 50;
	
	// Animation variables
	private float _animationTime = 0f;
	private float _animationSpeed = 0.5f;
	private float _amplitude = 100f;  // Size of the animation path
	
	// Original positions for each cultural object
	private Vector2 _rumahHonaiPos;
	private Vector2 _sateUlatSaguPos;
	private Vector2 _tasNokenPos;

	public override void _Ready()
	{
		GetNode<Button>("BackButton4").Connect("pressed", this, nameof(OnButtonBackPressed));
		UpdateButtonLabel("BackButton4", "Kembali");
		
			// Initialize objects at their positions
		InitializeObjects();
	}

	private void InitializeObjects()
	{
		Vector2 windowSize = GetViewportRect().Size;
		int centerX = (int)windowSize.x / 2;
		int centerY = (int)windowSize.y / 2;
		
		// Initialize with original positions similar to Karya1
		_rumahHonaiPos = new Vector2(centerX - 200, centerY - 100);
		_sateUlatSaguPos = new Vector2(centerX, centerY);
		_tasNokenPos = new Vector2(centerX + 200, centerY + 100);
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

	public override void _Process(float delta)
	{
		_animationTime += delta * _animationSpeed;
		Update(); // Request redraw
	}

	public override void _Draw()
	{
		Vector2 windowSize = GetViewportRect().Size;
		int screenWidth = (int)windowSize.x;
		int screenHeight = (int)windowSize.y;
		int marginRight = screenWidth - MarginLeft;
		int marginBottom = screenHeight - MarginTop;

		// Draw margin
		MarginPixel(MarginLeft, MarginTop, marginRight, marginBottom);
		
		// Calculate positions based on animation time
		float normalizedTime = (_animationTime % 2.0f) / 2.0f; // Value between 0 and 1
		
		// 1. Rumah Honai - Triangle path
		Vector2 rumahHonaiOffset = GetTrianglePath(normalizedTime);
		Vector2 currentRumahHonaiPos = _rumahHonaiPos + rumahHonaiOffset;
		DrawRumahHonai(currentRumahHonaiPos);
		
		// 2. Sate Ulat Sagu - Circle path (O shape)
		Vector2 sateUlatSaguOffset = GetCirclePath(normalizedTime);
		Vector2 currentSateUlatSaguPos = _sateUlatSaguPos + sateUlatSaguOffset;
		DrawSateUlatSagu(currentSateUlatSaguPos);
		
		// 3. Tas Noken - Trapezium path
		Vector2 tasNokenOffset = GetTrapeziumPath(normalizedTime);
		Vector2 currentTasNokenPos = _tasNokenPos + tasNokenOffset;
		DrawTasNoken(currentTasNokenPos);
	}
	
	private Vector2 GetTrianglePath(float t)
	{
		// Triangle path - 3 segments
		int segment = (int)(t * 3); // Which segment (0, 1, or 2)
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
		// Trapezium path - 4 segments
		int segment = (int)(t * 4); // Which segment (0, 1, 2, or 3)
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
	
	private void DrawRumahHonai(Vector2 position)
	{
		// Create temporary object at the current position and draw outline
		RumahHonai tempRumah = new RumahHonai(this, position);
		tempRumah.GambarOutline();
	}
	
	private void DrawSateUlatSagu(Vector2 position)
	{
		// Create temporary object at the current position and draw outline
		SateUlatSagu tempSate = new SateUlatSagu(this, position);
		tempSate.GambarOutline();
	}
	
	private void DrawTasNoken(Vector2 position)
	{
		// Create temporary object at the current position and draw outline
		TasNoken tempTas = new TasNoken(this, position);
		tempTas.GambarOutline();
	}

	private void MarginPixel(int left, int top, int right, int bottom)
	{
		Color color = new Color("#32CD30");
		var margin = _bentukDasar.Margin(left, top, right, bottom);
		PutPixelAll(margin, color);
	}

	private void PutPixel(float x, float y, Color? color = null)
	{
		Color actualColor = color ?? Colors.White;
		
		// Replace DrawRect with DrawPrimitive for single point drawing
		Vector2[] points = { new Vector2(x, y) }; // Single point
		Color[] colors = { actualColor }; // Single color
		
		// DrawPrimitive with 1 point draws a point primitive
		DrawPrimitive(points, colors, null, null);
	}
	
	private void PutPixelAll(List<Vector2> dots, Color? color = null)
	{
		Color actualColor = color ?? Colors.White;
		
		// Process each point individually
		foreach (Vector2 point in dots)
		{
			PutPixel(point.x, point.y, actualColor);
		}
	}

	public override void _ExitTree()
	{
		_bentukDasar?.Dispose();
		_bentukDasar = null;
		base._ExitTree();
	}
}
