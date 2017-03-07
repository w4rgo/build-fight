using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.CustomObjects.VoxelEngine
{
    public class WorldInstaller : MonoInstaller
    {
        private Dictionary<BlockType, BlockTextureInfo> typeToTextureDictionary =
            new Dictionary<BlockType, BlockTextureInfo>();

        private Dictionary<int, BlockTextureInfo> idToTextureDictionary =
            new Dictionary<int, BlockTextureInfo>();

        [SerializeField] public List<BlockTextureInfo> textureMap = new List<BlockTextureInfo>();
        [SerializeField] public float textureUnit = 0.25f;

        public override void InstallBindings()
        {
            foreach (var texture in textureMap)
            {
                typeToTextureDictionary[texture.Type] = texture;
                idToTextureDictionary[texture.id] = texture;
            }


            Container.Bind<IWorld>().To<World>().AsSingle();
            Container.BindInstance(typeToTextureDictionary).WhenInjectedInto<WorldMeshView>();
            Container.BindInstance(textureUnit).WhenInjectedInto<WorldMeshView>();
            Container.BindInstance(idToTextureDictionary).WhenInjectedInto<WorldObjectsView>();
            Container.BindInstance(textureUnit).WhenInjectedInto<WorldObjectsView>();
        }
    }
}