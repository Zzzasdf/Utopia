using System;

public partial class RingScrollGenerator
{
    /// 方向
    private enum Direction
    {
        // 顺时针
        Clockwise = -1,
        // 逆时针
        Anticlockwise = 1,
    }

    /// 排序
    private enum Sequence
    {
        /// 反序
        Reverse = -1,
        /// 正序
        Forward = 1,
    }
    
    private class DirectionInfo
    {
        private Func<Direction> generateDirFunc;
        
        public DirectionInfo(Func<Direction> generateDirFunc)
        {
            this.generateDirFunc = generateDirFunc;
        }

        public Direction GenerateDir()
        {
            return generateDirFunc.Invoke();
        }

        public Sequence ScrollSequence(int selectedIndex, int curIndex, int count)
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
                return Sequence.Forward;
            }
            if (downwardTime > upwardCountTime)
            {
                return Sequence.Reverse;
            }
            // 相等，全部顺时针旋转
            return GenerateDir() == Direction.Clockwise ? Sequence.Reverse : Sequence.Forward;
        }
    }
}
