using UnityEngine;
using UnityEngine.Pool;

namespace Status.Transform
{
    public interface ITransformStatus
    {
        TransformStatus Release();
        
        void SetTransform(UnityEngine.Transform transform, TransformFieldStatus.ERelative eRelative);
        
        TransformStatus SetPosition(Vector3 position);
        TransformStatus SetLocalPosition(Vector3 localPosition);
        TransformStatus SetPosition(Vector3 position, TransformFieldStatus.ERelative eRelative);
        bool TryGetPosition(out Vector3 position, out TransformFieldStatus.ERelative eRelative);
        
        TransformStatus SetEulerAngles(Vector3 eulerAngles);
        TransformStatus SetLocalEulerAngles(Vector3 localEulerAngles);
        TransformStatus SetEulerAngles(Vector3 eulerAngles, TransformFieldStatus.ERelative eRelative);
        bool TryGetEulerAngles(out Vector3 eulerAngles, out TransformFieldStatus.ERelative eRelative);
        
        TransformStatus SetLossyScale(Vector3 lossyScale);
        TransformStatus SetLocalScale(Vector3 localScale);
        TransformStatus SetScale(Vector3 scale, TransformFieldStatus.ERelative eRelative);
        bool TryGetScale(out Vector3 scale, out TransformFieldStatus.ERelative eRelative);
    }
    
    public class TransformStatus: ITransformStatus
    {
        // 位置
        private TransformFieldStatus positionStatus;

        // 欧拉角
        private TransformFieldStatus eulerAnglesStatus;

        // 缩放
        private TransformFieldStatus scaleStatus;

        public TransformStatus Release()
        {
            positionStatus?.Release();
            eulerAnglesStatus?.Release();
            scaleStatus?.Release();
            return this;
        }
        
        public void SetTransform(UnityEngine.Transform transform, TransformFieldStatus.ERelative eRelative)
        {
            positionStatus ??= GenericPool<TransformFieldStatus>.Get();
            eulerAnglesStatus ??= GenericPool<TransformFieldStatus>.Get();
            scaleStatus ??= GenericPool<TransformFieldStatus>.Get();
            if (eRelative == TransformFieldStatus.ERelative.World)
            {
                positionStatus.SetWorldValue(transform.position);
                eulerAnglesStatus.SetWorldValue(transform.eulerAngles);
                scaleStatus.SetWorldValue(transform.lossyScale);
            }
            else
            {
                positionStatus.SetLocalValue(transform.localPosition);
                eulerAnglesStatus.SetLocalValue(transform.localEulerAngles);
                scaleStatus.SetLocalValue(transform.localScale);
            }
        }

        public TransformStatus SetPosition(Vector3 position) => SetPosition(position, TransformFieldStatus.ERelative.World);
        public TransformStatus SetLocalPosition(Vector3 localPosition) => SetPosition(localPosition, TransformFieldStatus.ERelative.Local);
        public TransformStatus SetPosition(Vector3 position, TransformFieldStatus.ERelative eRelative)
        {
            positionStatus ??= GenericPool<TransformFieldStatus>.Get();
            positionStatus.SetValue(position, eRelative);
            return this;
        }
        public bool TryGetPosition(out Vector3 position, out TransformFieldStatus.ERelative eRelative)
        {
            if (positionStatus == null)
            {
                position = Vector3.zero;
                eRelative = default;
                return false;
            }
            return positionStatus.TryGetValue(out position, out eRelative);
        }
        
        public TransformStatus SetEulerAngles(Vector3 eulerAngles) => SetEulerAngles(eulerAngles, TransformFieldStatus.ERelative.World);
        public TransformStatus SetLocalEulerAngles(Vector3 localEulerAngles) => SetEulerAngles(localEulerAngles, TransformFieldStatus.ERelative.Local);
        public TransformStatus SetEulerAngles(Vector3 eulerAngles, TransformFieldStatus.ERelative eRelative)
        {
            eulerAnglesStatus ??= GenericPool<TransformFieldStatus>.Get();
            eulerAnglesStatus.SetValue(eulerAngles, eRelative);
            return this;
        }
        public bool TryGetEulerAngles(out Vector3 eulerAngles, out TransformFieldStatus.ERelative eRelative)
        {
            if (eulerAnglesStatus == null)
            {
                eulerAngles = Vector3.zero;
                eRelative = default;
                return false;
            }
            return eulerAnglesStatus.TryGetValue(out eulerAngles, out eRelative);
        }
        
        public TransformStatus SetLossyScale(Vector3 lossyScale) => SetScale(lossyScale, TransformFieldStatus.ERelative.World);
        public TransformStatus SetLocalScale(Vector3 localScale) => SetScale(localScale, TransformFieldStatus.ERelative.Local);
        public TransformStatus SetScale(Vector3 scale, TransformFieldStatus.ERelative eRelative)
        {
            scaleStatus ??= GenericPool<TransformFieldStatus>.Get();
            scaleStatus.SetValue(scale, eRelative);
            return this;
        }
        public bool TryGetScale(out Vector3 scale, out TransformFieldStatus.ERelative eRelative)
        {
            if (scaleStatus == null)
            {
                scale = Vector3.zero;
                eRelative = default;
                return false;
            }
            return scaleStatus.TryGetValue(out scale, out eRelative);
        }
    }
}
