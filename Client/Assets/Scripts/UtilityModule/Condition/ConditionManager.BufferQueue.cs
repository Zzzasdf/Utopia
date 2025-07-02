using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConditionModule
{
    public class BufferQueue
    {
        private bool collecting;
        private bool fireAll;
        private HashSet<EConditionType> fireTypes;
        
        private Action fireAllFunc;
        private Action<EConditionType> fireTypeFunc;

        public BufferQueue(Action fireAllFunc, Action<EConditionType> fireTypeFunc)
        {
            collecting = false;
            fireAll = false;
            fireTypes = new HashSet<EConditionType>();
            this.fireAllFunc = fireAllFunc;
            this.fireTypeFunc = fireTypeFunc;
        }
        public void Clear()
        {
            fireTypes.Clear();
            fireAll = false;
            collecting = false;
        }
        public void AddFireType(EConditionType eConditionType)
        {
            fireTypes.Add(eConditionType);
            if (collecting) return;
            collecting = !collecting;
            OnCollectHandleAsync();
        }

        public void SetFireAll()
        {
            fireAll = true;
            if (collecting) return;
            collecting = !collecting;
            OnCollectHandleAsync();
        }

        private async void OnCollectHandleAsync()
        {
            // 收集 0.1秒 内需要推送的数据类型，统一处理
            // Unity 默认回到原线程
            await Task.Delay(TimeSpan.FromSeconds(0.1));
            if (fireAll)
            {
                fireAllFunc.Invoke();
            }
            else
            {
                foreach (var eConditionType in fireTypes)
                {
                    fireTypeFunc.Invoke(eConditionType);
                }
            }
            Clear();
        }
    }
}