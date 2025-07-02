using UnityEngine;

namespace Status.Transform
{
    public interface ITransformFieldStatus
    {
        TransformFieldStatus Release();
        
        TransformFieldStatus SetWorldValue(Vector3 worldValue);
        TransformFieldStatus SetLocalValue(Vector3 localValue);
        TransformFieldStatus SetValue(Vector3 value, TransformFieldStatus.ERelative eRelative);
        bool TryGetValue(out Vector3 value, out TransformFieldStatus.ERelative eRelative);
    }
    
    public class TransformFieldStatus: ITransformFieldStatus
    {
        /// 相对类型
        public enum ERelative
        {
            World = 1,
            Local = 2,
        }
        
        private Vector3 value;
        private ERelative eRelative;

        public TransformFieldStatus Release()
        {
            value = Vector3.zero;
            eRelative = default;
            return this;
        }

        public TransformFieldStatus SetWorldValue(Vector3 worldValue) => SetValue(worldValue, ERelative.World);
        public TransformFieldStatus SetLocalValue(Vector3 localValue) => SetValue(localValue, ERelative.Local);
        public TransformFieldStatus SetValue(Vector3 value, ERelative eRelative)
        {
            this.value = value;
            this.eRelative = eRelative;
            return this;
        }

        public bool TryGetValue(out Vector3 value, out ERelative eRelative)
        { 
            if (this.eRelative == default)
            {
                value = Vector3.zero;
                eRelative = default;
                return false;
            }
            value = this.value;
            eRelative = this.eRelative;
            return true;
        }
    }
}
