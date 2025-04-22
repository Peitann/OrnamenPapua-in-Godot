using Godot;
using System.Collections.Generic;

public class TransformasiFast
{
	private Matrix3x3 matriks = Matrix3x3.Identity;

	public void Reset() => matriks = Matrix3x3.Identity;
	
	public void Translasi(float dx, float dy)
	{
		Matrix3x3 m = new Matrix3x3(new float[] {
			1, 0, dx,
			0, 1, dy,
			0, 0, 1
		});
		matriks *= m;
	}

	public void Scaling(float sx, float sy, Vector2 pivot)
	{
		Matrix3x3 m = new Matrix3x3(new float[] {
			sx, 0, pivot.x * (1 - sx),
			0, sy, pivot.y * (1 - sy),
			0, 0, 1
		});
		matriks *= m;
	}

	public void Rotasi(float derajat, Vector2 pivot)
	{
		float rad = Mathf.Deg2Rad(derajat);
		float cos = Mathf.Cos(rad);
		float sin = Mathf.Sin(rad);
		
		Matrix3x3 m = new Matrix3x3(new float[] {
			cos, -sin, pivot.x * (1 - cos) + pivot.y * sin,
			sin, cos, pivot.y * (1 - cos) - pivot.x * sin,
			0, 0, 1
		});
		matriks *= m;
	}

	public List<Vector2> Apply(List<Vector2> titik)
	{
		List<Vector2> hasil = new List<Vector2>();
		foreach (Vector2 t in titik)
		{
			Vector3 v = new Vector3(t.x, t.y, 1);
			Vector3 transformed = matriks * v;
			hasil.Add(new Vector2(transformed.x, transformed.y));
		}
		return hasil;
	}

	// Path and animation methods from Karya classes
	public Vector2 ApplySingle(Vector2 point)
	{
		Vector3 v = new Vector3(point.x, point.y, 1);
		Vector3 transformed = matriks * v;
		return new Vector2(transformed.x, transformed.y);
	}
	
	public static Vector2 GetTrianglePath(Vector2 center, float amplitude, float t)
	{
		// Triangle path - 3 segments
		int segment = (int)(t * 3); // Which segment (0, 1, or 2)
		float segmentT = (t * 3) % 1.0f; // Position within segment (0 to 1)
		
		// Define triangle vertices relative to center
		Vector2[] vertices = new Vector2[]
		{
			center + new Vector2(-amplitude, -amplitude/2), // Top left
			center + new Vector2(amplitude, -amplitude/2),  // Top right
			center + new Vector2(0, amplitude/2)            // Bottom
		};
		
		// Interpolate between vertices based on segment
		Vector2 start = vertices[segment];
		Vector2 end = vertices[(segment + 1) % 3];
		
		return start.LinearInterpolate(end, segmentT);
	}
	
	public static Vector2 GetCirclePath(Vector2 center, float radius, float t)
	{
		// Circle/O path using polar coordinates
		float angle = t * Mathf.Pi * 2;
		return center + new Vector2(
			radius * Mathf.Cos(angle),
			radius * Mathf.Sin(angle)
		);
	}
	
	// Follow behavior for objects
	public static Vector2 Follow(Vector2 current, Vector2 target, float speed, float delta)
	{
		Vector2 direction = target - current;
		float distance = direction.Length();
		
		if (distance < 5)
			return current;
			
		return current + direction.Normalized() * speed * delta;
	}
	
	// Apply transformation with scale around a pivot
	public void ScaleAroundPivot(float scale, Vector2 pivot)
	{
		// First translate pivot to origin
		Translasi(-pivot.x, -pivot.y);
		// Apply scaling
		Scaling(scale, scale, Vector2.Zero);
		// Translate back
		Translasi(pivot.x, pivot.y);
	}
}

// Helper class untuk operasi matriks 3x3
public class Matrix3x3
{
	private float[] data = new float[9];
	
	public static Matrix3x3 Identity => new Matrix3x3(new float[] {
		1,0,0,
		0,1,0,
		0,0,1
	});

	public Matrix3x3(float[] values) => data = values;
	
	public static Matrix3x3 operator *(Matrix3x3 a, Matrix3x3 b)
	{
		float[] result = new float[9];
		for (int row = 0; row < 3; row++)
		{
			for (int col = 0; col < 3; col++)
			{
				result[row * 3 + col] = 
					a.data[row * 3 + 0] * b.data[0 * 3 + col] +
					a.data[row * 3 + 1] * b.data[1 * 3 + col] +
					a.data[row * 3 + 2] * b.data[2 * 3 + col];
			}
		}
		return new Matrix3x3(result);
	}

	public static Vector3 operator *(Matrix3x3 m, Vector3 v)
	{
		return new Vector3(
			m.data[0] * v.x + m.data[1] * v.y + m.data[2] * v.z,
			m.data[3] * v.x + m.data[4] * v.y + m.data[5] * v.z,
			m.data[6] * v.x + m.data[7] * v.y + m.data[8] * v.z
		);
	}
}
