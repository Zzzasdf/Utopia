using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Pool;

namespace Selector.Linear.Core.OptionalTween
{
    public class ZoomSelectedContainer : SelectedTweenUnitBaseCore
    {
        // 时间参数
        private DurationParam durationParam;
        // 目标参数
        private ScaleParam targetParam;

        public void Init(DurationParam durationParam, ScaleParam targetParam)
        {
            this.durationParam = durationParam;
            this.targetParam = targetParam;
        }
        
        protected override (List<RectTransform>, List<TweenerCore<Vector3, Vector3, VectorOptions>>) ImplementTween(int preSelectedDataIndex, TweenCallback completeCallback)
        {
            List<RectTransform> resultTransforms = ListPool<RectTransform>.Get();
            List<TweenerCore<Vector3, Vector3, VectorOptions>> resultTweenerCores = ListPool<TweenerCore<Vector3, Vector3, VectorOptions>>.Get();
            for (int i = 0; i < linearSelectorCore.ItemsTra.Count; i++)
            {
                RectTransform item = linearSelectorCore.ItemsTra[i];
                float duration = durationParam.GetValue(linearSelectorCore, i, preSelectedDataIndex);
                TweenerCore<Vector3, Vector3, VectorOptions> tweenLocalScale
                    = item.DOScale(targetParam.GetValue(linearSelectorCore, i, preSelectedDataIndex), duration)
                        .SetEase(targetParam.GetEase());
                
                resultTransforms.Add(item);
                resultTweenerCores.Add(tweenLocalScale);
            }
            float maxDuration = durationParam.GetMaxValue(linearSelectorCore, preSelectedDataIndex);
            TweenerCore<Vector3, Vector3, VectorOptions> tweenerComplete = DOTween.To(()=> default(Vector3), _ => { }, Vector3.zero, maxDuration);
            tweenerComplete.OnComplete(completeCallback);
            resultTweenerCores.Add(tweenerComplete);
            return (resultTransforms, resultTweenerCores);
        }
    }
}
