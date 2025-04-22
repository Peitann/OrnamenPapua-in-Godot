using Godot;
using System;
using System.Collections.Generic;

public class RumahHonai
{
	private Node2D _node;
	private Vector2 _offset;
	private BentukDasar _bentuk = new BentukDasar();
	private float _radiusAtap = 120;
	// Position dinding now at Y = -50 to position it higher
	private Vector2 posisiDinding = new Vector2(0, -50);
	private Vector2 ukuranDinding = new Vector2(200, 150);
	
	// Default colors
	private Color _dindingColor = new Color(0.5f, 0.25f, 0);
	private Color _atapColor = new Color(1.0f, 0.89f, 0.77f);
	private Color _zigzagColor = new Color(0.6f, 0.3f, 0.1f);
	private Color _pagarColor = Colors.Gray;
	private Color _pintuColor = Colors.Black;

	public RumahHonai(Node2D node, Vector2 offset)
	{
		_node = node;
		_offset = offset;
	}

	// Method to set custom colors
	public void SetColors(Color dindingColor, Color atapColor, Color zigzagColor, Color pagarColor, Color pintuColor)
	{
		_dindingColor = dindingColor;
		_atapColor = atapColor;
		_zigzagColor = zigzagColor;
		_pagarColor = pagarColor;
		_pintuColor = pintuColor;
	}

	// =============================
	// KARYA 1 & 2 : OUTLINE ONLY
	// =============================
	public void GambarOutline()
	{
		GambarDinding(false);
		GambarPintu(false);
		GambarAtap(false);
		GambarMotifAtapOutline();
		GambarPagar(false);
		GambarMotifZigZag(false); // Modified to pass false for outline mode
	}

	// =============================
	// KARYA 3 & 4 : WARNA
	// =============================
	public void GambarBerwarna()
	{
		GambarDinding(true);
		GambarPintu(true);
		GambarAtapBerwarna();
		GambarPagar(true);
		GambarMotifZigZag(true);
	}

	// =============================
	// Bagian DINDING
	// =============================
	private void GambarDinding(bool isi)
	{
		Vector2 posisi = posisiDinding + _offset;
		if (isi)
			// Use custom dinding color
			_node.DrawRect(new Rect2(posisi, ukuranDinding), _dindingColor);
		else
		{
			List<Vector2> titik = _bentuk.Persegi(posisi.x, posisi.y, ukuranDinding.x, ukuranDinding.y);
			foreach (var t in titik)
				_node.DrawRect(new Rect2(t, new Vector2(1, 1)), Colors.White);
		}
	}

	// Modified: Added boolean parameter to control outline/color mode
	private void GambarMotifZigZag(bool isi)
	{
		Vector2 posisi = posisiDinding + _offset + new Vector2(0, 40);
		
		if (isi)
		{
			 // Use custom zigzag color
			int jumlah = 10;
			for (int i = 0; i < jumlah; i++)
			{
				float x1 = posisi.x + i * ukuranDinding.x / jumlah;
				float y1 = posisi.y + (i % 2 == 0 ? 0 : -20);
				
				float x2 = posisi.x + (i + 1) * ukuranDinding.x / jumlah;
				float y2 = posisi.y + ((i + 1) % 2 == 0 ? 0 : -20);
				
				_node.DrawLine(new Vector2(x1, y1), new Vector2(x2, y2), _zigzagColor, 3);
			}
		}
		else
		{
			// Draw outline zigzag
			// Create a zigzag pattern using points and draw them in white outline
			int jumlah = 10;
			List<Vector2> titikZigzag = new List<Vector2>();
			
			for (int i = 0; i <= jumlah; i++)
			{
				float x = posisi.x + i * ukuranDinding.x / jumlah;
				float y = posisi.y + (i % 2 == 0 ? 0 : -20); // 20 is the height of the zigzag
				titikZigzag.Add(new Vector2(x, y));
			}
			
			// Connect the zigzag points with lines
			for (int i = 0; i < titikZigzag.Count - 1; i++)
			{
				var linePoints = _bentuk.Garis(
					titikZigzag[i].x, titikZigzag[i].y, 
					titikZigzag[i+1].x, titikZigzag[i+1].y
				);
				
				foreach (var point in linePoints)
				{
					_node.DrawRect(new Rect2(point, new Vector2(1, 1)), Colors.White);
				}
			}
		}
	}

	// =============================
	// Bagian PINTU
	// =============================
	private void GambarPintu(bool isi)
	{
		Vector2 posisi = posisiDinding + _offset + new Vector2(75, 80);
		Vector2 ukuran = new Vector2(50, 70);
		if (isi)
			// Use custom pintu color
			_node.DrawRect(new Rect2(posisi, ukuran), _pintuColor);
		else
		{
			var titik = _bentuk.Persegi(posisi.x, posisi.y, ukuran.x, ukuran.y);
			foreach (var t in titik)
				_node.DrawRect(new Rect2(t, new Vector2(1, 1)), Colors.White);
		}
	}

	// =============================
	// Bagian ATAP
	// =============================
	private void GambarAtap(bool isi)
	{
		// Align the roof center with the wall top
		// The roof's center Y should be at the same height as the dinding's top position
		Vector2 center = new Vector2(100, posisiDinding.y) + _offset;
		
		if (isi)
			Primitif.GambarAtapBergaris(new Color(1.0f, 0.89f, 0.77f), center, _radiusAtap, _node);
		else
		{
			var titik = _bentuk.Lingkaran(center.x, center.y, _radiusAtap);
			foreach (var t in titik)
				if (t.y <= center.y) // hanya setengah atas
					_node.DrawRect(new Rect2(t, new Vector2(1, 1)), Colors.White);
		}
	}
	
	// New method based on Karya3 and Karya4 implementation
	private void GambarAtapBerwarna()
	{
		Vector2 center = new Vector2(100, posisiDinding.y) + _offset;
		
		// Draw filled semi-circle for roof
		List<Vector2> points = new List<Vector2>();
		int segments = 100;
		
		for (int i = 0; i <= segments; i++)
		{
			float angle = Mathf.Pi * i / segments;
			float x = center.x + Mathf.Cos(angle) * _radiusAtap;
			float y = center.y - Mathf.Sin(angle) * _radiusAtap;
			points.Add(new Vector2(x, y));
		}
		
		// Add center point to complete the polygon
		points.Add(center);
		
		// Draw roof with custom atap color
		_node.DrawPolygon(points.ToArray(), new Color[] { _atapColor });
		
		// Draw vertical pattern lines on roof
		int numLines = 20;
		for (int i = 0; i <= numLines; i++)
		{
			float x = center.x - _radiusAtap + i * (2 * _radiusAtap / numLines);
			float dx = x - center.x;
			
			if (Mathf.Abs(dx) <= _radiusAtap)
			{
				float yTop = center.y - Mathf.Sqrt(_radiusAtap * _radiusAtap - dx * dx);
				// Use a darker version of the atap color for the pattern lines
				Color lineColor = _atapColor.Darkened(0.4f);
				_node.DrawLine(new Vector2(x, center.y), new Vector2(x, yTop), lineColor, 1);
			}
		}
	}

	private void GambarMotifAtapOutline()
	{
		// Update motif position to match the new roof position
		Vector2 center = new Vector2(100, posisiDinding.y) + _offset;
		int jumlah = 20;

		for (int i = 0; i <= jumlah; i++)
		{
			float x = center.x - _radiusAtap + i * (2 * _radiusAtap / jumlah);
			float dx = x - center.x;

			// ✅ CEK agar tidak menyebabkan akar negatif!
			if (Mathf.Abs(dx) > _radiusAtap)
				continue;

			float radicand = _radiusAtap * _radiusAtap - dx * dx;

			// ✅ CEK agar tidak menyebabkan akar dari nilai negatif
			if (radicand < 0)
				continue;

			float yTop = center.y - Mathf.Sqrt(radicand);

			// ✅ CEK agar hasil yTop adalah angka yang valid
			if (float.IsNaN(yTop) || float.IsInfinity(yTop))
				continue;

			var garis = _bentuk.Garis(x, center.y, x, yTop);

			// ✅ CEK: batasi panjang garis
			if (garis.Count > 1000)
				continue;

			foreach (var p in garis)
				_node.DrawRect(new Rect2(p, new Vector2(1, 1)), Colors.White);
		}
	}

	// =============================
	// Bagian PAGAR
	// =============================
	private void GambarPagar(bool isi)
	{
		// Adjust pagar position based on new dinding position
		Vector2 posisi = posisiDinding + _offset + new Vector2(-40, 150); 
		Vector2 ukuran = new Vector2(280, 60);

		if (isi)
		{
			// Top border as rectangle - direct drawing with custom color
			_node.DrawRect(new Rect2(posisi, new Vector2(ukuran.x, 5)), _pagarColor);
			
			// Bottom border as rectangle - direct drawing with custom color
			_node.DrawRect(new Rect2(posisi + new Vector2(0, ukuran.y - 5), new Vector2(ukuran.x, 5)), _pagarColor);
			
			// Vertical posts - direct drawing with custom color
			int posts = 9;
			float postWidth = ukuran.x / 10;
			float spacing = ukuran.x / posts;
			
			for (int i = 0; i < posts; i++)
			{
				Vector2 postPos = posisi + new Vector2(i * spacing, 5);
				_node.DrawRect(new Rect2(postPos, new Vector2(postWidth, ukuran.y - 10)), _pagarColor.Darkened(0.3f));
			}
		}
		else
		{
			// Draw rectangle outlines for top and bottom borders
			var titikGarisAtas = _bentuk.Persegi(posisi.x, posisi.y, ukuran.x, 5);
			foreach (var t in titikGarisAtas)
				_node.DrawRect(new Rect2(t, new Vector2(1, 1)), Colors.White);
				
			var titikGarisBawah = _bentuk.Persegi(posisi.x, posisi.y + ukuran.y - 5, ukuran.x, 5);
			foreach (var t in titikGarisBawah)
				_node.DrawRect(new Rect2(t, new Vector2(1, 1)), Colors.White);

			// Tiang vertikal
			int jumlahTiang = 9;
			float jarak = ukuran.x / jumlahTiang;
			float lebarTiang = 15;

			for (int i = 0; i < jumlahTiang; i++)
			{
				Vector2 pos = posisi + new Vector2(i * jarak, 0);
				var garisTiang = _bentuk.Persegi(pos.x, pos.y, lebarTiang, ukuran.y);
				foreach (var t in garisTiang)
					_node.DrawRect(new Rect2(t, new Vector2(1, 1)), Colors.White);
			}
		}
	}
}
