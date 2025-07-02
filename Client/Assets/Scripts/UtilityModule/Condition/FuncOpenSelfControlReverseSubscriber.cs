using System;
using UnityEngine;

namespace ConditionModule
{
    public interface IFuncOpenSelfControlReverseSubscriber
    {
        // 输出 更新重检方法
        void OutSubscribeUpdate(out Action<bool> subscribeUpdate);
    }
    
    [RequireComponent(typeof(IFuncOpenSelfControlReverseSubscriber))]
    [DisallowMultipleComponent]
    public class FuncOpenSelfControlReverseSubscriber : MonoBehaviour
    {
        [SerializeField] private EFuncOpenId eFuncOpenId;
        private Action<bool> subscribeUpdate;
        
        private void Awake()
        {
            var reverseSubscriber = GetComponent<IFuncOpenSelfControlReverseSubscriber>();
            reverseSubscriber.OutSubscribeUpdate(out subscribeUpdate);
            GameEntry.ConditionManager.Subscribe(eFuncOpenId.GetCondition(), SubscribeUpdate);
        }
        private void OnDestroy()
        {
            GameEntry.ConditionManager.Unsubscribe(eFuncOpenId.GetCondition(), SubscribeUpdate);
        }

        private bool Status() => GameEntry.ConditionManager.Status(eFuncOpenId.GetCondition());
        private void SubscribeUpdate(bool status) => subscribeUpdate?.Invoke(status);
    }
}
