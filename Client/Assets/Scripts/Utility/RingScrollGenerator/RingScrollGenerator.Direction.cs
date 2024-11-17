using System;

public partial class RingScrollGenerator
{
    /// 方向
    private enum EDirection
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
        private Func<EDirection> eGenerateDirFunc;
        
        public DirectionInfo(Func<EDirection> eGenerateDirFunc)
        {
            this.eGenerateDirFunc = eGenerateDirFunc;
        }

        public EDirection EGenerateDir()
        {
            return eGenerateDirFunc.Invoke();
        }

        public ESequence EScrollSequence(int selectedIndex, int curIndex, int count)
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
            // 相等，全部顺时针旋转
            return EGenerateDir() == EDirection.Clockwise ? ESequence.Reverse : ESequence.Forward;
        }
    }
}