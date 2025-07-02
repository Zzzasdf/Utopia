using System;
using UnityEngine.UI;

namespace Selector.Linear.Core.OptionalPart
{
    public class BtnClickMoveSelectedTriggerCore: IBtnClickMoveSelectedTriggerCore
    {
        private Button btnTrigger;
        private int timer;
        private Action<int> onMoveTimer;

        public IBtnClickMoveSelectedTriggerCore Init(Button btnTrigger, int timer)
        {
            this.btnTrigger = btnTrigger;
            this.timer = timer;
            return this;
        }

        void IBtnClickMoveSelectedTriggerCore.Subscribe(Action<int> onMoveTimer)
        {
            this.onMoveTimer = onMoveTimer;
            btnTrigger.onClick.RemoveAllListeners();
            btnTrigger.onClick.AddListener(OnTriggerClick);
            return;
            
            void OnTriggerClick()
            {
                this.onMoveTimer.Invoke(timer);
            }
        }
        
        void IBtnClickMoveSelectedTriggerCore.DisplayStatus(int index, int count)
        {
            btnTrigger.gameObject.SetActive(timer switch
            {
                < 0 => index > 0 && index < count,
                > 0 => index >= 0 && index < count - 1,
                _ => false
            });
        }
    }
}