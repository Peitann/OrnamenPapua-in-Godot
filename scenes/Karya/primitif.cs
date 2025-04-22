using Godot;
using System;
using System.Collections.Generic;

public class Primitif : Reference // Mengganti RefCounted dengan Reference untuk Godot 3.x
{

	public List<Vector2> LineDDA(float xa, float ya, float xb, float yb)
	{
		float dx = xb - xa;
		float dy = yb - ya;
		float steps = Math.Max(Mathf.Abs(dx), Mathf.Abs(dy));
		float xIncrement = dx / steps;
		float yIncrement = dy / steps;
		float x = xa;
		float y = ya;

		List<Vector2> res = new List<Vector2> { new Vector2(Mathf.Round(x), Mathf.Round(y)) };

		for (int k = 0; k < steps; k++)
		{
			x += xIncrement;
			y += yIncrement;
			res.Add(new Vector2(Mathf.Round(x), Mathf.Round(y)));
		}

		return res;
	}

	public List<Vector2> LineBresenham(float xa, float ya, float xb, float yb)
	{
		List<Vector2> res = new List<Vector2>();
		int x1 = (int)xa;
		int y1 = (int)ya;
		int x2 = (int)xb;
		int y2 = (int)yb;

		int dx = Math.Abs(x2 - x1);
		int dy = Math.Abs(y2 - y1);
		int sx = (x1 < x2) ? 1 : -1;
		int sy = (y1 < y2) ? 1 : -1;
		int err = dx - dy;

		while (true)
		{
			res.Add(new Vector2(x1, y1));
			if (x1 == x2 && y1 == y2) break;
			int e2 = 2 * err;
			if (e2 > -dy) { err -= dy; x1 += sx; }
			if (e2 < dx) { err += dx; y1 += sy; }
		}
		return res;
	}

	public float[] ConvertToKartesian(float xa, float ya, float xb, float yb, float marginLeft, float marginTop)
	{
		float axis = (float)Math.Ceiling(OS.WindowSize.x / 2.0f);
		float ordinat = (float)Math.Ceiling(OS.WindowSize.y / 2.0f);

		xa = axis + xa;
		xb = axis + xb;
		ya = ordinat - ya;
		yb = ordinat - yb;

		return new float[] { xa, ya, xb, yb };
	}

	public float[] ConvertToPixel(float xa, float ya, float xb, float yb, float marginLeft, float marginTop)
	{
		xa = marginLeft + xa;
		xb = marginLeft + xb;
		ya = OS.WindowSize.y - marginTop - ya;
		yb = OS.WindowSize.y - marginTop - yb;

		return new float[] { xa, ya, xb, yb };
	}
	public List<Vector2> CircleMidpoint(float xc, float yc, float r)
	{
		List<Vector2> res = new List<Vector2>();
		int x = 0;
		int y = (int)r;
		int P = 1 - (int)r; // Decision parameter awal

		while (x <= y)
		{
			// Menambahkan semua titik simetris dari lingkaran
			res.Add(new Vector2(xc + x, yc + y));
			res.Add(new Vector2(xc - x, yc + y));
			res.Add(new Vector2(xc + x, yc - y));
			res.Add(new Vector2(xc - x, yc - y));
			res.Add(new Vector2(xc + y, yc + x));
			res.Add(new Vector2(xc - y, yc + x));
			res.Add(new Vector2(xc + y, yc - x));
			res.Add(new Vector2(xc - y, yc - x));

			// Update decision parameter
			if (P < 0)
			{
				// Titik berikutnya di dalam lingkaran
				P += 2 * x + 3;
			}
			else
			{
				// Titik berikutnya di luar lingkaran
				P += 2 * (x - y) + 5;
				y--;
			}

			x++;
		}

		return res;
	}
	public List<Vector2> EllipseMidpoint(float xc, float yc, float rx, float ry)
	{
		List<Vector2> res = new List<Vector2>();
		float rx2 = rx * rx;
		float ry2 = ry * ry;
		float tworx2 = 2 * rx2;
		float twory2 = 2 * ry2;
		float x = 0, y = ry;
		float px = 0, py = tworx2 * y;

		// Region 1
		float p1 = ry2 - (rx2 * ry) + (0.25f * rx2);
		while (px < py)
		{
			res.Add(new Vector2(xc + x, yc + y));
			res.Add(new Vector2(xc - x, yc + y));
			res.Add(new Vector2(xc + x, yc - y));
			res.Add(new Vector2(xc - x, yc - y));

			x++;
			px += twory2;
			if (p1 < 0)
				p1 += ry2 + px;
			else
			{
				y--;
				py -= tworx2;
				p1 += ry2 + px - py;
			}
		}

		// Region 2
		float p2 = ry2 * (x + 0.5f) * (x + 0.5f) + rx2 * (y - 1) * (y - 1) - rx2 * ry2;
		while (y >= 0)
		{
			res.Add(new Vector2(xc + x, yc + y));
			res.Add(new Vector2(xc - x, yc + y));
			res.Add(new Vector2(xc + x, yc - y));
			res.Add(new Vector2(xc - x, yc - y));

			y--;
			py -= tworx2;
			if (p2 > 0)
				p2 += rx2 - py;
			else
			{
				x++;
				px += twory2;
				p2 += rx2 - py + px;
			}
		}

		return res;
	}

	public List<Vector2> DottedLine(float x0, float y0, float x1, float y1)
	{
		List<Vector2> points = new List<Vector2>();
		int dx = Math.Abs((int)(x1 - x0));
		int dy = Math.Abs((int)(y1 - y0));
		int sx = x0 < x1 ? 1 : -1;
		int sy = y0 < y1 ? 1 : -1;
		int err = dx - dy;

		int patternIndex = 0; // Untuk mengatur pola titik-titik

		while (true)
		{
			if (patternIndex % 4 == 0) // Gambar setiap 4 piksel
			{
				points.Add(new Vector2(x0, y0));
			}

			if (x0 == x1 && y0 == y1) break;

			int e2 = 2 * err;
			if (e2 > -dy)
			{
				err -= dy;
				x0 += sx;
			}
			if (e2 < dx)
			{
				err += dx;
				y0 += sy;
			}

			patternIndex++;
		}

		return points;
	}

	public List<Vector2> DashedLine(float x0, float y0, float x1, float y1)
	{
		List<Vector2> points = new List<Vector2>();
		int dx = Math.Abs((int)(x1 - x0));
		int dy = Math.Abs((int)(y1 - y0));
		int sx = x0 < x1 ? 1 : -1;
		int sy = y0 < y1 ? 1 : -1;
		int err = dx - dy;

		int patternIndex = 0; // Untuk mengatur pola putus-putus

		while (true)
		{
			if (patternIndex % 8 < 5) // Gambar 5 piksel, lewati 3 piksel
			{
				points.Add(new Vector2(x0, y0));
			}

			if (x0 == x1 && y0 == y1) break;

			int e2 = 2 * err;
			if (e2 > -dy)
			{
				err -= dy;
				x0 += sx;
			}
			if (e2 < dx)
			{
				err += dx;
				y0 += sy;
			}

			patternIndex++;
		}

		return points;
	}

	public List<Vector2> DotDashLine(float x0, float y0, float x1, float y1)
	{
		List<Vector2> points = new List<Vector2>();
		int dx = Math.Abs((int)(x1 - x0));
		int dy = Math.Abs((int)(y1 - y0));
		int sx = x0 < x1 ? 1 : -1;
		int sy = y0 < y1 ? 1 : -1;
		int err = dx - dy;

		int patternIndex = 0; // Untuk mengatur pola titik-garis

		while (true)
		{
			if (patternIndex % 12 < 2 || (patternIndex % 12 >= 4 && patternIndex % 12 < 7)) // Titik-garis
			{
				points.Add(new Vector2(x0, y0));
			}

			if (x0 == x1 && y0 == y1) break;

			int e2 = 2 * err;
			if (e2 > -dy)
			{
				err -= dy;
				x0 += sx;
			}
			if (e2 < dx)
			{
				err += dx;
				y0 += sy;
			}

			patternIndex++;
		}

		return points;
	}
	
	public static void GambarAtapBergaris(Color color, Vector2 center, float radius, Node2D node)
	{
		// Gambar setengah lingkaran atap
		BentukDasar.GambarSetengahLingkaran(color, center, radius, node, 120); // segmen 120, atap halus

		// Gambar garis-garis vertikal sebagai motif yang mengikuti lengkung setengah lingkaran
		int jumlahGaris = 20;
		for (int i = 0; i <= jumlahGaris; i++)
		{
			float x = center.x - radius + i * (2 * radius / jumlahGaris);

			// Hitung posisi Y bagian atas dari lengkungan setengah lingkaran
			float deltaX = x - center.x;
			if (Mathf.Abs(deltaX) <= radius)
			{
				float yTop = center.y - Mathf.Sqrt(radius * radius - deltaX * deltaX); // mengikuti kontur atap

				var garis = new Line2D();
				garis.Width = 1;
				garis.DefaultColor = Colors.DarkRed;
				garis.AddPoint(new Vector2(x, center.y));   // titik bawah (tengah bawah setengah lingkaran)
				garis.AddPoint(new Vector2(x, yTop));       // titik atas mengikuti kontur lengkung
				node.AddChild(garis);
			}
		}
	}


	public static void GambarPagar(Vector2 posisi, Vector2 ukuran, Color warnaBingkai, Color warnaTiang, Node2D node)
	{
		float tiangLebar = ukuran.x / 10;
		float tiangTinggi = ukuran.y * 0.6f;
		float jarak = ukuran.x / 9;

		// Atas dan bawah pagar
		BentukDasar.GambarPersegi(warnaBingkai, posisi, new Vector2(ukuran.x, tiangLebar / 2), node); // atas
		BentukDasar.GambarPersegi(warnaBingkai, posisi + new Vector2(0, ukuran.y - tiangLebar / 2), new Vector2(ukuran.x, tiangLebar / 2), node); // bawah

		// Tiang-tiang pagar
		for (int i = 0; i < 9; i++)
		{
			Vector2 tiangPos = posisi + new Vector2(i * jarak, tiangLebar / 2);
			BentukDasar.GambarPersegi(warnaTiang, tiangPos, new Vector2(tiangLebar, tiangTinggi), node);
		}
	}
}
