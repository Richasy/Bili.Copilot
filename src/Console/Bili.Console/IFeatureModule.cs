namespace Bili.Console;

internal interface IFeatureModule
{
    Task RunAsync();

    void Exit();
}
