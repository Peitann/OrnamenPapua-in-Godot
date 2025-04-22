using Godot;
using System;
using System.Collections.Generic;

public class BentukDasar : Node2D, IDisposable
{
	internal Primitif _primitif = new Primitif();
	private bool _disposed = false;

	public override void _Ready()
	{
		// Siapkan jika ada kebutuhan tambahan saat scene siap
	}
	
	 public static void GambarPersegi(Color color, Vector2 posisi, Vector2 ukuran, Node2D node)
	{
		var rect = new ColorRect();
		rect.Color = color;
		rect.RectPosition = posisi;
		rect.RectSize = ukuran;
		node.AddChild(rect);
	}

	public static void GambarSetengahLingkaran(Color color, Vector2 center, float radius, Node2D node, int segmentCount = 100, bool menghadapKeBawah = false)
	{
		List<Vector2> points = new List<Vector2>();

		// Gambar setengah lingkaran (atas atau bawah tergantung flag)
		for (int i = 0; i <= segmentCount; i++)
		{
			// Sudut dalam radian dari 0 ke π (setengah lingkaran)
			float angle = Mathf.Pi * i / segmentCount;

			// Jika menghadap ke bawah, kita geser sin menjadi negatif
			float x = center.x + Mathf.Cos(angle) * radius;
			float y = center.y + (menghadapKeBawah ? Mathf.Sin(angle) : -Mathf.Sin(angle)) * radius;

			points.Add(new Vector2(x, y));
		}

		// Tambahkan titik pusat untuk membuat polygon tertutup (bukan hanya garis)
		points.Add(center);

		// Gambar polygon dari array titik
		var polygon = new Polygon2D();
		polygon.Polygon = points.ToArray();
		polygon.Color = color;
		node.AddChild(polygon);
	}


	public static void GambarZigZag(Color color, Vector2 mulai, float panjang, float tinggi, int jumlah, Node2D node)
	{
		var zigzag = new Line2D();
		zigzag.Width = 3;
		zigzag.DefaultColor = color;
		for (int i = 0; i <= jumlah; i++)
		{
			float x = mulai.x + i * panjang / jumlah;
			float y = mulai.y + (i % 2 == 0 ? 0 : -tinggi);
			zigzag.AddPoint(new Vector2(x, y));
		}
		node.AddChild(zigzag);
	}
	
	public Primitif PrimitifInstance
	{
		get { return _primitif; }
	}
	
	public List<Vector2> Garis(float x1, float y1, float x2, float y2)
	{
		List<Vector2> hasil = new List<Vector2>();

		float dx = x2 - x1;
		float dy = y2 - y1;

		int steps = (int)Math.Max(Math.Abs(dx), Math.Abs(dy));
		if (steps == 0)
		{
			hasil.Add(new Vector2(x1, y1));
			return hasil;
		}

		float xInc = dx / steps;
		float yInc = dy / steps;

		float x = x1;
		float y = y1;

		for (int i = 0; i <= steps; i++)
		{
			hasil.Add(new Vector2(Mathf.Round(x), Mathf.Round(y)));
			x += xInc;
			y += yInc;

			// 🔒 Amankan agar tidak terlalu panjang
			if (hasil.Count > 2000)
				break;
		}

		return hasil;
	}

	
	public void FloodFill(Image img, Vector2 start, Color fillColor, Color borderColor)
	{
		Queue<Vector2> queue = new Queue<Vector2>();
		queue.Enqueue(start);

		Color targetColor = img.GetPixel((int)start.x, (int)start.y);
		if (targetColor == fillColor || targetColor == borderColor)
			return;

		while (queue.Count > 0)
		{
			Vector2 p = queue.Dequeue();
			int x = (int)p.x;
			int y = (int)p.y;

			if (x < 0 || x >= img.GetWidth() || y < 0 || y >= img.GetHeight())
				continue;

			Color currentColor = img.GetPixel(x, y);
			if (currentColor != targetColor)
				continue;

			img.SetPixel(x, y, fillColor);

			queue.Enqueue(new Vector2(x + 1, y));
			queue.Enqueue(new Vector2(x - 1, y));
			queue.Enqueue(new Vector2(x, y + 1));
			queue.Enqueue(new Vector2(x, y - 1));
		}
	}


	public List<Vector2> Margin(int marginLeft, int marginTop, int marginRight, int marginBottom)
	{
		if (_primitif == null)
		{
			GD.PrintErr("Node Primitif belum di-assign!");
			return new List<Vector2>();
		}

		List<Vector2> res = new List<Vector2>();
		res.AddRange(_primitif.LineDDA(marginLeft, marginTop, marginRight, marginTop));
		res.AddRange(_primitif.LineDDA(marginLeft, marginBottom, marginRight, marginBottom));
		res.AddRange(_primitif.LineDDA(marginLeft, marginTop, marginLeft, marginBottom));
		res.AddRange(_primitif.LineDDA(marginRight, marginTop, marginRight, marginBottom));

		return res;
	}

	public List<Vector2> Persegi(float x, float y, float width, float height)
	{
		List<Vector2> garis1 = _primitif.LineBresenham(x, y, x + width, y);
		List<Vector2> garis2 = _primitif.LineBresenham(x + width, y, x + width, y + height);
		List<Vector2> garis3 = _primitif.LineBresenham(x + width, y + height, x, y + height);
		List<Vector2> garis4 = _primitif.LineBresenham(x, y + height, x, y);

		List<Vector2> persegi = new List<Vector2>();
		persegi.AddRange(garis1);
		persegi.AddRange(garis2.GetRange(1, garis2.Count - 1));
		persegi.AddRange(garis3.GetRange(1, garis3.Count - 1));
		persegi.AddRange(garis4.GetRange(1, garis4.Count - 1));

		return persegi;
	}


	public List<Vector2> PersegiPanjang(float x, float y, float panjang, float lebar)
	{
		if (_primitif == null)
		{
			GD.PrintErr("Node Primitif belum di-assign!");
			return new List<Vector2>();
		}

		List<Vector2> garis1 = _primitif.LineBresenham(x, y, x + panjang, y);
		List<Vector2> garis2 = _primitif.LineBresenham(x + panjang, y, x + panjang, y + lebar);
		List<Vector2> garis3 = _primitif.LineBresenham(x + panjang, y + lebar, x, y + lebar);
		List<Vector2> garis4 = _primitif.LineBresenham(x, y + lebar, x, y);

		List<Vector2> persegiPanjang = new List<Vector2>();
		persegiPanjang.AddRange(garis1);
		persegiPanjang.AddRange(garis2.GetRange(1, garis2.Count - 1));
		persegiPanjang.AddRange(garis3.GetRange(1, garis3.Count - 1));
		persegiPanjang.AddRange(garis4.GetRange(1, garis4.Count - 1));

		return persegiPanjang;
	}

	// Konversi dari koordinat pixel ke koordinat kartesian
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

	// Konversi dari koordinat dunia ke koordinat pixel
	public float[] ConvertToPixel(float xa, float ya, float xb, float yb, float marginLeft, float marginTop)
	{
		xa = marginLeft + xa;
		xb = marginLeft + xb;
		ya = OS.WindowSize.y - marginTop - ya;
		yb = OS.WindowSize.y - marginTop - yb;

		return new float[] { xa, ya, xb, yb };
	}
	public List<Vector2> SegitigaSiku(float x, float y, float alas, float tinggi)
	{
		List<Vector2> garis1 = _primitif.LineBresenham(x, y, x + alas, y);
		List<Vector2> garis2 = _primitif.LineBresenham(x, y, x, y - tinggi);
		List<Vector2> garis3 = _primitif.LineBresenham(x, y - tinggi, x + alas, y);
		
		List<Vector2> segitiga = new List<Vector2>();
		segitiga.AddRange(garis1);
		segitiga.AddRange(garis2.GetRange(1, garis2.Count - 1));
		segitiga.AddRange(garis3.GetRange(1, garis3.Count - 1));
		return segitiga;
	}

	public List<Vector2> TrapesiumSiku(float x, float y, float atas, float bawah, float tinggi)
	{
		List<Vector2> garis1 = _primitif.LineBresenham(x, y, x + bawah, y);
		List<Vector2> garis2 = _primitif.LineBresenham(x, y, x + (bawah - atas), y - tinggi);
		List<Vector2> garis3 = _primitif.LineBresenham(x + (bawah - atas), y - tinggi, x + bawah, y - tinggi);
		List<Vector2> garis4 = _primitif.LineBresenham(x + bawah, y - tinggi, x + bawah, y);
		
		List<Vector2> trapesium = new List<Vector2>();
		trapesium.AddRange(garis1);
		trapesium.AddRange(garis2);
		trapesium.AddRange(garis3);
		trapesium.AddRange(garis4);
		return trapesium;
	}

	public List<Vector2> SegitigaSamaKaki(float x, float y, float alas, float tinggi)
	{
		float tengah = x + (alas / 2);
		List<Vector2> garis1 = _primitif.LineBresenham(x, y, x + alas, y);
		List<Vector2> garis2 = _primitif.LineBresenham(x, y, tengah, y - tinggi);
		List<Vector2> garis3 = _primitif.LineBresenham(x + alas, y, tengah, y - tinggi);
		
		List<Vector2> segitiga = new List<Vector2>();
		segitiga.AddRange(garis1);
		segitiga.AddRange(garis2.GetRange(1, garis2.Count - 1));
		segitiga.AddRange(garis3.GetRange(1, garis3.Count - 1));
		return segitiga;
	}

	public List<Vector2> TrapesiumSamaKaki(float x, float y, float atas, float bawah, float tinggi)
	{
		float selisih = (bawah - atas) / 2;
		List<Vector2> garis1 = _primitif.LineBresenham(x, y, x + bawah, y);
		List<Vector2> garis2 = _primitif.LineBresenham(x + selisih, y - tinggi, x + selisih + atas, y - tinggi);
		List<Vector2> garis3 = _primitif.LineBresenham(x + selisih, y - tinggi, x, y);
		List<Vector2> garis4 = _primitif.LineBresenham(x + selisih + atas, y - tinggi, x + bawah, y);
		
		List<Vector2> trapesium = new List<Vector2>();
		trapesium.AddRange(garis1);
		trapesium.AddRange(garis2);
		trapesium.AddRange(garis3);
		trapesium.AddRange(garis4);
		return trapesium;
	}

	 public List<Vector2> Lingkaran(float x, float y, float radius)
	{
		return _primitif.CircleMidpoint(x, y, radius);
	}

	public List<Vector2> Elips(float x, float y, float rx, float ry)
	{
		return _primitif.EllipseMidpoint(x, y, rx, ry);
	}
	
	
	public List<Vector2> Jajargenjang(float x, float y, float alas, float tinggi, float shear)
	{
		if (_primitif == null)
		{
			GD.PrintErr("Node Primitif belum di-assign!");
			return new List<Vector2>();
		}

		Vector2 kiriBawah = new Vector2(x, y);
		Vector2 kananBawah = new Vector2(x + alas + shear, y);
		Vector2 kiriAtas = new Vector2(x + shear, y - tinggi);
		Vector2 kananAtas = new Vector2(x + alas + shear + shear, y - tinggi);

		List<Vector2> garis1 = _primitif.LineBresenham(kiriBawah.x, kiriBawah.y, kananBawah.x, kananBawah.y);
		List<Vector2> garis2 = _primitif.LineBresenham(kananBawah.x, kananBawah.y, kananAtas.x, kananAtas.y);
		List<Vector2> garis3 = _primitif.LineBresenham(kananAtas.x, kananAtas.y, kiriAtas.x, kiriAtas.y);
		List<Vector2> garis4 = _primitif.LineBresenham(kiriAtas.x, kiriAtas.y, kiriBawah.x, kiriBawah.y);

		List<Vector2> jajargenjang = new List<Vector2>();
		jajargenjang.AddRange(garis1);
		jajargenjang.AddRange(garis2.GetRange(1, garis2.Count - 1));
		jajargenjang.AddRange(garis3.GetRange(1, garis3.Count - 1));
		jajargenjang.AddRange(garis4.GetRange(1, garis4.Count - 1));

		return jajargenjang;
	}

	public List<Vector2> Poligon(float x, float y, int jumlahSisi, float radius)
	{
		if (_primitif == null || jumlahSisi < 3)
		{
			GD.PrintErr("Parameter tidak valid!");
			return new List<Vector2>();
		}

		List<Vector2> titik = new List<Vector2>();
		List<Vector2> poligon = new List<Vector2>();

		// Hitung titik sudut
		for (int i = 0; i < jumlahSisi; i++)
		{
			float sudut = Mathf.Deg2Rad(i * 360f / jumlahSisi);
			float px = x + radius * Mathf.Cos(sudut);
			float py = y + radius * Mathf.Sin(sudut);
			titik.Add(new Vector2(px, py));
		}

		// Hubungkan titik dengan garis
		for (int i = 0; i < jumlahSisi; i++)
		{
			Vector2 awal = titik[i];
			Vector2 akhir = titik[(i + 1) % jumlahSisi];
			List<Vector2> garis = _primitif.LineBresenham(awal.x, awal.y, akhir.x, akhir.y);
			poligon.AddRange(i == 0 ? garis : garis.GetRange(1, garis.Count - 1));
		}

		return poligon;
	}


	public void Dispose()
	{
		if (!_disposed)
		{
			GD.Print($"_primitif is null in Dispose(): {_primitif == null}");
			_primitif?.Dispose();
			_primitif = null;
			_disposed = true;
		}
	}
}
