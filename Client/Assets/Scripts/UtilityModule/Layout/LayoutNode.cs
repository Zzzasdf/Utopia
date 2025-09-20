using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[DisallowMultipleComponent]
public abstract class LayoutNode: MonoBehaviour 
{
    [SerializeField] private LayoutNode _parentNode;
    protected abstract List<LayoutNode> _childrenNodes { get; }
    
    private bool _isDirty;
    
    private int? timerId;
    /// 缓存委托，直接传递方法有 gc
    private Action<bool> timerCallback;

    private void Awake()
    {
        timerCallback = DelayLayoutAsync;
    }

#region Delay
    /// <summary>
    /// 延迟更新，收集一帧内的所有变动，一次更新
    /// </summary>
    public void SetLayout()
    {
        SetDirtyChildren();
        SetDirtyParent();
    }
    private void SetDirtyChildren()
    {
        for (int i = 0; i < _childrenNodes?.Count; i++)
        {
            _childrenNodes[i].SetDirtyChildren();
            _childrenNodes[i]._isDirty = true;
        }
    }
    private void SetDirtyParent()
    {
        // 当前节点若已被标记为 dirty，说明该节点往上的节点都已被标记过，无需进一步操作
        if (_isDirty) return;
        _isDirty = true;
        if (_parentNode != null)
        {
            _parentNode.SetDirtyParent();
        }
        else
        {
            // 在根节点执行定时器
            timerId = GameEntry.TimerManager.SetAfterMilliseconds(10, timerCallback);
        }
    }
    private void DelayLayoutAsync(bool isSuccess)
    {
        SetDirtyLayout();
        SetSelfLayoutBuiltIn();
        timerId = null;
    }
    private void SetDirtyLayout()
    {
        for (int i = 0; i < _childrenNodes?.Count; i++)
        {
            _childrenNodes[i].SetDirtyLayout();
            if (!_childrenNodes[i]._isDirty) continue;
            _childrenNodes[i].SetSelfLayoutBuiltIn();
        }
    }
#endregion

#region Immediately
    public void SetLayoutNow()
    {
        SetAllChildrenLayout();
        SetParentLayout();
    }
    // 子节点更新，只向下调用 子节点的布局，不往上调用
    private void SetAllChildrenLayout()
    {
        for (int i = 0; i < _childrenNodes?.Count; i++)
        {
            _childrenNodes[i].SetAllChildrenLayout();
            _childrenNodes[i].SetSelfLayoutBuiltIn();
        }
    }
    // 父节点更新，只向上调用 父节点的布局，不往下调用，保证只更新一条确定的分支
    private void SetParentLayout()
    {
        SetSelfLayoutBuiltIn();
        if (_parentNode == null)
        {
            if (timerId != null)
            {
                GameEntry.TimerManager.Cancel(timerId.Value);
            }
            return;
        }
        _parentNode.SetParentLayout();
    }
#endregion
    private void SetSelfLayoutBuiltIn()
    {
        SetSelfLayout();
        _isDirty = false;
    }
    protected abstract void SetSelfLayout();
}
