using NUnit.Framework;

[TestFixture]
public class TestBusyDisposable
{
    private static bool result;

    private static BusyDisposable NewBusyDisposable => new BusyDisposable(() => result = false);

    [Test] 
    public static void Foo()
    { 
        Foo1();
        Assert.AreEqual(result, false, "未被更改");
    }
    
    private static void Foo1()
    {
        result = true;
        using var _ = NewBusyDisposable;
    }
}