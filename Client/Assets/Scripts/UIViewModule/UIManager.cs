using UIViewModule;

public partial class UIManager
{
    private static UIManager _instance;
    public static UIManager Instance => _instance ??= new UIManager();

    public void Show<TView>()
        where TView: UIViewUnitBase
    {
        TView unit = null; // 加载实体
        UIViewDriver uiViewDriver = unit.gameObject.AddComponent<UIViewDriver>(); // 装载 View 型驱动
        uiViewDriver.Init();
        uiViewDriver.Show();
    }
}
