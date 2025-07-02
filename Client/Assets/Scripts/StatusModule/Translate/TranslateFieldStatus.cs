using System;
using UnityEngine;

namespace Status.Translate
{
    [Serializable]
    public class TranslateFieldStatusWithComponent: TranslateFieldStatus
    {
        [SerializeField] private UnityEngine.Transform transform;
    }
    
    [Serializable]
    public class TranslateFieldStatus
    {
        [SerializeField] private TranslateFieldUnitStatus unitStatusX;
        [SerializeField] private TranslateFieldUnitStatus unitStatusY;
        [SerializeField] private TranslateFieldUnitStatus unitStatusZ;

    }

    [Serializable]
    public class TranslateFieldUnitStatus
    {
        [SerializeField] private AnimationCurve curveValue;
        [SerializeField] private int millisecond;
    }
}