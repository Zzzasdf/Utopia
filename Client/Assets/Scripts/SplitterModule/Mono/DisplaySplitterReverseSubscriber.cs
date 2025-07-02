using System;
using System.Collections.Generic;
using UnityEngine;

namespace Splitter.Mono
{
    /// 显示分离器：核心 Mono 形态 反向订阅器
    /// 对应 的 组件上实现 该接口
    public interface IDisplaySplitterReverseSubscriber
    {
        void SubscribeDisplayUnique(Action<int> displayUnique);
        void SubscribeDisplaySet(Action<List<int>, bool> displaySet);
        void SubscribeDisplayFlags(Action<int> displayFlags);
    }

    /// 显示分离器 的 反向订阅器
    [RequireComponent(typeof(IDisplaySplitterReverseSubscriber))]
    public class DisplaySplitterReverseSubscriber : MonoBehaviour
    {
        [SerializeField] private List<DisplaySplitterMonoBase> splitters;

        private void Awake()
        {
            IDisplaySplitterReverseSubscriber subscriber = GetComponent<IDisplaySplitterReverseSubscriber>();
            Action<int> displayUnique = null;
            Action<List<int>, bool> displaySet = null;
            Action<int> displayFlags = null;
            for (int i = 0; i < splitters.Count; i++)
            {
                DisplaySplitterMonoBase splitter = splitters[i];
                if (!splitter.Flags)
                {
                    displayUnique += splitter.Core.DisplayUnique;
                    displaySet += splitter.Core.DisplaySet;
                }
                else
                {
                    displayFlags += splitter.Core.DisplayFlags;
                }
            }
            subscriber.SubscribeDisplayUnique(displayUnique);
            subscriber.SubscribeDisplaySet(displaySet);
            subscriber.SubscribeDisplayFlags(displayFlags);
        }
    }
}