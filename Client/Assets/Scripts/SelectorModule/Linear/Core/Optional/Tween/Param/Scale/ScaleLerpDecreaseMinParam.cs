using UnityEngine;

namespace Selector.Linear.Core.OptionalTween
{
    // 缩放 递减 最小 值
    public class ScaleLerpDecreaseMinParam : ScaleLerpDecreaseParam
    {
        [SerializeField] [Header("最小值")] protected float minValue = 0.8f;
        
        public override Vector3 GetValue(LinearSelectorCore linearSelectorCore, int itemIndex, int selectedDataIndex)
        {
            int selectedIndex = selectedDataIndex - linearSelectorCore.SetDisplayStartDataIndex(selectedDataIndex); 
            int layerCount = Mathf.Abs(selectedIndex - itemIndex);
            float itemScale = Mathf.Max(minValue, value - lerpDecreaseValue * layerCount);
            return Vector3.one * itemScale;
        }
    }
}