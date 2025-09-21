using MVP;
using UnityEngine;
using Zenject;

public class CoreInstaller : MonoInstaller
{
    [SerializeField] private ClickerData clickerData;
    
    public override void InstallBindings()
    {
        Container.Bind<TabsPresenter>().AsSingle();
        Container.Bind<TabsModel>().AsSingle();
        Container.Bind<ClickerData>().FromInstance(clickerData).AsSingle();
    }
}
