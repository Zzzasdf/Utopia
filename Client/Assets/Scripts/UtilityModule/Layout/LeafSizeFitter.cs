using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 叶子节点适配器
/// </summary>
public partial class LeafSizeFitter: LayoutNode
{
    [System.NonSerialized] private RectTransform m_Rect;
    private RectTransform rectTransform
    {
        get
        {
            if (m_Rect == null)
                m_Rect = GetComponent<RectTransform>();
            return m_Rect;
        }
    }
    protected override List<LayoutNode> _childrenNodes => null;

    [SerializeField] private bool setNativeSizeX = true;
    [SerializeField] private bool setNativeSizeY = true;

    protected override void SetSelfLayout()
    {
        Layout.SetLeafSize(rectTransform, setNativeSizeX, setNativeSizeY);
    }
}
