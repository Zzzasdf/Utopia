namespace UIViewModule
{
    // Mono 型驱动
    public class UIMonoDriver : UIDriverBase
    {
        private void Awake() => Init();
        private void OnEnable() => Show();
        private void OnDisable() => Hide();
        private void OnDestroy() => Destroy();
    }
}