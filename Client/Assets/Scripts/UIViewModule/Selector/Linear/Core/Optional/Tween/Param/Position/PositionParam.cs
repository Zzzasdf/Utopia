using DG.Tweening;
using UnityEngine;

namespace Selector.Linear.Core.OptionalTween
{
    // 坐标 值
    public class PositionParam : MonoBehaviour
    {
        [SerializeField] [Header("动画曲线")] private Ease eEase = DOTween.defaultEaseType;
        [SerializeField] [Header("坐标")] protected Vector3 value = Vector3.zero;
        public Ease GetEase() => eEase;
        public virtual Vector3 GetValue(LinearSelectorCore linearSelectorCore, int itemIndex, int selectedDataIndex)
        {
            return value;
        }
    }
}