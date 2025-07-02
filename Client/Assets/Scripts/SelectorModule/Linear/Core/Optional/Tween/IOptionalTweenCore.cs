using System;
using DG.Tweening;
using UnityEngine;

namespace Selector.Linear.Core.OptionalTween
{
    /// 动画
    public interface IOptionalTweenCore
    {
        EnterTweenUnitBaseCore EnterTweenUnitCore();
        SelectedTweenUnitBaseCore SelectedTweenUnitCore();
        ExitTweenUnitBaseCore ExitTweenUnitCore();
    }
    /// 进入动画
    [DisallowMultipleComponent]
    public abstract class EnterTweenUnitBaseCore : TweenUnitCore
    {
        
    }
    /// 选中动画
    [DisallowMultipleComponent]
    public abstract class SelectedTweenUnitBaseCore : TweenUnitCore
    {
        
    }
    /// 退出动画
    [DisallowMultipleComponent]
    public abstract class ExitTweenUnitBaseCore : TweenUnitCore
    {
        
    }
    // 动画单元
    public interface ITweenUnitCore
    {
        ITweenUnitCore Init(bool skipTween, bool allowBreak, EAfterBreakStatus eAfterBreakStatus,
            LinearSelectorCore linearSelectorCore, TweenCallback beforeCallback, TweenCallback completeCallback);

        ETweenStatus TweenStatus();
        void BreakLastRunningTween(ETweenStatus eTweenStatus);
        void BeforeRunTween();
        void RunTween();
    }
    
    /// 阶段
    public enum EStageStatus
    {
        // 进入
        Enter = 1 << 1,
        // 选中
        Selected = 1 << 2,
        // 退出
        Exit = 1 << 3,
    }
    /// 动画 状态
    [Flags]
    public enum ETweenStatus
    {
        None = 0,
        /// 运行中
        Running = 1 << 1,
        
        /// 是否允许执行
        AllowRun = 1 << 2,
        /// 是否要打断动画
        AllowBreak = 1 << 3 | Running,
    }
    /// 动画 使用 状态
    public enum ETweenUseStatus
    {
        /// 一次
        Once = 1,
        /// 可复用
        Reusable = 2,
    }
    /// 打断后的 状态
    public enum EAfterBreakStatus
    {
        /// 保持
        Keep = 0,
        /// 复原
        Restore = 1,
        /// 完成
        Complete = 2,
    }
}