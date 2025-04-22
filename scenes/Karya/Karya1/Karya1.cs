using Godot;
using System;
using System.Collections.Generic;

public class Karya1 : Node2D
{
	private Vector2 pusatKartesius = new Vector2(640, 360);
	private BentukDasar _bentukDasar = new BentukDasar();

	public override void _Ready()
	{
		GetNode<Button>("BackButton3").Connect("pressed", this, nameof(OnButtonBackPressed));
		UpdateButtonLabel("BackButton3", "Kembali");
		GambarGarisKartesius(pusatKartesius);
		Update(); // untuk memicu _Draw
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

	public override void _Draw()
	{
		// Garis Kartesius
		GambarGarisKartesius(pusatKartesius);
		
		 // Remove quadrant labels to eliminate unwanted boxes
		// We'll only keep the essential object labels
		
		// Kuadran I (atas kanan) - Tas Noken
		Vector2 offsetNoken = new Vector2(250, -150); 
		new TasNoken(this, pusatKartesius + offsetNoken).GambarOutline();
		
		// Use simple label with minimal display
		DrawSimpleLabel(pusatKartesius + new Vector2(200, -250), "Tas Noken", Colors.White);

		// Kuadran II (atas kiri) - Rumah Honai
		Vector2 offsetHonai = new Vector2(-280, -180);
		new RumahHonai(this, pusatKartesius + offsetHonai).GambarOutline();
		DrawSimpleLabel(pusatKartesius + new Vector2(-300, -320), "Rumah Honai", Colors.White);

		// Kuadran III (bawah kiri) - empty for now
		
		// Kuadran IV (bawah kanan) - Sate Ulat Sagu
		Vector2 offsetSate = new Vector2(250, 150);
		new SateUlatSagu(this, pusatKartesius + offsetSate).GambarOutline();
		DrawSimpleLabel(pusatKartesius + new Vector2(200, 250), "Sate Ulat Sagu", Colors.White);
	}

	private void GambarGarisKartesius(Vector2 pusat)
	{
		// Draw x axis
		List<Vector2> xAxis = new List<Vector2>();
		for (int x = 0; x < 1280; x++)
		{
			xAxis.Add(new Vector2(x, pusat.y));
		}
		PutPixelAll(xAxis, Colors.White);

		// Draw y axis
		List<Vector2> yAxis = new List<Vector2>();
		for (int y = 0; y < 720; y++)
		{
			yAxis.Add(new Vector2(pusat.x, y));
		}
		PutPixelAll(yAxis, Colors.White);

		// Add minimal axis labels
		DrawSimpleLabel(new Vector2(pusat.x + 10, 20), "Y", Colors.White);
		DrawSimpleLabel(new Vector2(1260, pusat.y + 20), "X", Colors.White);
	}

	// Replace GambarTeks with a simpler version that doesn't draw boxes
	private void DrawSimpleLabel(Vector2 position, string text, Color color)
	{
		// Use a single point for each character - just to mark its position
		// This is a minimalist approach to avoid boxes while still placing visible markers
		float spacing = 5;
		
		Vector2 currentPos = position;
		
		for (int i = 0; i < text.Length; i++)
		{
			// Just place a single point for each character position
			PutPixel(currentPos.x, currentPos.y, color);
			currentPos.x += spacing;
		}
	}

	// New PutPixel method using DrawPrimitive
	private void PutPixel(float x, float y, Color? color = null)
	{
		Color actualColor = color ?? Colors.White;
		
		// Replace DrawRect with DrawPrimitive for single point drawing
		Vector2[] points = { new Vector2(x, y) }; // Single point
		Color[] colors = { actualColor }; // Single color
		
		// DrawPrimitive with 1 point draws a point primitive
		DrawPrimitive(points, colors, null, null);
	}
	
	// New PutPixelAll method
	private void PutPixelAll(List<Vector2> dots, Color? color = null)
	{
		Color actualColor = color ?? Colors.White;
		
		// Process each point individually
		foreach (Vector2 point in dots)
		{
			PutPixel(point.x, point.y, actualColor);
		}
	}

	// Simple bitmap-like text renderer
	private void GambarTeks(Vector2 position, string text, Color color)
	{
		// This is a simple implementation - for each character, we'll draw a simple dot pattern
		// In a real app, you'd want a proper bitmap font
		float charWidth = 8;
		float charHeight = 12;
		float spacing = 2;
		
		Vector2 currentPos = position;
		
		foreach (char c in text)
		{
			// Draw a simple representation of each character
			// This is extremely simplified - in practice you'd want actual character bitmap data
			List<Vector2> charPixels = GetCharPixels(c, currentPos, charWidth, charHeight);
			PutPixelAll(charPixels, color);
			
			currentPos.x += charWidth + spacing;
		}
	}
	
	// Helper to generate pixels for a character
	private List<Vector2> GetCharPixels(char c, Vector2 pos, float width, float height)
	{
		List<Vector2> pixels = new List<Vector2>();
		
		// Very basic character rendering - just the character shape outline
		// For alphabetic characters, make a simple shape
		if (char.IsLetter(c))
		{
			// Draw a simple box outline for each letter
			// Top horizontal line
			for (float x = pos.x; x < pos.x + width; x++)
				pixels.Add(new Vector2(x, pos.y));
			
			// Bottom horizontal line
			for (float x = pos.x; x < pos.x + width; x++)
				pixels.Add(new Vector2(x, pos.y + height));
			
			// Left vertical line
			for (float y = pos.y; y < pos.y + height; y++)
				pixels.Add(new Vector2(pos.x, y));
			
			// Right vertical line
			for (float y = pos.y; y < pos.y + height; y++)
				pixels.Add(new Vector2(pos.x + width, y));
			
			// For some letters, add distinguishing features
			switch(char.ToUpper(c))
			{
				case 'A':
					// Middle horizontal line
					for (float x = pos.x; x < pos.x + width; x++)
						pixels.Add(new Vector2(x, pos.y + height/2));
					break;
				case 'H':
					// Middle horizontal line
					for (float x = pos.x; x < pos.x + width; x++)
						pixels.Add(new Vector2(x, pos.y + height/2));
					break;
				case 'I':
					// Add middle vertical line
					for (float y = pos.y; y <= pos.y + height; y++)
						pixels.Add(new Vector2(pos.x + width/2, y));
					break;
				// Add more cases as needed
			}
		}
		else if (c == ' ')
		{
			// Space - leave empty
		}
		else
		{
			// For numbers and special chars, make a simple representation
			// Draw a diagonal line
			for (int i = 0; i < width; i++)
			{
				pixels.Add(new Vector2(pos.x + i, pos.y + i * height/width));
			}
		}
		
		return pixels;
	}
	
	public override void _ExitTree()
	{
		_bentukDasar?.Dispose();
		_bentukDasar = null;
		base._ExitTree();
	}
}
