using UnityEngine;

namespace Selector.Linear.Core.OptionalTween
{
    // 持续时间 递减 最小 值
    public class DurationLerpDecreaseMinParam : DurationLerpDecreaseParam
    {
        [SerializeField] [Header("最小值")] protected int minValue = 30;
        
        public override float GetValue(LinearSelectorCore linearSelectorCore, int itemIndex, int selectedDataIndex)
        {
            int result = value - lerpDecreaseValue * (linearSelectorCore.ItemsTra.Count - 1 - itemIndex);
            return Mathf.Max(result / 1000f, minValue / 1000f);
        }
    }
}