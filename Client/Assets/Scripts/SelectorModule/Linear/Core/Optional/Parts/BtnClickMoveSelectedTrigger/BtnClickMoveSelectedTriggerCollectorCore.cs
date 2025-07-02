using System;
using System.Collections.Generic;

namespace Selector.Linear.Core.OptionalPart
{
    public class BtnClickMoveSelectedTriggerCollectorCore: IBtnClickMoveSelectedTriggerCollectorCore
    {
        private List<IBtnClickMoveSelectedTriggerCore> itemTriggers;

        public IBtnClickMoveSelectedTriggerCollectorCore Init(List<IBtnClickMoveSelectedTriggerCore> itemTriggers)
        {
            this.itemTriggers = itemTriggers;
            return this;
        }

        void IBtnClickMoveSelectedTriggerCollectorCore.Subscribe(Action<int> onMoveTimer)
        {
            foreach (IBtnClickMoveSelectedTriggerCore t in itemTriggers)
            {
                t.Subscribe(onMoveTimer);
            }
        }

        void IBtnClickMoveSelectedTriggerCollectorCore.DisplayStatus(int index, int count)
        {
            foreach (IBtnClickMoveSelectedTriggerCore t in itemTriggers)
            {
                t.DisplayStatus(index, count);
            }
        }
    }
}