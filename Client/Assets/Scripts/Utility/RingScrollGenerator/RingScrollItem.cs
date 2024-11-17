using System;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class RingScrollItem : MonoBehaviour
{
    [SerializeField] private Button btnArea;
    [SerializeField] private GameObject objSelected;
    private Action<int, Transform> onItemRender;
    private Action<int> onItemClick;
    
    private int index;

    public void Init(Action<int, Transform> onItemRender, Action<int> onItemClick)
    {
        this.onItemRender = onItemRender;
        btnArea.onClick.RemoveAllListeners();
        btnArea.onClick.AddListener(OnBtnAreaClick);
        this.onItemClick = onItemClick;
        return;

        void OnBtnAreaClick()
        {
            this.onItemClick?.Invoke(index);
        }
    }

    public void Refresh(int index)
    {
        this.index = index;
        this.onItemRender?.Invoke(index, transform);
    }

    public void SetSelectedEffect(bool isActive)
    {
        objSelected.gameObject.SetActive(isActive);
    }
}