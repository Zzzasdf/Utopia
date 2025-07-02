using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Selector.Linear.Core.OptionalPart
{
    /// 按钮 点击 移动 触发器 收集器
    public interface IBtnClickMoveSelectedTriggerCollectorCore
    {
        IBtnClickMoveSelectedTriggerCollectorCore Init(List<IBtnClickMoveSelectedTriggerCore> itemTriggers);
        void Subscribe(Action<int> onMoveTimer);
        void DisplayStatus(int index, int count);
    }
    /// 按钮 点击 移动 触发器
    public interface IBtnClickMoveSelectedTriggerCore
    {
        IBtnClickMoveSelectedTriggerCore Init(Button btnTrigger, int timer);
        void Subscribe(Action<int> onMoveTimer);
        void DisplayStatus(int index, int count);
    }
}