using UnityEngine;

namespace Selector.Linear.Core.OptionalTween
{
    // 坐标 递减 值
    public class PositionLerpDecreaseParam : PositionParam
    {
        [SerializeField] [Header("递减值")] protected Vector3 lerpDecreaseValue = new Vector3(160f, 0, 0);

        public override Vector3 GetValue(LinearSelectorCore linearSelectorCore, int itemIndex, int selectedDataIndex)
        {
            float index = -(linearSelectorCore.ItemsTra.Count / 2 - 0.5f * ((linearSelectorCore.ItemsTra.Count + 1) & 1));
            return value + lerpDecreaseValue * (index + itemIndex);
        }
    }
}