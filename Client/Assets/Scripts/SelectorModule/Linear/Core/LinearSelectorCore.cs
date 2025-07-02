using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;

namespace Selector.Linear.Core
{
    public partial class LinearSelectorCore: ILinearSelectorCore
    {
        // 索引计算器
        public IItemDataIndexParamCore ItemDataIndexParamCore;
        
        // item
        public List<RectTransform> ItemsTra;
        public List<ILinearSelectorItemCore> Items;
        
        // data
        public bool DirtyData;
        public int DataCount;
        public int DisplayLeftStartIndex;
        
        // 播放 进入动画前 选中
        public bool BeforeEnterTweenSelected;
        // 播放 选中动画前 选中
        public bool BeforeSelectedTweenSelected;

        /// 预选中的 数据选中索引
        public int PreSelectedDataIndex;
        /// 当前的 数据选中索引
        public int CurrSelectedDataIndex { get; set; }= -1;
        
        public bool EnterDirty;
        public TweenCallback setDirtyCallback;
        public TweenCallback beforeEnterExtraCallback;
        public TweenCallback afterEnterExtraCallback;
        public TweenCallback beforeSelectedExtraCallback;
        public TweenCallback selectedCallback;
        public TweenCallback afterSelectedExtraCallback;

        public Action<int> runSelectedCallback;
        
        public LinearSelectorCore(IItemDataIndexParamCore itemDataIndexParamCore, TweenCallback setDirtyCallback,
            TweenCallback beforeEnterExtraCallback, TweenCallback afterEnterExtraCallback, 
            TweenCallback beforeSelectedExtraCallback, TweenCallback selectedCallback, TweenCallback afterSelectedExtraCallback): this(itemDataIndexParamCore)
        {
            this.setDirtyCallback += setDirtyCallback;
            this.beforeEnterExtraCallback += beforeEnterExtraCallback;
            this.afterEnterExtraCallback += afterEnterExtraCallback;
            this.beforeSelectedExtraCallback += beforeSelectedExtraCallback;
            this.selectedCallback += selectedCallback;
            this.afterSelectedExtraCallback += afterSelectedExtraCallback;
        }
        
        public LinearSelectorCore(IItemDataIndexParamCore itemDataIndexParamCore)
        {
            ItemsTra = ListPool<RectTransform>.Get();
            Items = ListPool<ILinearSelectorItemCore>.Get();
            
            ItemDataIndexParamCore = itemDataIndexParamCore;

            setDirtyCallback += SetDirtyExtraCallback;

            beforeEnterExtraCallback += BeforeEnterExtraCallback;

            afterEnterExtraCallback += AfterEnterExtraCallback;

            beforeSelectedExtraCallback += BeforeSelectedExtraCallback;

            selectedCallback += ResponseSwitchSelectedIndex;
            selectedCallback += RefreshItemsRender;
            selectedCallback += OnItemSelected;
            selectedCallback += SetSelectedExtraCallback;

            afterSelectedExtraCallback += AfterSelectedExtraCallback;

            runSelectedCallback = RunSelectedCallback;
            return;
            
            void RunSelectedCallback(int afterAdjustSelectedDataIndex)
            {
                PreSelectedDataIndex = afterAdjustSelectedDataIndex;
                if (EnterDirty)
                {
                    EnterDirty = !EnterDirty;
                    beforeEnterExtraCallback.Invoke();
                    afterEnterExtraCallback.Invoke();
                }
                beforeSelectedExtraCallback.Invoke();
                selectedCallback.Invoke();
                afterSelectedExtraCallback.Invoke();
            }
        }

        /// 可支持大于 实际显示 的 item数量 数据
        public void SetCustomDataCount(int dataCount)
        {
            EnterDirty = true;
            setDirtyCallback.Invoke();
            SetCustomDataCount(dataCount, null);
        }


        public void SetSelectedDataIndex(int selectedDataIndex)
        {
            // 有效 范围内 检测 调整 选中索引
            if (!TryGetValidSelectedIndex(selectedDataIndex, out int afterAdjustSelectedDataIndex))
            {
                return;
            }
            
            // 禁用 条件 判断
            if (DisableCondition(afterAdjustSelectedDataIndex))
            {
                return;
            }
            
            // 选中
            {
                if (IsLastSelectedDataIndex(selectedDataIndex))
                {
                    return;
                }
                runSelectedCallback.Invoke(afterAdjustSelectedDataIndex);
            }
        }

        /// 回应 切换 选中
        private void ResponseSwitchSelectedIndex()
        {
            // 切换 选中
            CurrSelectedDataIndex = PreSelectedDataIndex;
        }
        
        /// 刷新 items
        private void RefreshItemsRender()
        {
            // 显示 data 的 起始 索引
            DisplayLeftStartIndex = SetDisplayStartDataIndex(CurrSelectedDataIndex);
            
            // 初始化 items 渲染订阅
            InitItemsRenderSubscribe();
            // 刷新
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].RefreshRender(DisplayLeftStartIndex + i);
            }
            SetSelectedEffect(DisplayLeftStartIndex);
            return;
            
            // 设置 选中 效果
            void SetSelectedEffect(int displayLeftStartIndex)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    Items[i].SetSelectedEffect(i + displayLeftStartIndex == CurrSelectedDataIndex);
                }
            }
        }

        public void AddRangeItems(List<GameObject> items)
        {
            Items.Clear();
            for (int i = 0; i < items.Count; i++)
            {
                GameObject item = items[i];
                if (!item.TryGetComponent(out ILinearSelectorItemCore selectorItem))
                {
                    Debug.LogError($"item 上需要 挂载 选中组件 {typeof(ILinearSelectorItemCore)}");
                    continue;
                }
                Items.Add(selectorItem);
            }
            AddRangeItems(Items);
        }
        public void AddRangeItems(List<ILinearSelectorItemCore> items)
        {
            Items = items;
            ItemsTra.Clear();
            for (int i = 0; i < items.Count; i++)
            {
                ILinearSelectorItemCore itemCore = items[i];
                RectTransform rectTransform = itemCore.GetRectTransform();
                ItemsTra.Add(rectTransform);
            }
        }
        
        /// 可支持大于 实际显示 的 item数量 数据
        public void SetCustomDataCount(int dataCount, Action resetCallback)
        {
            DataCount = dataCount;
            DirtyData = true;
            resetCallback?.Invoke();
        }
        
        /// 有效 范围 内 检测 调整 选中索引
        public bool TryGetValidSelectedIndex(int selectedIndex, out int afterAdjustSelectedIndex)
        {
            int tempSelectedIndex = Mathf.Clamp(selectedIndex, 0, DataCount - 1);
            if (tempSelectedIndex < 0 || tempSelectedIndex >= DataCount)
            {
                afterAdjustSelectedIndex = -1;
                return false;
            }
            afterAdjustSelectedIndex = tempSelectedIndex;
            return true;
        }
        
        /// 是否是 上次的 选中
        public bool IsLastSelectedDataIndex(int selectedDataIndex)
        {
            if (!DirtyData)
            {
                return selectedDataIndex == CurrSelectedDataIndex;
            }
            DirtyData = !DirtyData;
            return false;
        }

        /// 两个数据索引 是否 指向 同块 item
        public bool IsSameItem(int leftSelectedDataIndex, int rightSelectedDataIndex)
        {
            return ItemDataIndexParamCore.IsSameItem(new IItemDataIndexParamCore.IsSameItemParam
            {
                ItemCount = ItemsTra.Count,
                DataCount = DataCount,
                LeftSelectedDataIndex = leftSelectedDataIndex,
                RightSelectedDataIndex = rightSelectedDataIndex,
            });
        }

        /// 设置显示 item 的 起始数据索引
        public int SetDisplayStartDataIndex(int selectedDataIndex)
        {
            return ItemDataIndexParamCore.SetDisplayStartDataIndex(new IItemDataIndexParamCore.SetDisplayStartDataIndexParam
            {
                ItemCount = ItemsTra.Count,
                DataCount = DataCount,
                SelectedDataIndex = selectedDataIndex,
            });
        }
        
        /// Dirty 额外效果
        protected virtual void SetDirtyExtraCallback()
        {
            
        }
        
        /// 进入前的 额外效果
        protected virtual void BeforeEnterExtraCallback()
        {
            
        }
        /// 进入后的 额外效果
        protected virtual void AfterEnterExtraCallback()
        {
            
        }
        
        /// 选中前的 额外效果
        protected virtual void BeforeSelectedExtraCallback()
        {
            
        }
        
        /// 选中 额外效果
        protected virtual void SetSelectedExtraCallback()
        {
            
        }
        
        /// 选中后的 额外效果
        protected virtual void AfterSelectedExtraCallback()
        {
            
        }
    }
}