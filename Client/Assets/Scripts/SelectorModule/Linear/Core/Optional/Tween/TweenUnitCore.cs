using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Pool;

namespace Selector.Linear.Core.OptionalTween
{
    // 动画 单元
    [DisallowMultipleComponent]
    public abstract class TweenUnitCore: ITweenUnitCore
    {
        // 跳过动画
        private bool skipTween;
        // 允许动画
        private bool allowBreak;
        // 打断后的状态
        private EAfterBreakStatus eAfterBreakStatus;

        protected LinearSelectorCore linearSelectorCore;
        private TweenCallback beforeCallback;
        private TweenCallback completeCallback;
        
        // 动画
        private List<RectTransform> tweenerTransforms;
        private List<TweenerCore<Vector3, Vector3, VectorOptions>> tweenerCores;

        public virtual ITweenUnitCore Init(bool skipTween, bool allowBreak, EAfterBreakStatus eAfterBreakStatus,
            LinearSelectorCore linearSelectorCore, TweenCallback beforeCallback, TweenCallback completeCallback)
        {
            
            this.linearSelectorCore = linearSelectorCore;
            this.beforeCallback = beforeCallback;
            this.completeCallback = completeCallback;
            return this;
        }

        /// 获取动画状态
        public virtual ETweenStatus TweenStatus()
        {
            if (skipTween)
            {
                return ETweenStatus.AllowRun;
            }
            ETweenStatus eTweenStatus = ETweenStatus.None;
            // 判断之前的动画是否完成
            for (int i = 0; i < tweenerCores?.Count; i++)
            {
                if (tweenerCores[i].IsActive() && !tweenerCores[i].IsComplete())
                {
                    eTweenStatus |= ETweenStatus.Running;
                    break;
                }
            }
            if (eTweenStatus == ETweenStatus.Running)
            {
                if (allowBreak)
                {
                    eTweenStatus |= ETweenStatus.AllowBreak;
                    eTweenStatus |= ETweenStatus.AllowRun;
                }
                return eTweenStatus;
            }
            eTweenStatus |= ETweenStatus.AllowRun;
            return eTweenStatus;
        }

        /// 打断 上次 运行中的 状态
        public virtual void BreakLastRunningTween(ETweenStatus eTweenStatus)
        {
            // 无动画 不需要 打断
            if (skipTween) return;
            
            // 播放中，可打断 处理
            if(eTweenStatus.HasFlag(ETweenStatus.AllowBreak))
            {
                // 允许打断，打断处理
                AfterBreakTweenHandle();
            }
        }

        /// 运行 动画 前
        public virtual void BeforeRunTween()
        {
            BeforeRunTweenBeforeExtraCallback();
            beforeCallback.Invoke();
        }
        protected virtual void BeforeRunTweenBeforeExtraCallback()
        {
            
        }

        /// 运行 动画
        public virtual void RunTween()
        {
            // 无动画
            {
                if (skipTween)
                {
                    beforeCallback.Invoke();
                    // 执行新动画，直接完成
                    (List<RectTransform> tempTransforms, List<TweenerCore<Vector3, Vector3, VectorOptions>> tempTweenerCores) 
                        = ImplementTween(linearSelectorCore.PreSelectedDataIndex, completeCallback);
                    foreach (TweenerCore<Vector3, Vector3, VectorOptions> tweenerCore in tempTweenerCores)
                    {
                        tweenerCore.Kill(true);
                    }
                    ListPool<RectTransform>.Release(tempTransforms);
                    ListPool<TweenerCore<Vector3, Vector3, VectorOptions>>.Release(tempTweenerCores);
                    return;
                }
            }
            // 播放动画
            {
                if (tweenerTransforms != null)
                {
                    ListPool<RectTransform>.Release(tweenerTransforms);
                    tweenerTransforms = null;
                }
                if (tweenerCores != null)
                {
                    ListPool<TweenerCore<Vector3, Vector3, VectorOptions>>.Release(tweenerCores);
                    tweenerCores = null;
                }
                (tweenerTransforms, tweenerCores) = ImplementTween(linearSelectorCore.PreSelectedDataIndex, completeCallback);
            }
        }
        protected abstract (List<RectTransform>, List<TweenerCore<Vector3, Vector3, VectorOptions>>) ImplementTween(int preSelectedDataIndex, TweenCallback completeCallback);

        /// 打断处理
        private void AfterBreakTweenHandle()
        {
            if (tweenerCores == null)
            {
                return;
            }
            switch (eAfterBreakStatus)
            {
                case EAfterBreakStatus.Keep:
                {
                    for (int i = 0; i < tweenerCores.Count; i++)
                    {
                        tweenerCores[i].Kill();
                    }
                    break;
                }
                case EAfterBreakStatus.Restore:
                {
                    for (int i = 0; i < tweenerCores.Count; i++)
                    {
                        tweenerCores[i].Kill();
                    }
                    // 执行新动画，直接完成
                    (List<RectTransform> tempTransforms, List<TweenerCore<Vector3, Vector3, VectorOptions>> tempTweenerCores) 
                        = ImplementTween(linearSelectorCore.CurrSelectedDataIndex, null);
                    foreach (TweenerCore<Vector3, Vector3, VectorOptions> tweenerCore in tempTweenerCores)
                    {
                        tweenerCore.Kill(true);
                    }
                    ListPool<RectTransform>.Release(tempTransforms);
                    ListPool<TweenerCore<Vector3, Vector3, VectorOptions>>.Release(tempTweenerCores);
                    break;
                }
                case EAfterBreakStatus.Complete:
                {
                    for (int i = 0; i < tweenerCores.Count; i++)
                    {
                        tweenerCores[i].Kill(true);
                    }
                    break;
                }
            }
        }
    }
}
