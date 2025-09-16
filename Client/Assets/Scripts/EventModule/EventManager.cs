using System;
using System.Collections.Generic;
using UnityEngine;

namespace EventModule
{
    public interface IEvent<T>: IEvent
    {
        void EventHandler(T data);
    }
    public interface IEvent
    {
        
    }
    
    public class EventManager
    {
        private static EventManager _instance;
        public static EventManager Instance => _instance ??= new EventManager();

        private Dictionary<Type, HashSet<IEvent>> eventDict = new Dictionary<Type, HashSet<IEvent>>();
        
        public void Subscribe<T>(IEvent<T> tEvent)
        {
            Type type = typeof(T);
            if (!eventDict.TryGetValue(type, out HashSet<IEvent> events))
            {
                eventDict.Add(type, events = new HashSet<IEvent>());
            }
            events.Add(tEvent);
        }

        public void Fire<T>(T t)
        {
            Type type = typeof(T);
            if (!eventDict.TryGetValue(type, out HashSet<IEvent> events))
            {
                return;
            }
            foreach (var item in events)
            {
                ((IEvent<T>)item).EventHandler(t);   
            }
        }
    }
    

    public class Test: IEvent<EventType.AA>, IEvent<EventType.BB>
    {
        public void Register()
        {
            EventManager.Instance.Subscribe<EventType.AA>(this);
            EventManager.Instance.Subscribe<EventType.BB>(this);
        }

        public void Fire()
        {
            EventManager.Instance.Fire(new EventType.AA
            {
                a = 1,
            });
        }
        
        public void EventHandler(EventType.AA data)
        {
            Debug.LogError(data.a);
        }

        public void EventHandler(EventType.BB data)
        {
            Debug.LogError(data.b);
        }
    }
}
