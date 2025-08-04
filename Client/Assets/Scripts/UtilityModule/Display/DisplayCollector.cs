using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class DisplayCollector : MonoBehaviour 
{
    [SerializeField] private List<DisplayCollectorItemGroup> groups;
    private bool? status;

    public void SetStatus()
    {
        bool status = Status();
        if (!this.status.HasValue || status != this.status)
        {
            this.status = status;
            gameObject.SetActive(status);
        }
    }
    private bool Status()
    {
        bool result = false;
        for (int i = 0; i < groups.Count; i++)
        {
            DisplayCollectorItemGroup group = groups[i];
            if (group.Status())
            {
                return true;
            }
        }
        return result;
    }
    
    [Serializable]
    private class DisplayCollectorItemGroup 
    {
        [SerializeField] private List<DisplayCollectorItemBase> conditions;
        public bool Status()
        {
            for (int i = 0; i < conditions.Count; i++)
            {
                if (!conditions[i].Status())
                {
                    return false;
                }
            }
            return true;
        }
    }
}

[RequireComponent(typeof(DisplayCollector))]
public abstract class DisplayCollectorItemBase: MonoBehaviour
{
    public abstract bool Status();
}
