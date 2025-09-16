using System;
using UnityEngine;

public partial class HVLayoutGroup
{
    public class Layout
    {
        public enum EDirection 
        {
            /// 正向，从左上开始排序
            Forward = 1,
            /// 逆向，从右下开始排序
            Reverse = -1,
        }
        public enum ELayout
        {
            /// 横向布局
            Horizontal = 0,
            /// 纵向布局
            Vertical = 1,
        }
        [Serializable]
        public struct Padding
        {
            public static readonly Padding Default = new Padding(0, 0, 0, 0);
            public int Left;
            public int Right;
            public int Top;
            public int Bottom;
            public int Horizontal => Left + Right;
            public int Vertical => Top + Bottom;
            public Padding(int left, int right, int top, int bottom)
            {
                Left = left;
                Right = right;
                Top = top;
                Bottom = bottom;
            }
        }

        /// <summary>设置子对象布局</summary>
        public static Vector2 SetChildrenLayout(RectTransform rectParent, TextAnchor textAnchor, ELayout eLayout)
        {
            return SetChildrenLayout(rectParent, textAnchor, eLayout, 0, Padding.Default);
        }
        /// <summary>设置子对象布局</summary>
        public static Vector2 SetChildrenLayout(RectTransform rectParent, TextAnchor textAnchor, ELayout eLayout, float spacing)
        {
            return SetChildrenLayout(rectParent, textAnchor, eLayout, spacing, Padding.Default);
        }
        /// <summary>设置子对象布局</summary>
        public static Vector2 SetChildrenLayout(RectTransform rectParent, TextAnchor textAnchor, ELayout eLayout, in Padding padding)
        {
            return SetChildrenLayout(rectParent, textAnchor, eLayout, 0, padding);
        }
        /// <summary>设置子对象布局</summary>
        public static Vector2 SetChildrenLayout(RectTransform rectParent, TextAnchor textAnchor, ELayout eLayout, float spacing, in Padding padding)
        {
            return SetChildrenLayout(rectParent, textAnchor, eLayout, EDirection.Forward, spacing, padding);
        }
    
        /// <summary>
        /// 设置子对象布局
        /// </summary>
        /// <param name="rectParent">父节点</param>
        /// <param name="textAnchor">锚点</param>
        /// <param name="eLayout">布局方式</param>
        /// <param name="eDir">显示方向</param>
        /// <param name="spacing">间隔</param>
        /// <param name="padding">边距</param>
        /// <returns>子节点集用到的矩形大小</returns>
        public static Vector2 SetChildrenLayout(RectTransform rectParent, TextAnchor textAnchor, ELayout eLayout, EDirection eDir, float spacing, in Padding padding)
        {
            using (PooledList<RectTransform> list = PooledList<RectTransform>.Get())
            {
                int childCount = rectParent.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    Transform childTra = rectParent.GetChild(i);
                    if (!childTra.gameObject.activeInHierarchy) continue; // 跳过隐藏的
                    list.Add(childTra.GetComponent<RectTransform>());
                }
                if (list.Count == 0) return Vector2.zero; // 没有对象不计算边距，特化！！
                
                Vector2 anchor = textAnchor switch
                {
                    TextAnchor.UpperLeft => new Vector2(0f, 1f),
                    TextAnchor.UpperCenter => new Vector2(0.5f, 1f),
                    TextAnchor.UpperRight => new Vector2(1f, 1f),
                    TextAnchor.MiddleLeft => new Vector2(0f, 0.5f),
                    TextAnchor.MiddleCenter => new Vector2(0.5f, 0.5f),
                    TextAnchor.MiddleRight => new Vector2(1f, 0.5f),
                    TextAnchor.LowerLeft => new Vector2(0f, 0f),
                    TextAnchor.LowerCenter => new Vector2(0.5f, 0f),
                    TextAnchor.LowerRight => new Vector2(1f, 0f),
                    _ => Vector2.zero,
                };
                
                Func<Rect, (float axisLength, float anotherAxisLength)> rectAxisFunc;
                Func<Vector2, float> anchorAxisFunc;
                Func<float, Vector2, Vector2> anchoredPositionFunc;
                Func<EDirection, int, (int startIndex, int increaseDir)> forItemInfoFunc;
                if (eLayout == ELayout.Horizontal)
                {
                    rectAxisFunc = rectSideLengthFuncH;
                    anchorAxisFunc = anchorAxisFuncH;
                    anchoredPositionFunc = anchoredPositionFuncH;
                    forItemInfoFunc = forItemInfoFuncH;
                }
                else
                {
                    rectAxisFunc = rectSideLengthFuncV;
                    anchorAxisFunc = anchorAxisFuncV;
                    anchoredPositionFunc = anchoredPositionFuncV;
                    forItemInfoFunc = forItemInfoFuncV;
                }
                
                float totalLength = 0;
                float anotherTotalLength = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    (float axis, float anotherAxis) = rectAxisFunc(list[i].rect);
                    totalLength += axis;
                    if (anotherAxis <= anotherTotalLength) continue;
                    anotherTotalLength = anotherAxis;
                }
                (int axisPadding, int anotherAxisPadding) = GetAxisPadding(eLayout, padding);
                totalLength += spacing * (list.Count - 1) + axisPadding;
                anotherTotalLength += anotherAxisPadding;
                
                float anchorAxis = anchorAxisFunc(anchor);
                float startAxis = -totalLength * anchorAxis;
                (int startIndex, int increaseDir) = forItemInfoFunc(eDir, list.Count);
                Vector2 paddingOffset = GetAxisPaddingOffset(eLayout, anchor, padding);
                for (int count = list.Count; count > 0; count--)
                {
                    RectTransform itemTra = list[startIndex];
                    itemTra.anchorMin = anchor;
                    itemTra.anchorMax = anchor;
                    itemTra.pivot = anchor;
                    (float axis, _) = rectAxisFunc(itemTra.rect);
                    float currOffset = axis * anchorAxis;
                    itemTra.anchoredPosition = anchoredPositionFunc(startAxis + currOffset, paddingOffset);
                    startAxis += axis + spacing;
                    startIndex += increaseDir;
                }
                return CalculateRect(eLayout, totalLength, anotherTotalLength);
            }
        }
        private static Func<Rect, (float axisLength, float anotherAxisLength)> rectSideLengthFuncH = rect => (rect.width, rect.height);
        private static Func<Rect, (float axisLength, float anotherAxisLength)> rectSideLengthFuncV = rect => (rect.height, rect.width);
        private static Func<Vector2, float> anchorAxisFuncH = anchor => anchor.x;
        private static Func<Vector2, float> anchorAxisFuncV = anchor => anchor.y;
        private static Func<float, Vector2, Vector2> anchoredPositionFuncH = (axis, paddingOffset) => new Vector2(axis + paddingOffset.x, paddingOffset.y);
        private static Func<float, Vector2, Vector2> anchoredPositionFuncV = (axis, paddingOffset) => new Vector2(paddingOffset.x, axis + paddingOffset.y);
        private static Func<EDirection, int, (int startIndex, int increaseDir)> forItemInfoFuncH = (dir, count) => (dir == EDirection.Forward ? 0 : count - 1, (int)dir);
        private static Func<EDirection, int, (int startIndex, int increaseDir)> forItemInfoFuncV = (dir, count) => (dir == EDirection.Forward ? count - 1 : 0, -(int)dir);

        static (int axisPadding, int anotherAxisPadding) GetAxisPadding(ELayout eLayout, in Padding padding)
        {
            if (eLayout == ELayout.Horizontal)
                return (padding.Horizontal, padding.Vertical);
            return (padding.Vertical, padding.Horizontal);
        }
        static Vector2 GetAxisPaddingOffset(ELayout eLayout, Vector2 anchor, in Padding padding)
        {
            if (eLayout == ELayout.Horizontal)
                return new Vector2(padding.Left, padding.Bottom * (1 - anchor.y) - padding.Top * anchor.y);
            return new Vector2(padding.Left * (1 - anchor.x) - padding.Right * anchor.x, padding.Bottom);
        }
        static Vector2 CalculateRect(ELayout eLayout, float totalLength, float anotherTotalLength)
        {
            if (eLayout == ELayout.Horizontal)
                return new Vector2(totalLength, anotherTotalLength);
            return new Vector2(anotherTotalLength, totalLength);
        }
    }
}
