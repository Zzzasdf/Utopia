using System.Collections.Generic;

namespace ReddotModule
{
    public partial class ReddotConfig
    {
        /// 模型列表
        private partial List<EReddot> ModelList() => new()
        {
            TestFunc2(),
        };

        private EReddot TestFunc2() =>
            EReddot.TestIcon.Linker(
                EReddot.TestTab1.Linker(
                    EReddot.TestDataList.Linker(
                        EReddot.TestDataUnit.BindNodeLayer1<Layer1>(Layer1Condition, Layer1Count).Linker(
                            EReddot.TestDataUnitChild.BindNodeLayer2<Layer2>(Layer2Condition, Layer2Count))),
                    EReddot.TestTab2),
                EReddot.TestTab2);
        
        public int Layer1Count()
        {
            return 10;
        }

        public bool Layer1Condition(int layer1Index)
        {
            return false;
        }

        public int Layer2Count(int layer1Index)
        {
            return 10;
        }
        public bool Layer2Condition(int layer1Index, int layer2Index)
        {
            return false;
        }
    }
}