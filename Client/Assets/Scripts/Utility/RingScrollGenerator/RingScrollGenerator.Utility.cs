using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public partial class RingScrollGenerator
{
    /// <summary>
    /// 以椭圆为外框，内置多边形
    /// </summary>
    /// <param name="count">边的数量</param>
    /// <param name="positive">是否正向</param>
    /// <param name="xAxisRadius">椭圆短轴</param>
    /// <param name="yAxisRadius">椭圆长轴</param>
    /// <param name="deg">当前角度</param>
    /// <returns></returns>
    private static Vector2Int GetIntersection(int count, bool positive, int xAxisRadius, int yAxisRadius, float deg)
    {
        while (deg < 0)
        {
            deg += 360;
        }
        while (deg > 360)
        {
            deg -= 360;
        }
        float angleUnit = 360f / count;
        float startAngle = 90;
        if (count % 2 == 0)
        {
            startAngle += positive ? angleUnit / 2 : 0;
        }
        else
        {
            startAngle += positive ? 0 : angleUnit / 2;
        }
        
        List<Vector2Int> shape = ListPool<Vector2Int>.Get();
        
        // 画出多边形顶点
        for (int i = 0; i < count; i++)
        {
            float radians = (startAngle + angleUnit * i) * Mathf.Deg2Rad;
            float x = xAxisRadius * Mathf.Cos(radians);
            float y = yAxisRadius * Mathf.Sin(radians);
            shape.Add(new Vector2Int(Mathf.RoundToInt(x), Mathf.RoundToInt(y)));
        }
        Vector2Int result = GetIntersection(shape, xAxisRadius, yAxisRadius, deg);
        ListPool<Vector2Int>.Release(shape);
        return result;
    }
    
    /// 获取 闭环 图形 与 角度 的交点
    private static Vector2Int GetIntersection(IReadOnlyList<Vector2Int> shape, int xAxisRadius, int yAxisRadius, float deg)
    {
        List<float> degs = ListPool<float>.Get();
        for (int i = 0; i < shape.Count; i++)
        {
            Vector2Int corner = shape[i];
            degs.Add(GetPositiveDeg(corner));
        }

        float originDeg;
        float targetDeg;
        
        for (int i = 0; i + 1 < degs.Count; i++)
        {
            originDeg = degs[i];
            targetDeg = degs[i + 1];
            
            // 逆时针, 跨 0 度
            if (targetDeg - originDeg < 0)
            {
                if (deg >= targetDeg && deg < originDeg)
                {
                    continue;
                }
            }
            else if (deg < originDeg || deg >= targetDeg)
            {
                continue;
            }
            ListPool<float>.Release(degs);
            return GetLineIntersection((originDeg, targetDeg), deg);
        }
        originDeg = degs[^1];
        targetDeg = degs[0];
        ListPool<float>.Release(degs);
        return GetLineIntersection((originDeg, targetDeg), deg);
        
        Vector2Int GetLineIntersection((float originDeg, float targetDeg) line, float assignDeg)
        {
            float originRad = line.originDeg * Mathf.Deg2Rad;
            Vector2 originPos = new Vector2(xAxisRadius * Mathf.Cos(originRad), yAxisRadius * Mathf.Sin(originRad));
            float targetRad = line.targetDeg * Mathf.Deg2Rad;
            Vector2 targetPos = new Vector2(xAxisRadius * Mathf.Cos(targetRad), yAxisRadius * Mathf.Sin(targetRad));
            float assignRad = assignDeg * Mathf.Deg2Rad;
            Vector2 assignPos = new Vector2(xAxisRadius * Mathf.Cos(assignRad), yAxisRadius * Mathf.Sin(assignRad));

            (Vector2 from, Vector2 to) line01 = (originPos, targetPos);
            (Vector2 from, Vector2 to) line02 = (Vector2.zero, assignPos);
            
            float a01 = line01.to.y - line01.from.y;
            float b01 = line01.from.x - line01.to.x;
            float c01 = a01 * line01.from.x + b01 * line01.from.y;
            
            float a02 = line02.to.y - line02.from.y;
            float b02 = line02.from.x - line02.to.x;
            float c02 = a02 * line02.from.x + b02 * line02.from.y;
            
            float determinant = a01 * b02 - a02 * b01;
            
            if (Mathf.Approximately(determinant, 0f))
            {
                // 计算机误差，导致两条线平行或重合
                // 处理方案：不移动
                return new Vector2Int(Mathf.RoundToInt(line01.from.x), Mathf.RoundToInt(line01.from.y));
            }
            float x = (b02 * c01 - b01 * c02) / determinant;
            float y = (a01 * c02 - a02 * c01) / determinant;
            return new Vector2Int(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
        }
    }
    
    /// 转换成正向角度
    private static float GetPositiveDeg(Vector2 pos)
    {
        float deg = Vector2.Angle(Vector2.right, pos);
        float targetYDir = Vector2.Dot(Vector2.up, pos);
        if (targetYDir < 0)
        {
            deg = 360 - deg;
        }
        return deg;
    }
}
