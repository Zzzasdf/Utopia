using UnityEngine;

namespace Selector.Linear.Core.OptionalTween

{
    // 持续时间 递减 值
    public class DurationLerpDecreaseParam : DurationParam
    {
        [SerializeField] [Header("递减（单位：毫秒）")] protected int lerpDecreaseValue = 30;
        
        public override float GetValue(LinearSelectorCore linearSelectorCore, int itemIndex, int selectedDataIndex)
        {
            int result = value - lerpDecreaseValue * (linearSelectorCore.ItemsTra.Count - 1 - itemIndex);
            return result / 1000f;
        }
    }
}
