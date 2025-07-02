using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Generator.Ring
{
    public partial class Generator
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

        private static Dictionary<EShape, IShape> ShapeMap = new()
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

            private Vector2Int GetIntersection(int count, bool positive, int xAxisRadius, int yAxisRadius, float deg)
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

                // 画出多边形顶点
                List<Vector2Int> shape = ListPool<Vector2Int>.Get();
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
            private Vector2Int GetIntersection(IReadOnlyList<Vector2Int> shape, int xAxisRadius, int yAxisRadius, float deg)
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
            private float GetPositiveDeg(Vector2 pos)
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
    }
}