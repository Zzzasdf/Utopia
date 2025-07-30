using System;
using System.Collections.Generic;

namespace ConditionModule
{
    public enum EConditionType
    {
        Test1 = 1,
    }
    public interface IConditionTypeGroup
    {
        public void GetConditionTypes(in IList<EConditionType> types)
        {
            
        }
    }
    
    public class ConditionManager: IManager
    {
        /// 条件响应器
        private ConditionResponder commonResponder;

        // 单线程同步仅需一个缓冲队列
        private BufferQueue bufferQueue;
        
        void IInit.OnInit()
        {
            commonResponder = new ConditionResponder();
            bufferQueue = new BufferQueue(commonResponder.Fire, commonResponder.Fire);
        }

        void IReset.OnReset()
        {
            bufferQueue.Clear();
            commonResponder.Clear();
        }

        void IDestroy.OnDestroy()
        {
            
        }

        public void Subscribe(IConditionTypeGroup key, Action<bool> callback) => commonResponder.Subscribe(key, callback);
        public void Unsubscribe(IConditionTypeGroup key, Action<bool> callback) => commonResponder.Unsubscribe(key, callback);
        public bool Status(IConditionTypeGroup key) => commonResponder.Status(key);
        
        public void Fire() => bufferQueue.SetFireAll();
        public void Fire(EConditionType eConditionType) => bufferQueue.AddFireType(eConditionType);
    }
    
    public enum EFuncOpenId
    {
        Test1 = 1,
        Test2 = 2,
    }
    public static class EFuncOpenIdExtensions
    {
        public static IConditionTypeGroup GetCondition(this EFuncOpenId eFuncOpenId)
        {
            // TODO ZZZ
            throw new NotImplementedException();
        }
    }
}