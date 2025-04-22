using Godot;
using System.Collections.Generic;

public class Transformasi
{
	public static List<Vector2> Translasi(List<Vector2> titik, float dx, float dy)
	{
		List<Vector2> hasil = new List<Vector2>();
		foreach (Vector2 t in titik)
		{
			hasil.Add(new Vector2(t.x + dx, t.y + dy));
		}
		return hasil;
	}

	public static List<Vector2> Scaling(List<Vector2> titik, float sx, float sy, Vector2 pivot)
	{
		List<Vector2> hasil = new List<Vector2>();
		foreach (Vector2 t in titik)
		{
			float x = (t.x - pivot.x) * sx + pivot.x;
			float y = (t.y - pivot.y) * sy + pivot.y;
			hasil.Add(new Vector2(x, y));
		}
		return hasil;
	}

	public static List<Vector2> Rotasi(List<Vector2> titik, float derajat, Vector2 pivot)
	{
		List<Vector2> hasil = new List<Vector2>();
		float rad = Mathf.Deg2Rad(derajat);
		float cos = Mathf.Cos(rad);
		float sin = Mathf.Sin(rad);
		
		foreach (Vector2 t in titik)
		{
			float x = t.x - pivot.x;
			float y = t.y - pivot.y;
			
			float xBaru = x * cos - y * sin + pivot.x;
			float yBaru = x * sin + y * cos + pivot.y;
			
			hasil.Add(new Vector2(xBaru, yBaru));
		}
		return hasil;
	}

	// Path interpolation methods from Karya classes
	public static Vector2 LinearInterpolate(Vector2 start, Vector2 end, float t)
	{
		return start + (end - start) * t;
	}
	
	public static Vector2 GetTrianglePath(Vector2[] vertices, float t)
	{
		// Triangle path - 3 segments
		int segment = (int)(t * 3); // Which segment (0, 1, or 2)
		float segmentT = (t * 3) % 1.0f; // Position within segment (0 to 1)
		
		// Interpolate between vertices based on segment
		Vector2 start = vertices[segment];
		Vector2 end = vertices[(segment + 1) % 3];
		
		return LinearInterpolate(start, end, segmentT);
	}
	
	public static Vector2 GetCirclePath(float t, float radius)
	{
		// Circle path using polar coordinates
		float angle = t * Mathf.Pi * 2;
		return new Vector2(
			radius * Mathf.Cos(angle),
			radius * Mathf.Sin(angle)
		);
	}
	
	public static Vector2 GetTrapeziumPath(Vector2[] vertices, float t)
	{
		// Trapezium path - 4 segments
		int segment = (int)(t * 4); // Which segment (0, 1, 2, or 3)
		float segmentT = (t * 4) % 1.0f; // Position within segment (0 to 1)
		
		// Interpolate between vertices based on segment
		Vector2 start = vertices[segment];
		Vector2 end = vertices[(segment + 1) % 4];
		
		return LinearInterpolate(start, end, segmentT);
	}
	
	// Movement methods
	public static Vector2 MoveTowards(Vector2 current, Vector2 target, float maxDelta)
	{
		Vector2 direction = target - current;
		float distance = direction.Length();
		
		if (distance <= maxDelta || distance == 0)
			return target;
			
		return current + direction.Normalized() * maxDelta;
	}
	
	// Scale around a point
	public static Vector2 ScaleAround(Vector2 point, Vector2 pivot, float scale)
	{
		return pivot + (point - pivot) * scale;
	}
}
