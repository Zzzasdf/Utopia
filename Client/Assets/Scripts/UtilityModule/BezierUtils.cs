using UnityEngine;

public static class BezierUtils
{
    /// <summary>
    /// 检测点是否在贝塞尔曲线附近
    /// </summary>
    /// <param name="start">起点</param>
    /// <param name="end">终点</param>
    /// <param name="startTangent">起点控制点</param>
    /// <param name="endTangent">终点控制点</param>
    /// <param name="point">检测点</param>
    /// <param name="maxDistance">最大允许距离</param>
    /// <param name="steps">采样精度（默认20）</param>
    /// <returns></returns>
    public static bool IsPointNearBezier(
        Vector2 start,
        Vector2 end,
        Vector2 startTangent,
        Vector2 endTangent,
        Vector2 point,
        float maxDistance = 10f,
        int steps = 20)
    {
        // 优化1：快速包围盒检测
        if (!IsPointInBezierBounds(start, end, startTangent, endTangent, point, maxDistance))
            return false;

        // 优化2：起点/终点优先检测（常见点击位置）
        if (Vector2.Distance(point, start) <= maxDistance) return true;
        if (Vector2.Distance(point, end) <= maxDistance) return true;
        
        // 采样曲线计算最近距离
        Vector2 prevPoint = start;
        float minDistance = float.MaxValue;
        
        for (int i = 1; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector2 curvePoint = CalculateBezierPoint(t, start, end, startTangent, endTangent);
            
            // 计算点到线段的距离
            float dist = DistanceToLineSegment(prevPoint, curvePoint, point);
            minDistance = Mathf.Min(minDistance, dist);
            
            // 提前退出优化
            if (minDistance <= maxDistance) return true;
            
            prevPoint = curvePoint;
        }
        
        return minDistance <= maxDistance;
    }

    /// <summary>
    /// 计算贝塞尔曲线上的点
    /// </summary>
    private static Vector2 CalculateBezierPoint(
        float t,
        Vector2 start,
        Vector2 end,
        Vector2 startTangent,
        Vector2 endTangent)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;
        
        Vector2 point = uuu * start; // (1-t)^3 * P0
        point += 3 * uu * t * startTangent; // 3(1-t)^2 * t * P1
        point += 3 * u * tt * endTangent; // 3(1-t) * t^2 * P2
        point += ttt * end; // t^3 * P3
        
        return point;
    }

    /// <summary>
    /// 计算点到线段的最短距离
    /// </summary>
    private static float DistanceToLineSegment(Vector2 a, Vector2 b, Vector2 point)
    {
        Vector2 ab = b - a;
        Vector2 ap = point - a;
        
        float lengthSqr = ab.sqrMagnitude;
        if (lengthSqr == 0) return Vector2.Distance(a, point);
        
        float t = Mathf.Clamp01(Vector2.Dot(ap, ab) / lengthSqr);
        Vector2 projection = a + t * ab;
        
        return Vector2.Distance(point, projection);
    }

    /// <summary>
    /// 快速包围盒检测
    /// </summary>
    private static bool IsPointInBezierBounds(
        Vector2 start,
        Vector2 end,
        Vector2 startTangent,
        Vector2 endTangent,
        Vector2 point,
        float margin)
    {
        // 计算曲线边界
        float minX = Mathf.Min(start.x, end.x, startTangent.x, endTangent.x);
        float maxX = Mathf.Max(start.x, end.x, startTangent.x, endTangent.x);
        float minY = Mathf.Min(start.y, end.y, startTangent.y, endTangent.y);
        float maxY = Mathf.Max(start.y, end.y, startTangent.y, endTangent.y);
        
        // 扩展边界
        Rect bounds = Rect.MinMaxRect(
            minX - margin, 
            minY - margin, 
            maxX + margin, 
            maxY + margin);
        
        return bounds.Contains(point);
    }
}