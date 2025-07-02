using System.Collections.Generic;
using UnityEngine;

namespace Splitter.Core
{
    /// 显示分离器：核心
    public interface IDisplaySplitterCore
    {
        /// 必回收时的条件 ||
        /// 1、对象池 Get 出来的 Core
        /// 2、订阅了 标记为 回收 的 list
        void Release();

        /// 订阅器，list 来自对象池，且需要 Release 时触发 回收，须标记
        IDisplaySplitterCore Subscribe(int key, List<GameObject> items, bool allowRelease = false);

        void DisplayUnique(int key);
        /// 即时回收，keys 来自非对象池，勿标记回收
        void DisplaySet(List<int> keys, bool allowRelease = false);
        /// 当 订阅的 key 为 flags 时，可使用
        void DisplayFlags(int keyFlags);
    }
}