using System.Collections.Generic;
using UnityEngine;

public partial class RingScrollGenerator
{
    /// 形状
    private enum EShape
    {
        /// 圆形
        Circle = 1,
        /// 三角形
        Triangle = 3, Triangle_Arc = 31,
        /// 四边形
        Quadrilateral = 4, Quadrilateral_Arc = 41,
        /// 五边形
        Pentagon = 5, Pentagon_Arc = 51,
        /// 六边形
        Hexagon = 6, Hexagon_Arc = 61,
        /// 八边形
        Octagon = 8, Octagon_Arc = 81,
    }
    
    private static Dictionary<EShape, IShape> shapeMap = new()
    {
        [EShape.Circle] = new CircleShape(),
        [EShape.Triangle] = new PolygonShape(3, true),
        [EShape.Triangle_Arc] = new PolygonShape(3, false),
        [EShape.Quadrilateral] = new PolygonShape(4, true),
        [EShape.Quadrilateral_Arc] = new PolygonShape(4, false),
        [EShape.Pentagon] = new PolygonShape(5, true),
        [EShape.Pentagon_Arc] = new PolygonShape(5, false),
        [EShape.Hexagon] = new PolygonShape(6, true),
        [EShape.Hexagon_Arc] = new PolygonShape(6, false),
        [EShape.Octagon] = new PolygonShape(8, true),
        [EShape.Octagon_Arc] = new PolygonShape(8, false),
    };
    
    public interface IShape
    {
        Vector3Int GetLocalPos(int xAxisRadius, int yAxisRadius, float angle);
    }

    public class CircleShape : IShape
    {
        Vector3Int IShape.GetLocalPos(int xAxisRadius, int yAxisRadius, float angle)
        {
            float radians = angle * Mathf.Deg2Rad;
            float x = xAxisRadius * Mathf.Cos(radians);
            float y = yAxisRadius * Mathf.Sin(radians);
            return new Vector3Int(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
        }
    }

    /// 多边形
    public class PolygonShape : IShape
    {
        /// 边数量
        private int count;

        /// 是否正向
        private bool positive;

        public PolygonShape(int count, bool positive)
        {
            this.count = count;
            this.positive = positive;
        }
        
        Vector3Int IShape.GetLocalPos(int xAxisRadius, int yAxisRadius, float angle)
        {
            return (Vector3Int)GetIntersection(count, positive, xAxisRadius, yAxisRadius, angle);
        }
    }
}