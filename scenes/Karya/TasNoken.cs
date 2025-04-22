using Godot;
using System;
using System.Collections.Generic;

public class TasNoken
{
	private Node2D _node;
	private Vector2 _offset;
	private BentukDasar _bentuk = new BentukDasar();
	private float _scale = 1.0f;
	private bool _isOpen = false;
	
	// Default colors
	private Color _badanColor = new Color(0.6f, 0.3f, 0.1f);
	private Color _taliColor = new Color(0.4f, 0.2f, 0.1f);
	private Color _motifColor = Colors.DarkRed;
	private Color _ornamenColor = new Color(1f, 0.7f, 0.2f);

	public TasNoken(Node2D node, Vector2 offset)
	{
		_node = node;
		_offset = offset;
	}
	
	// Additional constructor with scale parameter
	public TasNoken(Node2D node, Vector2 offset, float scale, bool isOpen = false)
	{
		_node = node;
		_offset = offset;
		_scale = scale;
		_isOpen = isOpen;
	}

	// Method to set custom colors
	public void SetColors(Color badanColor, Color taliColor, Color motifColor, Color ornamenColor)
	{
		_badanColor = badanColor;
		_taliColor = taliColor;
		_motifColor = motifColor;
		_ornamenColor = ornamenColor;
	}

	public void GambarOutline()
	{
		GambarTrapesium(false);
		GambarTali(false);
		GambarMotifGaris(false);
		GambarOrnamen(false);
	}

	public void GambarBerwarna()
	{
		GambarTrapesium(true);
		GambarTali(true);
		GambarMotifGaris(true);
		GambarOrnamen(true);
	}

	// ======================
	// BADAN TAS (TRAPESIUM)
	// ======================
	private void GambarTrapesium(bool isi)
	{
		// Using the trapezium points from Karya4
		Vector2[] trapesium = new Vector2[]
		{
			_offset + new Vector2(-30 * _scale, 0),
			_offset + new Vector2(30 * _scale, 0),
			_offset + new Vector2(40 * _scale, 60 * _scale),
			_offset + new Vector2(-40 * _scale, 60 * _scale)
		};

		if (isi)
		{
			if (_isOpen)
			{
				// Open bag - draw only sides and bottom
				_node.DrawLine(trapesium[2], trapesium[3], _badanColor, 2); // Bottom edge
				_node.DrawLine(trapesium[0], trapesium[3], _badanColor, 2); // Left edge
				_node.DrawLine(trapesium[1], trapesium[2], _badanColor, 2); // Right edge
				
				// Fill with lighter color of the custom badan color
				Color fillColor = _badanColor.Lightened(0.2f);
				fillColor.a = 0.7f; // Set transparency
				_node.DrawPolygon(trapesium, new Color[] { fillColor });
			}
			else
			{
				// Closed bag - draw full trapezium with custom badan color
				_node.DrawPolygon(trapesium, new Color[] { _badanColor });
			}
		}
		else
		{
			for (int i = 0; i < trapesium.Length; i++)
			{
				var garis = _bentuk.Garis(trapesium[i].x, trapesium[i].y, trapesium[(i + 1) % trapesium.Length].x, trapesium[(i + 1) % trapesium.Length].y);
				foreach (var t in garis)
					_node.DrawRect(new Rect2(t, new Vector2(1, 1)), Colors.White);
			}
		}
	}

	// ======================
	// TALI TAS (Lengkung U)
	// ======================
	private void GambarTali(bool isi)
	{
		// Based on Karya4 implementation with custom tali color
		_node.DrawLine(
			_offset + new Vector2(-30 * _scale, 0), 
			_offset + new Vector2(0, -20 * _scale), 
			_taliColor, 
			isi ? 3 : 1
		);
		
		_node.DrawLine(
			_offset + new Vector2(0, -20 * _scale), 
			_offset + new Vector2(30 * _scale, 0), 
			_taliColor, 
			isi ? 3 : 1
		);
	}

	// ======================
	// MOTIF GARIS DI BADAN
	// ======================
	private void GambarMotifGaris(bool isi)
	{
		// Based on Karya4 implementation - horizontal strips with custom motif color
		float[] motifYOffsets = { 20 * _scale, 40 * _scale, 50 * _scale };
		foreach (float yOffset in motifYOffsets)
		{
			float t = yOffset / (60 * _scale);
			float width = 60 * _scale + (80 * _scale - 60 * _scale) * t;
			
			Vector2 posMotif = _offset + new Vector2(-width/2, yOffset);
			Vector2 sizeMotif = new Vector2(width, 5 * _scale);
			
			if (isi)
				_node.DrawRect(new Rect2(posMotif, sizeMotif), _motifColor);
			else
			{
				var titikMotif = _bentuk.Persegi(
					posMotif.x, 
					posMotif.y, 
					sizeMotif.x, 
					sizeMotif.y
				);
				
				foreach (var point in titikMotif)
					_node.DrawRect(new Rect2(point, new Vector2(1, 1)), Colors.White);
			}
		}
	}

	// ======================
	// ORNAMEN BULAT DI TENGAH
	// ======================
	private void GambarOrnamen(bool isi)
	{
		// Based on Karya4 implementation with custom ornamen color
		Vector2 pusat = _offset + new Vector2(0, 30 * _scale);
		float radius = 10 * _scale;

		if (isi)
		{
			_node.DrawCircle(pusat, radius, _ornamenColor);
		}
		else
		{
			var titik = _bentuk.Lingkaran(pusat.x, pusat.y, radius);
			foreach (var t in titik)
				_node.DrawRect(new Rect2(t, new Vector2(1, 1)), Colors.White);
		}
	}
}
