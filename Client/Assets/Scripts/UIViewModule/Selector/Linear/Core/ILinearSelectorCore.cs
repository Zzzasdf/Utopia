using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Selector.Linear.Core
{
    #region Selector

    public interface ILinearSelectorCore
    {
        int CurrSelectedDataIndex { get; }
        void SetSelectedDataIndex(int selectedDataIndex);
        void AddRangeItems(List<GameObject> items);
        void AddRangeItems(List<ILinearSelectorItemCore> items);
        void SetCustomDataCount(int dataCount);
        void Subscribe(Action<int, RectTransform> onItemRender, Action<int> onItemSelected);
        void Subscribe(Action<int, RectTransform> onItemRender, Action<int> onItemSelected, Func<int, bool> onDisableCondition);
    }
    #endregion

    #region Item
    public interface ILinearSelectorItemCore
    {
        ILinearSelectorItemCore Init(RectTransform thisRect, Button btnArea, GameObject objSelectedEffect);
        RectTransform GetRectTransform();
        void Subscribe(Action<int, RectTransform> onItemRender, Action<int> onItemSelected);
        void RefreshRender(int index);
        void SetSelectedEffect(bool isActive);
    }
    #endregion

    #region IndexParam
    public interface IItemDataIndexParamCore
    {
        /// 两个数据索引 是否 指向 同块 item
        bool IsSameItem(IsSameItemParam param);
        /// 设置显示 item 的 起始数据索引
        int SetDisplayStartDataIndex(SetDisplayStartDataIndexParam param);
            
        public struct IsSameItemParam
        {
            public int ItemCount;
            public int DataCount;
            public int LeftSelectedDataIndex;
            public int RightSelectedDataIndex;
        }

        public struct SetDisplayStartDataIndexParam
        {
            public int ItemCount;
            public int DataCount;
            public int SelectedDataIndex;
        }
    }
    #endregion
}