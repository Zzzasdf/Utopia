using System;
using UnityEngine;

namespace Status.Translate
{
    [Serializable]
    public class TranslateStatus
    {
        [SerializeField] private UnityEngine.Transform transform;
        [SerializeField] private TranslateFieldStatus positionStatus;
        [SerializeField] private TranslateFieldStatus eulerAnglesStatus;
        [SerializeField] private TranslateFieldStatus scaleStatus;
    }
}