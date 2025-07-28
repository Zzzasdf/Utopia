using UnityEngine;
using UnityEngine.UI;

public partial class LeafSizeFitter
{
    public class Layout
    {
        /// <summary>
        /// 自适应自身的大小，不计算子对象！！
        /// </summary>
        /// <param name="rectSelf">需要自适应的对象</param>
        /// <param name="setNativeSizeX">是否自适应 X 轴</param>
        /// <param name="setNativeSizeY">是否自适应 Y 轴</param>
        public static void SetLeafSize(RectTransform rectSelf, bool setNativeSizeX, bool setNativeSizeY)
        {
            if (setNativeSizeX)
            {
                RectTransform.Axis axis = RectTransform.Axis.Horizontal;
                rectSelf.SetSizeWithCurrentAnchors(axis, LayoutUtility.GetPreferredSize(rectSelf, (int)axis));
            }

            if (setNativeSizeY)
            {
                RectTransform.Axis axis = RectTransform.Axis.Vertical;
                rectSelf.SetSizeWithCurrentAnchors(axis, LayoutUtility.GetPreferredSize(rectSelf, (int)axis));
            }
        }
    }
}