namespace Selector.Linear.Core
{
    // 极限范围 内 索引
    public class LimitRangeItemDataIndexParamCore : IItemDataIndexParamCore 
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
            if (param.SelectedDataIndex < param.ItemCount)
            {
                return 0;
            }
            return param.SelectedDataIndex + 1 - param.ItemCount;
        }
    }
}
