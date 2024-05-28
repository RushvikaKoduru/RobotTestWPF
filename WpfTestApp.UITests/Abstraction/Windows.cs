using Test.Common;
using WpfTestApp.UITests.Abstraction;

public class Windows : WindowTraversalBase
{
    public MainWindow MainWindow => (MainWindow)GetWindow( () => new MainWindow("HeadTestUtility"), "Head Test Utility");

}