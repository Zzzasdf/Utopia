namespace ReddotModule
{
    public interface IReddotTreeManager
    {
        IReddotConfig ReddotConfig { get; }
        IReddotTree ReddotTree { get; }
    }
    
    public class ReddotTreeManager : Singleton<ReddotTreeManager>, IReddotTreeManager
    {
        IReddotConfig IReddotTreeManager.ReddotConfig => reddotConfig;
        IReddotTree IReddotTreeManager.ReddotTree => reddotTree;
        private IReddotConfig reddotConfig { get; }
        private IReddotTree reddotTree { get; }

        public ReddotTreeManager()
        {
            reddotConfig = new ReddotConfig();
            reddotTree = new ReddotTree(reddotConfig);
        }

    }
}
