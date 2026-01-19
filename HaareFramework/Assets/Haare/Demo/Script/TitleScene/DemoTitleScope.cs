using VContainer;
using VContainer.Unity;

using Haare.Client.UI;


namespace Demo.TitleScene
{
    public class DemoTitleScope : LifetimeScope 
    {
        
        //protected override LifetimeScope Parent => FindObjectOfType<CoreLifetimeScope>();
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<DemoTitleUIManager>().As<SceneUIManager>().AsSelf();;
            builder.RegisterComponentInHierarchy<DemoTitleMono>();
            builder.RegisterEntryPoint<DemoTitleUIPresenter>();
        }
    }
}