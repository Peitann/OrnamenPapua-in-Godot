using Godot;
using System.Collections.Generic;

/// <summary>
/// Helper class for advanced transformations and shape manipulations
/// </summary>
public class TransformationHelper
{
    /// <summary>
    /// Creates a path following a mathematical function
    /// </summary>
    /// <param name="func">The function that defines the path y = f(x)</param>
    /// <param name="xMin">Starting x value</param>
    /// <param name="xMax">Ending x value</param>
    /// <param name="steps">Number of steps/points to generate</param>
    /// <returns>List of Vector2 points along the path</returns>
    public static List<Vector2> CreateFunctionPath(System.Func<float, float> func, float xMin, float xMax, int steps)
    {
        List<Vector2> points = new List<Vector2>();
        float step = (xMax - xMin) / steps;
        
        for (int i = 0; i <= steps; i++)
        {
            float x = xMin + step * i;
            float y = func(x);
            points.Add(new Vector2(x, y));
        }
        
        return points;
    }
    
    /// <summary>
    /// Creates a circular path
    /// </summary>
    /// <param name="center">Center point of the circle</param>
    /// <param name="radius">Radius of the circle</param>
    /// <param name="steps">Number of points to generate</param>
    /// <returns>List of Vector2 points along the circle</returns>
    public static List<Vector2> CreateCirclePath(Vector2 center, float radius, int steps)
    {
        List<Vector2> points = new List<Vector2>();
        
        for (int i = 0; i <= steps; i++)
        {
            float angle = i * Mathf.Tau / steps;
            float x = center.x + radius * Mathf.Cos(angle);
            float y = center.y + radius * Mathf.Sin(angle);
            points.Add(new Vector2(x, y));
        }
        
        return points;
    }
    
    /// <summary>
    /// Creates a starlike path with variable inner and outer radii
    /// </summary>
    /// <param name="center">Center point of the star</param>
    /// <param name="outerRadius">Radius to outer points</param>
    /// <param name="innerRadius">Radius to inner points</param>
    /// <param name="points">Number of star points</param>
    /// <returns>List of Vector2 positions forming a star</returns>
    public static List<Vector2> CreateStarPath(Vector2 center, float outerRadius, float innerRadius, int points)
    {
        List<Vector2> starPoints = new List<Vector2>();
        int totalPoints = points * 2; // Total vertices (inner + outer)
        
        for (int i = 0; i < totalPoints; i++)
        {
            float radius = i % 2 == 0 ? outerRadius : innerRadius;
            float angle = i * Mathf.Tau / totalPoints;
            
            float x = center.x + radius * Mathf.Cos(angle);
            float y = center.y + radius * Mathf.Sin(angle);
            starPoints.Add(new Vector2(x, y));
        }
        
        return starPoints;
    }
    
    /// <summary>
    /// Applies a sequence of transformations to a set of points
    /// </summary>
    /// <param name="points">Original points</param>
    /// <param name="transformations">List of transformation matrices to apply in sequence</param>
    /// <returns>Transformed points</returns>
    public static List<Vector2> ApplyTransformationSequence(List<Vector2> points, List<Matrix3x3> transformations)
    {
        // Combine all transformations into a single matrix
        Matrix3x3 combinedMatrix = Matrix3x3.Identity;
        foreach (var matrix in transformations)
        {
            combinedMatrix *= matrix;
        }
        
        // Apply the combined transformation
        List<Vector2> result = new List<Vector2>();
        foreach (Vector2 point in points)
        {
            Vector3 transformed = combinedMatrix * new Vector3(point.x, point.y, 1);
            result.Add(new Vector2(transformed.x, transformed.y));
        }
        
        return result;
    }
    
    /// <summary>
    /// Creates a custom path with variable parameters for creative animations
    /// </summary>
    /// <param name="t">Time parameter (0-1)</param>
    /// <param name="amplitude">Size of the path</param>
    /// <param name="frequency">Frequency of oscillation</param>
    /// <param name="phase">Phase shift</param>
    /// <returns>A vector position along the custom path</returns>
    public static Vector2 GetCustomPath(float t, float amplitude, float frequency, float phase)
    {
        // A figure-8 like pattern with customizable parameters
        float x = amplitude * Mathf.Sin(t * frequency + phase);
        float y = amplitude * Mathf.Sin(t * frequency * 2 + phase) / 2;
        
        return new Vector2(x, y);
    }
    
    /// <summary>
    /// Applies easing functions for smoother animations
    /// </summary>
    /// <param name="t">Input time/progress (0-1)</param>
    /// <param name="type">Type of easing to apply</param>
    /// <returns>Eased value</returns>
    public static float ApplyEasing(float t, EasingType type)
    {
        switch (type)
        {
            case EasingType.EaseInQuad:
                return t * t;
                
            case EasingType.EaseOutQuad:
                return 1 - (1 - t) * (1 - t);
                
            case EasingType.EaseInOutQuad:
                return t < 0.5f ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
                
            case EasingType.EaseInCubic:
                return t * t * t;
                
            case EasingType.EaseOutCubic:
                return 1 - Mathf.Pow(1 - t, 3);
                
            case EasingType.EaseInOutCubic:
                return t < 0.5f ? 4 * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 3) / 2;
                
            case EasingType.EaseInElastic:
                if (t == 0) return 0;
                if (t == 1) return 1;
                return -Mathf.Pow(2, 10 * t - 10) * Mathf.Sin((t * 10 - 10.75f) * (2 * Mathf.Pi) / 3);
                
            case EasingType.EaseOutElastic:
                if (t == 0) return 0;
                if (t == 1) return 1;
                return Mathf.Pow(2, -10 * t) * Mathf.Sin((t * 10 - 0.75f) * (2 * Mathf.Pi) / 3) + 1;
                
            case EasingType.EaseInOutElastic:
                if (t == 0) return 0;
                if (t == 1) return 1;
                if (t < 0.5f)
                    return -(Mathf.Pow(2, 20 * t - 10) * Mathf.Sin((20 * t - 11.125f) * (2 * Mathf.Pi) / 4.5f)) / 2;
                return (Mathf.Pow(2, -20 * t + 10) * Mathf.Sin((20 * t - 11.125f) * (2 * Mathf.Pi) / 4.5f)) / 2 + 1;
                
            default:
                return t; // Linear (no easing)
        }
    }
    
    /// <summary>
    /// Types of easing functions available
    /// </summary>
    public enum EasingType
    {
        Linear,
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic,
        EaseInElastic,
        EaseOutElastic,
        EaseInOutElastic
    }
}
