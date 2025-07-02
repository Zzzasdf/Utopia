using UnityEngine;

namespace Selector.Linear.Core.OptionalTween
{
    // 持续时间 值
    [DisallowMultipleComponent]
    public class DurationParam : MonoBehaviour
    {
        [SerializeField] [Header("持续时间（单位：毫秒）")] protected int value = 300;

        public virtual float GetValue(LinearSelectorCore linearSelectorCore, int itemIndex, int selectedDataIndex)
        {
            return value / 1000f;
        }

        public virtual float GetMaxValue(LinearSelectorCore linearSelectorCore, int selectedDataIndex)
        {
            return GetValue(linearSelectorCore, linearSelectorCore.ItemsTra.Count - 1, selectedDataIndex);
        }
    }
}
