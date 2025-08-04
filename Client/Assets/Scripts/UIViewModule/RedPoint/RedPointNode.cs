using System;
using System.Collections.Generic;

public class RedPointNode
{
    // 当前节点
    private readonly ERedPoint current;
    private Func<bool> condition;
    
    // 归属树
    private ERedPointTree belongToTree;
    
    // 父子节点
    private RedPointNode parent;
    private HashSet<RedPointNode> children;

    public RedPointNode(ERedPoint current, ERedPointTree belongToTree)
    {
        this.current = current;
        this.belongToTree = belongToTree;
    }

    /// 添加父节点
    public void AddParentNode(RedPointNode redPointNode)
    {
        parent = redPointNode;
    }
    /// 添加子节点
    public void AddChildNode(RedPointNode nodeChild)
    {
        children ??= new HashSet<RedPointNode>();
        children.Add(nodeChild);
    }
    /// 添加显示的条件
    public void AddCondition(Func<bool> condition)
    {
        this.condition = condition;
    }
}
