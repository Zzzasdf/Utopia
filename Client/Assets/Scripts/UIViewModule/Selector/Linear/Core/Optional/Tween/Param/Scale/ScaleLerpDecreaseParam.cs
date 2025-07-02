using UnityEngine;

namespace Selector.Linear.Core.OptionalTween
{
    // 缩放 递减 值
    public class ScaleLerpDecreaseParam : ScaleParam 
    {
        [SerializeField] [Header("递减值")] protected float lerpDecreaseValue = 0.05f;
        [SerializeField] [Header("索引计算器")] protected IItemDataIndexParamCore itemDataIndexParamCore;

        public override Vector3 GetValue(LinearSelectorCore linearSelectorCore, int itemIndex, int selectedDataIndex)
        {
            int selectedIndex = selectedDataIndex - linearSelectorCore.SetDisplayStartDataIndex(selectedDataIndex);
            int layerCount = Mathf.Abs(selectedIndex - itemIndex);
            float itemScale = value - lerpDecreaseValue * layerCount;
            return Vector3.one * itemScale;
        }
    }
}