using System.Collections.Generic;
using UnityEngine;

public partial class RingScrollGenerator
{
    /// 形状
    public enum EShape
    {
        /// 椭圆形
        Oval = 1,
        /// 矩形
        Rectangle = 2,
    }
    
    private static Dictionary<EShape, IShape> shapeMap = new()
    {
        [EShape.Oval] = new CircleShape(),
        [EShape.Rectangle] = new SquareShape(),
    };
    
    public interface IShape
    {
        /// 椭圆通过 正圆 长短边比例 实现，非圆是在通过边交点
        bool IsCircle();
        Vector3Int GetLocalPos(int xAxisRadius, int yAxisRadius, float angle);
    }

    public class CircleShape : IShape
    {
        bool IShape.IsCircle() => true;
        Vector3Int IShape.GetLocalPos(int xAxisRadius, int yAxisRadius, float angle)
        {
            float radians = angle * Mathf.Deg2Rad;
            float x = xAxisRadius * Mathf.Cos(radians);
            float y = yAxisRadius * Mathf.Sin(radians);
            return new Vector3Int(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
        }
    }

    public class SquareShape : IShape
    {
        bool IShape.IsCircle() => false;
        public Vector3Int GetLocalPos(int xAxisRadius, int yAxisRadius, float angle)
        {
            while (angle < 0)
            {
                angle += 360;
            }
            angle %= 360;
            
            float radians = angle * Mathf.Deg2Rad;
            // 对角角度
            float diagonalAngel = Mathf.Atan2(yAxisRadius, xAxisRadius) * Mathf.Rad2Deg;
            float x = 0;
            float y = 0;
            // 右边
            if ((angle >= 360 - diagonalAngel && diagonalAngel < 360)
                || (angle >= 0 && angle < diagonalAngel))
            {
                x = xAxisRadius;
                y = xAxisRadius * Mathf.Tan(radians);
            }
            // 上边
            else if (angle >= diagonalAngel && angle < 180 - diagonalAngel)
            {
                y = yAxisRadius;
                x = yAxisRadius / Mathf.Tan(radians);
            }
            // 左边
            else if (angle >= 180 - diagonalAngel && angle < 180 + diagonalAngel)
            {
                x = -xAxisRadius;
                y = -xAxisRadius * Mathf.Tan(radians);
            }
            // 下边
            else if (angle >= 180 + diagonalAngel && angle < 360 - diagonalAngel)
            {
                y = -yAxisRadius;
                x = -yAxisRadius / Mathf.Tan(radians);
            }
            return new Vector3Int(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
        }
    }
}