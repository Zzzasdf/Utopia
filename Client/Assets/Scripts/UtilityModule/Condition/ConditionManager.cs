using System;
using System.Collections.Generic;

namespace ConditionModule
{
    public enum EConditionType
    {
        Test1 = 1,
    }
    public interface ICondition<T>
        where T: struct
    {
        void GetConditionTypes(in ICollection<T> types);
        bool CheckCondition();
    }
    
    public class ConditionManager: IManager
    {
        /// 条件响应器
        private ConditionResponder<EConditionType> commonResponder;
        // 单线程仅需一个缓冲器
        private DelayBuffer<EConditionType> delayBuffer;
        
        void IInit.OnInit()
        {
            commonResponder = new ConditionResponder<EConditionType>();
            delayBuffer = new DelayBuffer<EConditionType>(10, commonResponder.Fire, commonResponder.Fire, commonResponder.Fire);
        }

        void IReset.OnReset()
        {
            delayBuffer.Reset();
            commonResponder.Clear();
        }

        void IDestroy.OnDestroy()
        {
            delayBuffer.Destroy();
            delayBuffer = null;
            commonResponder.Clear();
            commonResponder = null;
        }

        public void Subscribe(ICondition<EConditionType> types, Action<bool> callback) => commonResponder.Subscribe(types, callback);
        public void Unsubscribe(ICondition<EConditionType> types, Action<bool> callback) => commonResponder.Unsubscribe(types, callback);
        public bool Status(ICondition<EConditionType> types) => commonResponder.Status(types);
        
        public void Fire(EConditionType eConditionType) => delayBuffer.FireType(eConditionType);
        public void Fire() => delayBuffer.FireAllType();
        
        public void FireNow(EConditionType eConditionType) => delayBuffer.FireTypeNow(eConditionType);
        public void FireNow() => delayBuffer.FireAllTypeNow();
    }
    
    public enum EFuncOpenId
    {
        Test1 = 1,
        Test2 = 2,
    }
    public static class EFuncOpenIdExtensions
    {
        public static ICondition<EConditionType> GetCondition(this EFuncOpenId eFuncOpenId)
        {
            // TODO ZZZ
            throw new NotImplementedException();
        }
    }
}