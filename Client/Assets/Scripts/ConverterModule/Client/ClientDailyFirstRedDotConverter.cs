using System;

public class ClientDailyFirstRedDotConverter : BitmapTimingResetConverter
{
    protected override Func<long, bool> TimingResetFunc => OnTimingReset;
    protected override Func<long> TimingAfterResetDataFunc => OnTimingAfterResetDataFunc;
    protected override Action TimingAfterResetCallBack => OnTimingAfterResetCallBack;

    private bool OnTimingReset(long lastData)
    {
        return !SystemTime.Instance.IsToday(lastData);
    }

    private long OnTimingAfterResetDataFunc()
    {
        return SystemTime.Instance.CurrentUnixTimeMilliseconds();
    }
    
    private void OnTimingAfterResetCallBack()
    {
        ClientConverterCollectorSystem.EDailyFirstRedDot.Unique_16.Save();
    }
    
    public bool this[EDailyFirstRedDot flag]
    {
        get => HasFlag(flag);
        set => SetFlag(flag, value);
    }  
    
    public bool HasFlag(EDailyFirstRedDot flag)
    {
        return HasFlag((uint)flag);
    }
        
    public void SetFlag(EDailyFirstRedDot flag, bool value)
    {
        SetFlag((uint)flag, value);
    }

    public void AddFlag(EDailyFirstRedDot flag)
    {
        AddFlag((uint)flag);
    }

    public void RemoveFlag(EDailyFirstRedDot flag)
    {
        RemoveFlag((uint)flag);
    }
}