using System;
using UnityEngine;

namespace Selector.Linear.Core
{
    public partial class LinearSelectorCore
    {
        private bool dirtySubscribe;
        // 渲染订阅
        private Action<int, RectTransform> onItemRender;
        // 选中订阅
        private Action<int> onItemSelected;
        // 禁用订阅
        private Func<int, bool> onDisableCondition;
        
        public void Subscribe(Action<int, RectTransform> onItemRender, Action<int> onItemSelected)
        {
            Subscribe(onItemRender, onItemSelected, null);
        }
        public void Subscribe(Action<int, RectTransform> onItemRender, Action<int> onItemSelected, Func<int, bool> onDisableCondition)
        {
            this.onItemRender = onItemRender;
            this.onItemSelected = onItemSelected;
            this.onDisableCondition = onDisableCondition;
            this.dirtySubscribe = true;
        }

        /// 是否 禁用
        private bool DisableCondition(int selectedDataIndex)
        {
            return onDisableCondition?.Invoke(selectedDataIndex) == true;
        }

        /// 触发 选中 回调
        private void OnItemSelected()
        {
            onItemSelected?.Invoke(CurrSelectedDataIndex);
        }

        /// 初始化 订阅
        private void InitItemsRenderSubscribe()
        {
            if (!dirtySubscribe)
            {
                return;
            }
            dirtySubscribe = !dirtySubscribe;
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].Subscribe(onItemRender, SetSelectedDataIndex);
            }
        }
    }
}