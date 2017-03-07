using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.CustomObjects.VoxelEngine
{
    public class WorldObjectsView : MonoBehaviour
    {
        [SerializeField] private GameObject ragdollCubePrefab;

        private IWorld world;
        private Dictionary<int, BlockTextureInfo> textureMap;
        private float tUnit;

        [Inject]
        public void Init(IWorld world, Dictionary<int, BlockTextureInfo> textureMap, float tUnit)
        {
            this.world = world;
            this.world.OnCollapseLandMass += CollapseLandMass;
            this.textureMap = textureMap;
            this.tUnit = tUnit;
        }

        void Start()
        {
            PoolManager.WarmPool(ragdollCubePrefab, 100);
        }

        private void CollapseLandMass(List<BlockInfo> landmass)
        {
            foreach (var block in landmass)
            {
                world.SetBlock((int) block.Location.x, (int) block.Location.y, 0);

                var position = new Vector3(block.Location.x + 0.5f, block.Location.y - 0.5f);

                var ragdollCube = PoolManager.SpawnObject(ragdollCubePrefab, position, Quaternion.identity);
                var rigidBody = ragdollCube.GetComponent<Rigidbody>();
                var ragdollCubeScript = ragdollCube.GetComponent<RagdollCube>();
                ragdollCubeScript.terrain = gameObject;
                var textureVector = textureMap[block.Type].vectorOnAtlas;
                rigidBody.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX;
                ragdollCube.GetComponent<Renderer>().material.mainTextureOffset = textureVector * tUnit;
                ragdollCube.GetComponent<Renderer>().material.mainTextureScale = new Vector2(tUnit, tUnit);
            }
        }
    }
}