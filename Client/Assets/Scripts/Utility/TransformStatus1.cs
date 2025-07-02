// using System;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Pool;
//
// public class TranslateInfos
// {
//     private List<TranslateInfo> translateInfos;
//
//     public void Release()
//     {
//         UnityEngine.Pool.GenericPool<TranslateInfos>.Get();
//         if (translateInfos != null)
//         {
//             for (int i = 0; i < translateInfos.Count; i++)
//             {
//                 translateInfos[i].Release();
//             }
//             ListPool<TranslateInfo>.Release(translateInfos);
//         }
//     }
//     
//     public void Reset()
//     {
//         Release();
//         translateInfos = ListPool<TranslateInfo>.Get();
//     }
//     
//     public void SetInfos(List<(Transform transform, List<(EField eField, Vec3AnimationInfo animationInfo)>)> infos)
//     {
//         Reset();
//         for (int i = 0; i < infos.Count; i++)
//         {
//             (Transform transform, List<(EField eField, Vec3AnimationInfo animationInfo)> list) info = infos[i];
//             for (int j = 0; j < info.list.Count; j++)
//             {
//                 List<(EField eField, Vec3AnimationInfo animationInfo)> list = info.list;
//                 TranslateInfo translateInfo = new TranslateInfo();
//                 translateInfo.SetTransform(info.transform)
//                     .Set(list);
//                 translateInfos.Add(translateInfo);
//             }
//         }
//     }
// }
//
// public struct TranslateInfo
// {
//     private Transform transform;
//     private TransformInfo transformInfo;
//     
//     public void Release()
//     {
//         transformInfo.Release();
//     }
//     
//     public TransformInfo SetTransform(Transform transform)
//     {
//         this.transform = transform;
//         transformInfo.Reset();
//         return transformInfo;
//     }
// }
//
// public struct TransformInfo
// {
//     private EField eField;
//     private List<Vec3AnimationInfo> fields;
//
//     public void Release()
//     {
//         eField = default;
//         if(fields != null) ListPool<Vec3AnimationInfo>.Release(fields);
//     }
//
//     public void Reset()
//     {
//         Release();
//         fields = ListPool<Vec3AnimationInfo>.Get();
//     }
//
//     public void Set(List<(EField eField, Vec3AnimationInfo animationInfo)> list)
//     {
//         for (int i = 0; i < list.Count; i++)
//         {
//             (EField eField, Vec3AnimationInfo animationInfo) = list[i];
//             SetField(animationInfo, eField);
//         }
//     }
//
//     public TransformInfo SetPosition(Vec3AnimationInfo position, ERelative eRelative) 
//         => SetField(position, eRelative == ERelative.Local ? EField.LocalPosition : EField.Position);
//     
//     public TransformInfo SetEulerAngles(Vec3AnimationInfo eulerAngles, ERelative eRelative)
//         => SetField(eulerAngles, eRelative == ERelative.Local ? EField.LocalEulerAngles : EField.EulerAngles);
//     
//     public TransformInfo SetScale(Vec3AnimationInfo scale, ERelative eRelative)
//         => SetField(scale, eRelative == ERelative.Local ? EField.LocalScale : EField.LossyScale);
//     
//     private TransformInfo SetField(Vec3AnimationInfo field, EField eField)
//     {
//         switch (eField)
//         {
//             case EField.LocalPosition:
//             {
//                 this.eField &= ~EField.Position;
//                 this.eField |= EField.LocalPosition;
//                 break;
//             }
//             case EField.Position:
//             {
//                 this.eField &= ~EField.LocalPosition;
//                 this.eField |= EField.Position;
//                 break;
//             }
//             case EField.LocalEulerAngles:
//             {
//                 this.eField &= ~EField.EulerAngles;
//                 this.eField |= EField.LocalEulerAngles;
//                 break;
//             }
//             case EField.EulerAngles:
//             {
//                 this.eField &= ~EField.LocalEulerAngles;
//                 this.eField |= EField.EulerAngles;
//                 break;
//             }
//             case EField.LocalScale:
//             {
//                 this.eField &= ~EField.LossyScale;
//                 this.eField |= EField.LocalScale;
//                 break;
//             }
//             case EField.LossyScale:
//             {
//                 this.eField &= ~EField.LocalScale;
//                 this.eField |= EField.LossyScale;
//                 break;
//             }
//         }
//         int index = 0;
//         while (true)
//         {
//             if ((int)eField <= 1 << (index + 1))
//             {
//                 break;
//             }
//             index++;
//         }
//         while (index >= fields.Count)
//         {
//             fields.Add(default);
//         }
//         fields[index] = field;
//         return this;
//     }
// }
//
// public struct Vec3AnimationInfo
// {
//     private Vector3 origin;
//     private Vector3 target;
//     private int millisecond;
// }
//     
// /// 相对类型
// public enum ERelative
// {
//     Local = 1,
//     World = 2,
// }
//
// /// 字段类型
// [Flags]
// public enum EField
// {
//     LocalPosition = 1 << 0,
//     Position = 1 << 1,
//     LocalEulerAngles = 1 << 2,
//     EulerAngles = 1 << 3,
//     LocalScale = 1 << 4,
//     LossyScale = 1 << 5,
// }