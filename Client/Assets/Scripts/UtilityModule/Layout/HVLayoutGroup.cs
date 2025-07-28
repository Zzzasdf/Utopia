using System.Collections.Generic;
using UnityEngine;

public partial class HVLayoutGroup: LayoutNode
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
    protected override List<LayoutNode> _childrenNodes => childrenNodes;
    
    [SerializeField] private List<LayoutNode> childrenNodes;
    [SerializeField] private Layout.ELayout eLayout;
    [SerializeField] private Layout.Padding padding;
    [SerializeField] private float spacing;
    [SerializeField] private TextAnchor childAlignment;
    [SerializeField] private bool reverseArrangement;
    [SerializeField] private bool setNativeSizeX = true;
    [SerializeField] private bool setNativeSizeY = true;

    public float Spacing => spacing;
    public Layout.Padding Padding => padding;
    
    // 更新自己
    protected override void SetSelfLayout()
    {
        Vector2 size =
            Layout.SetChildrenLayout(rectTransform, childAlignment, 
                eLayout, reverseArrangement ? Layout.EDirection.Reverse : Layout.EDirection.Forward,
                spacing, padding);
        bool isSetNativeSize = setNativeSizeX || setNativeSizeY;
        if (isSetNativeSize)
        {
            if (!setNativeSizeX) size.x = rectTransform.sizeDelta.x;
            if (!setNativeSizeY) size.y = rectTransform.sizeDelta.y;
            rectTransform.sizeDelta = size;
        }
    }
}
