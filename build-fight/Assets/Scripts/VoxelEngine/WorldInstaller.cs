using System.ComponentModel;
using Zenject;

namespace Assets.Scripts.CustomObjects.VoxelEngine
{
    public class WorldInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IWorld>().To<World>().AsSingle();
        }
    }
}