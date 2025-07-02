using System;
using UnityEngine;
using UnityEngine.UI;

namespace Selector.Linear.Core
{
    public class LinearSelectorItemCore: ILinearSelectorItemCore
    {
        private RectTransform thisRect;
        private Button btnArea;
        private GameObject objSelectedEffect;
        private RectTransform rectTransform;
        
        private Action<int, RectTransform> onItemRender;
        private Action<int> onItemSelected;
        private int index;

        public ILinearSelectorItemCore Init(RectTransform thisRect, Button btnArea, GameObject objSelectedEffect)
        {
            this.btnArea = btnArea;
            this.objSelectedEffect = objSelectedEffect;
            this.rectTransform = btnArea.GetComponent<RectTransform>();
            return this;
        }

        public RectTransform GetRectTransform() => thisRect;

        void ILinearSelectorItemCore.Subscribe(Action<int, RectTransform> onItemRender, Action<int> onItemSelected)
        {
            this.onItemRender = onItemRender;
            this.onItemSelected = onItemSelected;
            btnArea.onClick.RemoveAllListeners();
            btnArea.onClick.AddListener(OnBtnAreaClick);
            return;

            void OnBtnAreaClick()
            {
                this.onItemSelected?.Invoke(index);
            }
        }
    
        void ILinearSelectorItemCore.RefreshRender(int index)
        {
            this.index = index;
            onItemRender?.Invoke(index, rectTransform);
        }
    
        void ILinearSelectorItemCore.SetSelectedEffect(bool isActive)
        {
            objSelectedEffect.gameObject.SetActive(isActive);
        }
    }
}