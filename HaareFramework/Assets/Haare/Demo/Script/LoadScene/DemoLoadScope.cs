using VContainer;
using VContainer.Unity;

using Haare.Client.UI;


namespace Demo.LoadScene
{
    public class DemoLoadScope : LifetimeScope 
    {
        
        //protected override LifetimeScope Parent => FindObjectOfType<CoreLifetimeScope>();
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<DemoLoadUIManager>()
                .As<SceneUIManager>().AsSelf();;
            builder.RegisterComponentInHierarchy<DemoLoadMono>();
            builder.RegisterEntryPoint<DemoLoadUIPresenter>();
        }
    }
}