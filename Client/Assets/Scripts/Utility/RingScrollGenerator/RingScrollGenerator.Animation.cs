using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public partial class RingScrollGenerator
{
    /// 滚动动画
    private enum ScrollAni
    {
        /// 线性，因为是椭圆，所以会 先向内聚拢，再向外分散
        Linear,
        
        /// 弧线，绕着椭圆弧边移动
        Arc,
    }

    private static Dictionary<ScrollAni, IScrollAni> scrollAniMap = new Dictionary<ScrollAni, IScrollAni>
    {
        [ScrollAni.Linear] = new ScrollAniLinear(),
        [ScrollAni.Arc] = new ScrollAniArc(),
    };
    
    public interface IScrollAni
    {
        void MoveAni(Transform itemTra, Vector2 originPos, Vector2 targetPos, float progress, 
            int scrollDir, float xAxisRadius, float yAxisRadius);
    }

    public class ScrollAniLinear: IScrollAni
    {
        void IScrollAni.MoveAni(Transform itemTra, Vector2 originPos, Vector2 targetPos, float progress, 
            int scrollDir, float xAxisRadius, float yAxisRadius)
        {
            itemTra.localPosition = originPos + (targetPos - originPos) * progress;
        }
    }

    public class ScrollAniArc: IScrollAni
    {
        void IScrollAni.MoveAni(Transform itemTra, Vector2 originPos, Vector2 targetPos, float progress, 
            int scrollDir, float xAxisRadius, float yAxisRadius)
        {
            if (originPos == Vector2.zero || originPos == targetPos)
            {
                return;
            }
            if (progress >= 1)
            {
                // 去除误差
                itemTra.transform.localPosition = targetPos;
                return;
            }
            
            float originAngeleDeg = Mathf.Acos(originPos.x / xAxisRadius) * Mathf.Rad2Deg;
            float originYDir = Vector2.Dot(Vector2.up, originPos);
            float originXDir = Vector2.Dot(Vector2.right, originPos);
            
            float targetAngeleDeg = Mathf.Acos(targetPos.x / xAxisRadius) * Mathf.Rad2Deg;
            float targetYDir = Vector2.Dot(Vector2.up, targetPos);
            float targetXDir = Vector2.Dot(Vector2.right, targetPos);
            
            float diffDeg = 0;
            if (originYDir * targetYDir > 0)
            {
                // 同 y 轴向
                diffDeg = Mathf.Abs(originAngeleDeg - targetAngeleDeg);
            }
            else
            {
                if (originAngeleDeg + targetAngeleDeg > 180)
                {
                    diffDeg = 360 - originAngeleDeg - targetAngeleDeg;
                }
                else
                {
                    diffDeg = originAngeleDeg + targetAngeleDeg;
                }
            }
            
            originAngeleDeg *= (originYDir, originXDir) switch
            {
                (0, > 0) => 1,
                (0, < 0) => -1,
                (> 0, _) => 1,
                (< 0, _) => -1,
            };
            // targetAngeleDeg *= (targetYDir, targetXDir) switch
            // {
            //     (0, > 0) => 1,
            //     (0, < 0) => -1,
            //     (> 0, _) => 1,
            //     (< 0, _) => -1,
            // };

            float newDeg = originAngeleDeg + scrollDir * diffDeg * progress;
            float angleRadians = newDeg * Mathf.Deg2Rad;
            float x = xAxisRadius * Mathf.Cos(angleRadians);
            float y = yAxisRadius * Mathf.Sin(angleRadians);
            itemTra.transform.localPosition = new Vector2(x, y);
        }
    }
}
