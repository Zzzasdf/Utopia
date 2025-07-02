/********************************************
 * 支持绑定复数父节点，复数子节点
 *
 *
 *
 * 
*********************************************/

using System;
using System.Collections.Generic;

public class RedPointNode
{
    private readonly ERedPoint current;
    /// 被连接的条件
    private Func<bool> beConnectedCondition;
    /// 显示的条件
    private Func<bool> condition;
    
    private HashSet<ERedPoint> parents;
    private HashSet<ERedPoint> children;

    public RedPointNode(ERedPoint current)
    {
        this.current = current;
    }

    /// 添加被连接的条件
    public void AddBeConnectedCondition(Func<bool> beConnectedCondition)
    {
        this.beConnectedCondition = beConnectedCondition;
    }
    /// 添加显示的条件
    public void AddCondition(Func<bool> condition)
    {
        this.condition = condition;
    }

    /// 添加父节点
    public void AddParentNode(ERedPoint parentNode)
    {
        parents ??= new HashSet<ERedPoint>();
        parents.Add(parentNode);
    }
    /// 添加子节点
    public void AddChildNode(ERedPoint childNode)
    {
        children ??= new HashSet<ERedPoint>();
        children.Add(childNode);
    }
}
