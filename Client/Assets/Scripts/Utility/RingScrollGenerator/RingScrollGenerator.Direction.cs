using System;

public partial class RingScrollGenerator
{
    /// 方向
    public enum EDirection
    {
        // 顺时针
        Clockwise = -1,
        // 逆时针
        Anticlockwise = 1,
    }

    /// 排序
    private enum ESequence
    {
        /// 反序
        Reverse = -1,
        /// 正序
        Forward = 1,
    }
    
    private class DirectionInfo
    {
        private EDirection eGenerateDir;
        
        public void Init(EDirection eGenerateDir)
        {
            this.eGenerateDir = eGenerateDir;
        }
        
        public EDirection EGenerateDir()
        {
            return eGenerateDir;
        }

        public ESequence EScrollSequence(int selectedIndex, int curIndex, int count, EDirection? eDirection)
        {
            int downwardTime = 0;
            int upwardCountTime = 0;
            if (selectedIndex < curIndex)
            {
                downwardTime += count;
            }
            else
            {
                upwardCountTime += count; 
            }
            downwardTime += selectedIndex - curIndex;
            upwardCountTime += curIndex - selectedIndex;

            if (downwardTime < upwardCountTime)
            {
                return ESequence.Forward;
            }
            if (downwardTime > upwardCountTime)
            {
                return ESequence.Reverse;
            }
            if (eDirection != null) // 处理只有两个 item 时，点击按钮同向问题
            {
                return (EGenerateDir(), eDirection.Value) switch
                {
                    (EDirection.Clockwise, EDirection.Clockwise) => ESequence.Reverse,
                    (EDirection.Clockwise, EDirection.Anticlockwise) => ESequence.Forward,
                    (EDirection.Anticlockwise, EDirection.Clockwise) => ESequence.Forward,
                    (EDirection.Anticlockwise, EDirection.Anticlockwise) => ESequence.Reverse,
                };
            }
            // 相等，全部顺时针旋转
            return EGenerateDir() == EDirection.Clockwise ? ESequence.Reverse : ESequence.Forward;
        }
    }
}