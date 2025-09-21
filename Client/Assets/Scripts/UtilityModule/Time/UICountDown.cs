using System;
using UnityEngine;
using UnityEngine.UI;

namespace TimeModule
{
    /// 倒计时
    [RequireComponent(typeof(Text))]
    public class UICountDown : MonoBehaviour
    {
        private Text text;
        private Action<int, long> fixedFrequencyHandler;
        private Action<int, bool> endHandler;

        private bool isAwake;
        private bool isStartUp;
        
        private long endMilliseconds;
        private FixedFrequencyManager.ETimeFormat eTimeFormat;
        private Action<bool> endCallback;
        
        private int? fixedFrequencyUniqueId;
        private int? timerUniqueId;
        
        private void Awake()
        {
            text = GetComponent<Text>();
            fixedFrequencyHandler = OnFixedFrequencyHandler;
            endHandler = OnEndHandler;
            isAwake = true;
            if (!isStartUp) return;
            SetFixedFrequency();
        }
        public void StartUp(long endMilliseconds, FixedFrequencyManager.ETimeFormat eTimeFormat, Action<bool> endCallback)
        {
            this.endMilliseconds = endMilliseconds;
            this.eTimeFormat = eTimeFormat;
            this.endCallback = endCallback;
            isStartUp = true;
            if (!isAwake) return;
            SetFixedFrequency();
        }
        private void SetFixedFrequency()
        {
            Cancel();
            fixedFrequencyUniqueId = FixedFrequencyManager.Instance.SetEndMilliseconds(endMilliseconds, fixedFrequencyHandler);
            timerUniqueId = TimerManager.Instance.SetEndMilliseconds(endMilliseconds, endHandler);
        }
    
        private void OnDestroy()
        {
            Cancel();
        }
        public void Cancel()
        {
            if (fixedFrequencyUniqueId.HasValue)
            {
                FixedFrequencyManager.Instance.Cancel(fixedFrequencyUniqueId.Value);
                fixedFrequencyUniqueId = null;
            }
            if (timerUniqueId.HasValue)
            {
                TimerManager.Instance.Cancel(timerUniqueId.Value);
                timerUniqueId = null;
            }
        }
    
        private void OnFixedFrequencyHandler(int fixedFrequencyUniqueId, long currentMilliseconds)
        {
            text.text = FixedFrequencyManager.GetTimeFormat(eTimeFormat, currentMilliseconds, endMilliseconds);
        }
        private void OnEndHandler(int timerUniqueId, bool isSuccess)
        {
            if (fixedFrequencyUniqueId.HasValue)
            {
                FixedFrequencyManager.Instance.Cancel(fixedFrequencyUniqueId.Value);
                fixedFrequencyUniqueId = null;
            }
            if (this.timerUniqueId.HasValue)
            {
                this.timerUniqueId = null;
            }
            endCallback?.Invoke(isSuccess);
        }
    }
}
