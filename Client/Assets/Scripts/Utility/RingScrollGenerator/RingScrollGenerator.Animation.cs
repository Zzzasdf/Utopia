using System.Collections.Generic;
using UnityEngine;

public partial class RingScrollGenerator
{
    /// 滚动动画
    private enum EScrollAni
    {
        /// 线性，最短距离
        Linear,
        
        /// 弧线，绕着边移动
        Arc,
    }

    private static Dictionary<EScrollAni, IScrollAni> scrollAniMap = new()
    {
        [EScrollAni.Linear] = new ScrollAniLinear(),
        [EScrollAni.Arc] = new ScrollAniArc(),
    };
    
    public interface IScrollAni
    {
        void MoveAni(IShape shape, Transform itemTra, Vector2 originPos, Vector2 targetPos, float progress, 
            int scrollDir, int xAxisRadius, int yAxisRadius);
    }

    public class ScrollAniLinear: IScrollAni
    {
        void IScrollAni.MoveAni(IShape shape, Transform itemTra, Vector2 originPos, Vector2 targetPos, float progress, 
            int scrollDir, int xAxisRadius, int yAxisRadius)
        {
            itemTra.localPosition = originPos + (targetPos - originPos) * progress;
        }
    }

    public class ScrollAniArc: IScrollAni
    {
        void IScrollAni.MoveAni(IShape shape, Transform itemTra, Vector2 originPos, Vector2 targetPos, float progress, 
            int scrollDir, int xAxisRadius, int yAxisRadius)
        {
            if (originPos == Vector2.zero || originPos == targetPos)
            {
                return;
            }

            float originDeg = GetPositiveDeg(originPos / new Vector2(xAxisRadius, yAxisRadius));
            float targetDeg = GetPositiveDeg(targetPos / new Vector2(xAxisRadius, yAxisRadius));
            float diffDeg = GetDiffDeg(originDeg, targetDeg, scrollDir);

            float newDeg = originDeg + diffDeg * progress;
            itemTra.transform.localPosition = shape.GetLocalPos(xAxisRadius, yAxisRadius, newDeg);
            return;

            float GetDiffDeg(float originDeg, float targetDeg, int dir)
            {
                float diffAngeleDefAbsDiff = Mathf.Abs(targetDeg - originDeg);
                if (diffAngeleDefAbsDiff <= 180)
                {
                    return Mathf.Abs(targetDeg - originDeg) * dir;
                }
                if (originDeg > targetDeg)
                {
                    targetDeg += 360;
                }
                else
                {
                    originDeg += 360;
                }
                return Mathf.Abs(targetDeg - originDeg) * dir;
            }
        }
    }
}