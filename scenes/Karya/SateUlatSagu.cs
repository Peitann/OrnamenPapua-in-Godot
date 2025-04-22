using Godot;
using System;
using System.Collections.Generic;

public class SateUlatSagu
{
	private Node2D _node;
	private Vector2 _offset;
	private BentukDasar _bentuk = new BentukDasar();
	private int _grubsPerSkewer = 5; // Default number of grubs
	
	// Default colors
	private Color _tusukColor = new Color(0.36f, 0.25f, 0.2f);
	private Color _ulatColor = new Color(1.0f, 0.9f, 0.8f);
	private Color _detailColor = new Color(0.3f, 0.15f, 0.05f);

	public SateUlatSagu(Node2D node, Vector2 offset)
	{
		_node = node;
		_offset = offset;
	}
	
	// Constructor with custom number of grubs
	public SateUlatSagu(Node2D node, Vector2 offset, int grubCount)
	{
		_node = node;
		_offset = offset;
		_grubsPerSkewer = grubCount;
	}

	// Method to set custom colors
	public void SetColors(Color tusukColor, Color ulatColor, Color detailColor)
	{
		_tusukColor = tusukColor;
		_ulatColor = ulatColor;
		_detailColor = detailColor;
	}

	public void GambarOutline()
	{
		GambarTusuk(false);
		GambarUlat(false);
	}

	public void GambarBerwarna()
	{
		GambarTusuk(true);
		GambarUlat(true);
	}

	// ====================
	// Tusuk Sate (Persegi panjang vertikal)
	// ====================
	private void GambarTusuk(bool isi)
	{
		 // Using same dimensions as in Karya4
		Vector2 ukuranTusuk = new Vector2(8, 160);
		Vector2 posisi = _offset - ukuranTusuk/2;

		if (isi)
			// Use custom tusuk color
			_node.DrawRect(new Rect2(posisi, ukuranTusuk), _tusukColor);
		else
		{
			var titik = _bentuk.Persegi(posisi.x, posisi.y, ukuranTusuk.x, ukuranTusuk.y);
			foreach (var t in titik)
				_node.DrawRect(new Rect2(t, new Vector2(1, 1)), Colors.White);
		}
	}

	// ====================
	// Ulat Sagu (Elips + Lingkaran per segmen)
	// ====================
	private void GambarUlat(bool isi)
	{
		// Draw ulat sagu based on Karya4 implementation
		Vector2 ukuranTusuk = new Vector2(8, 160);
		
		for (int i = 0; i < _grubsPerSkewer; i++)
		{
			float spacing = ukuranTusuk.y / (_grubsPerSkewer + 1);
			float yOffset = i * spacing + spacing/2;
			Vector2 pusatUlat = _offset + new Vector2(0, -ukuranTusuk.y/2 + yOffset);
			
			if (isi)
			{
				// Draw grub body with custom ulat color
				GambarElips(pusatUlat, 18, 12, _ulatColor);
				
				// Draw burnt pattern with custom detail color
				GambarElips(pusatUlat + new Vector2(6, 0), 4, 4, _detailColor);
			}
			else
			{
				var titikElips = _bentuk.Elips(pusatUlat.x, pusatUlat.y, 18, 12);
				foreach (var t in titikElips)
					_node.DrawRect(new Rect2(t, new Vector2(1, 1)), Colors.White);
					
				var titikDetail = _bentuk.Elips(pusatUlat.x + 6, pusatUlat.y, 4, 4);
				foreach (var t in titikDetail)
					_node.DrawRect(new Rect2(t, new Vector2(1, 1)), Colors.White);
			}
		}
	}
	
	// Helper method to draw filled ellipse
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

		_node.DrawPolygon(titik.ToArray(), new Color[] { warna });
	}
}
