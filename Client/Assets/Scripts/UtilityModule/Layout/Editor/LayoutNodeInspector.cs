/*=================================================
*FileName:      LayoutNodeInspector.cs 
*Author:        LiShaoWei 
*UnityVersion:  2021.2.18f1 
*Date:          2025-07-22 14:30 
*Description:   
*History:       
=================================================*/
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LayoutNode), true)]
public class LayoutNodeInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        LayoutNode layoutNode = (LayoutNode)target;
        if (GUILayout.Button("一键绑定"))
        {
            SetBindingRelationship(layoutNode);
            EditorUtility.SetDirty(target);
        }
        if (GUILayout.Button("Set Layout"))
        {
            layoutNode.SetLayoutNow();
            EditorUtility.SetDirty(target);
        }
    }
    private void SetBindingRelationship(LayoutNode layoutNode)
    {
        // 先向上找到最初的节点
        LayoutNode initialLayoutNode = layoutNode;
        while (initialLayoutNode.transform.parent.TryGetComponent(out LayoutNode node))
        {
            initialLayoutNode = node;
        }
        // 递归赋值
        SetChildren(initialLayoutNode);
    }
    private void SetChildren(LayoutNode initialLayoutNode)
    {
        FieldInfo parentNodeField = typeof(LayoutNode).GetField("_parentNode", BindingFlags.NonPublic | BindingFlags.Instance);
        switch(initialLayoutNode)
        {
            case HVLayoutGroup: 
                {
                    FieldInfo childrenNodesField = typeof(HVLayoutGroup).GetField("childrenNodes", BindingFlags.NonPublic | BindingFlags.Instance);
                    Transform layoutGroupTra = initialLayoutNode.transform;
                    parentNodeField.SetValue(initialLayoutNode, null);
                    List<LayoutNode> layoutNodes = new List<LayoutNode>();
                    for (int i = 0; i < layoutGroupTra.childCount; i++)
                    {
                        Transform childTra = layoutGroupTra.GetChild(i);
                        if (!childTra.TryGetComponent(out LayoutNode layoutNode))
                        {
                            continue;
                        }
                        SetChildren(layoutNode);
                        layoutNodes.Add(layoutNode);
                        parentNodeField.SetValue(layoutNode, initialLayoutNode);
                    }
                    childrenNodesField.SetValue(initialLayoutNode, layoutNodes);
                } 
                break;
            case LeafSizeFitter: // 无子节点
                {
                    Transform layoutGroupTra = initialLayoutNode.transform;
                    for (int i = 0; i < layoutGroupTra.childCount; i++)
                    {
                        Transform childTra = layoutGroupTra.GetChild(i);
                        if (!childTra.TryGetComponent(out LayoutNode layoutNode))
                        {
                            continue;
                        }
                        Debug.LogError($"{layoutNode.name} 该节点无法被连接，叶子节点适配器必须是末端的节点!!");
                    }
                } 
                break;
        }
    }
}
