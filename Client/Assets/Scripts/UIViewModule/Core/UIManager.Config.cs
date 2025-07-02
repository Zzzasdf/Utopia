using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public partial class UIManager
{
    private UIViewNodeCollector UIViewNodeCollector = new UIViewNodeCollector();
    public void ToString() => UIViewNodeCollector.ToString();
}

public class UIViewNodeCollector
{
    private Dictionary<Type, UIViewNode> nodeMap = new Dictionary<Type, UIViewNode>();
    private Dictionary<Type, UICheckerBase> checkerMap = new Dictionary<Type, UICheckerBase>();
    
    public UIViewNodeCollector()
    {
        Node<UIViewUnitTest, UICheckTest>().Linker(
            Node<UIViewUnitTest1, UICheckTest1>(), 
            Node<UIViewUnitTest2>().Linker(
                Node<UIViewUnitTest3, UICheckTest3>()));
    }

    private UIViewNode Node<TView>()
        where TView: UIViewUnitBase
    {
        Type type = typeof(TView);
        UIViewNode uiViewNode = new UIViewNode(type);
        nodeMap.Add(type, uiViewNode);
        return uiViewNode;
    }
    private UIViewNode Node<TView, TChecker>()
        where TView: UIViewUnitBase
        where TChecker: UICheckerBase, new()
    {
        UIViewNode uiViewNode = Node<TView>();
        TChecker checkerBase = new TChecker();
        checkerMap.Add(uiViewNode.CurrentNode, checkerBase);
        return uiViewNode;
    }

    public void ToString()
    {
        foreach (var pair in nodeMap)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{pair.Key}: ");
            sb.AppendLine($"Current => {pair.Value.CurrentNode} ");
            sb.AppendLine($"Parent => {pair.Value.ParentNode?.CurrentNode} ");
            for (int i = 0; i < pair.Value.SubNodes?.Count; i++)
            {
                sb.AppendLine($"Sub [{i}] => {pair.Value.SubNodes[i].CurrentNode} ");
            }
            Debug.LogWarning(sb.ToString());
        }
    }
}

public class UIViewNode
{
    public UIViewNode ParentNode { get; private set; } // 父节点
    public Type CurrentNode { get; private set; } // 当前节点
    public List<UIViewNode> SubNodes { get; private set; } // 子节点

    public UIViewNode(Type currentNode)
    {
        CurrentNode = currentNode;
    }
    
    public UIViewNode Linker(params UIViewNode[] subTypes)
    {
        SubNodes = new List<UIViewNode>(subTypes.Length);
        for (int i = 0; i < subTypes.Length; i++)
        {
            UIViewNode uiSubViewNode = subTypes[i];
            uiSubViewNode.ParentNode = this;
            SubNodes.Add(uiSubViewNode);
        }
        return this;
    }
}