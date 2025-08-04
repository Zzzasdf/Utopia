public class RedPointTree 
{
    private ERedPointTree current;
    private ERedPoint root;
    
    public RedPointTree(ERedPointTree self, ERedPoint root)
    {
        current = self;
        this.root = root;
    }
}
