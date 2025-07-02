namespace Selector.Linear.Core
{
    /// 居中 索引
    public class CenterItemDataIndexParamCore : IItemDataIndexParamCore 
    {
        /// 两个数据索引 是否 指向 同块 item
        bool IItemDataIndexParamCore.IsSameItem(IItemDataIndexParamCore.IsSameItemParam param)
        {
            return param.LeftSelectedDataIndex - SetDisplayStartDataIndex(new IItemDataIndexParamCore.SetDisplayStartDataIndexParam
                   {
                       ItemCount = param.ItemCount,
                       DataCount = param.DataCount,
                       SelectedDataIndex = param.LeftSelectedDataIndex
                   })
                   == param.RightSelectedDataIndex - SetDisplayStartDataIndex(new IItemDataIndexParamCore.SetDisplayStartDataIndexParam
                   {
                       ItemCount = param.ItemCount,
                       DataCount = param.DataCount,
                       SelectedDataIndex = param.RightSelectedDataIndex
                   });
        }

        /// 设置显示 item 的 起始数据索引
        int IItemDataIndexParamCore.SetDisplayStartDataIndex(IItemDataIndexParamCore.SetDisplayStartDataIndexParam param)
            => SetDisplayStartDataIndex(param);
        
        private int SetDisplayStartDataIndex(IItemDataIndexParamCore.SetDisplayStartDataIndexParam param)
        {
            // 中间前后有值的情况下，以中心显示，否则以 最值为边界
            int startDataIndex = param.SelectedDataIndex - param.ItemCount / 2;
            if (startDataIndex < 0) return 0;
            int endIndex = param.SelectedDataIndex + param.ItemCount / 2;
            if (endIndex >= param.DataCount) return param.DataCount - param.ItemCount;
            return startDataIndex;
        }
    }
}
