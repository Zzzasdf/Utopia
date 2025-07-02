using DG.Tweening;
using UnityEngine;

namespace Selector.Linear.Core.OptionalTween
{
    // 缩放 值
    public class ScaleParam : MonoBehaviour
    {
        [SerializeField] [Header("动画曲线")] private Ease eEase = DOTween.defaultEaseType;
        [SerializeField] [Header("缩放值")] protected float value = 1.2f;

        public Ease GetEase() => eEase;
        public virtual Vector3 GetValue(LinearSelectorCore linearSelectorCore, int itemIndex, int selectedDataIndex) => Vector3.one * value;
    }
}
