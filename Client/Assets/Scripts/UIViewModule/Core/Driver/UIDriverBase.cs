using UnityEngine;

[RequireComponent(typeof(UIViewUnitBase))]
[DisallowMultipleComponent]
public abstract class UIDriverBase: MonoBehaviour, IDriver
{
    private UIViewUnitBase uiViewUnitBase;
    public virtual void Init()
    {
        uiViewUnitBase = GetComponent<UIViewUnitBase>();
        uiViewUnitBase.Init();
    }
    public virtual void Show()
    {
        uiViewUnitBase.Show();
    }
    public virtual void Hide()
    {
        uiViewUnitBase.Hide();
    }
    public virtual void Destroy()
    {
        uiViewUnitBase.Destroy();
    }
}

public interface IDriver
{
    void Init();
    void Show();
    void Hide();
    void Destroy();
}