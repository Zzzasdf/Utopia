// using System.Collections.Generic;
// using DG.Tweening;
// using DG.Tweening.Core;
// using DG.Tweening.Plugins.Options;
// using UnityEngine;
// using UnityEngine.Pool;
//
// namespace Selector.Linear.Core.OptionalTween
// {
//     public class FlyInEnterTweenCore : EnterTweenUnitBaseCore
//     {
//         // 时间参数
//         private DurationParam durationParam;
//         // 初始参数
//         private InitialParam initialParam;
//         // 目标参数
//         private TargetParam targetParam;
//
//         public void Init(DurationParam durationParam, InitialParam initialParam, TargetParam targetParam)
//         {
//             this.durationParam = durationParam;
//             this.initialParam = initialParam;
//             this.targetParam = targetParam;
//         }
//         
//         protected override void BeforeRunTweenBeforeExtraCallback()
//         {
//             for (int i = 0; i < linearSelectorCore.Items.Count; i++)
//             {
//                 linearSelectorCore.ItemsTra[i].localPosition = initialParam.PositionParam.GetValue(linearSelectorCore, i, linearSelectorCore.CurrSelectedDataIndex);
//                 linearSelectorCore.ItemsTra[i].localScale = initialParam.ScaleParam.GetValue(linearSelectorCore, i, linearSelectorCore.CurrSelectedDataIndex);
//             }
//         }
//
//         protected override (List<RectTransform>, List<TweenerCore<Vector3, Vector3, VectorOptions>>) ImplementTween(int preSelectedDataIndex, TweenCallback completeCallback)
//         {
//             List<RectTransform> resultTransforms = ListPool<RectTransform>.Get();
//             List<TweenerCore<Vector3, Vector3, VectorOptions>> resultTweenerCores = ListPool<TweenerCore<Vector3, Vector3, VectorOptions>>.Get();
//             for (int i = 0; i < linearSelectorCore.ItemsTra.Count; i++)
//             {
//                 RectTransform item = linearSelectorCore.ItemsTra[i];
//                 float duration = durationParam.GetValue(linearSelectorCore, i, preSelectedDataIndex);
//                 TweenerCore<Vector3, Vector3, VectorOptions> tweenLocalMove 
//                     = item.DOLocalMove(targetParam.PositionParam.GetValue(linearSelectorCore, i, preSelectedDataIndex), duration)
//                         .SetEase(targetParam.PositionParam.GetEase());
//                 TweenerCore<Vector3, Vector3, VectorOptions> tweenLocalScale
//                     = item.DOScale(targetParam.ScaleParam.GetValue(linearSelectorCore, i, preSelectedDataIndex), duration)
//                         .SetEase(targetParam.ScaleParam.GetEase());
//                 
//                 resultTransforms.Add(item);
//                 resultTweenerCores.Add(tweenLocalMove);
//                 resultTweenerCores.Add(tweenLocalScale);
//             }
//             float maxDuration = durationParam.GetMaxValue(linearSelectorCore, preSelectedDataIndex);
//             TweenerCore<Vector3, Vector3, VectorOptions> tweenerComplete = DOTween.To(()=> default(Vector3), _ => { }, Vector3.zero, maxDuration);
//             tweenerComplete.OnComplete(completeCallback);
//             resultTweenerCores.Add(tweenerComplete);
//             return (resultTransforms, resultTweenerCores);
//         }
//
//         public class InitialParam
//         {
//             // 坐标
//             public PositionParam PositionParam;
//             // 缩放
//             public ScaleParam ScaleParam;
//         }
//
//         public class TargetParam
//         {
//             // 坐标
//             public PositionParam PositionParam;
//             // 缩放
//             public ScaleParam ScaleParam;
//         }
//     }
// }
