using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.CustomObjects.VoxelEngine
{
    public class WorldObjectsView : MonoBehaviour
    {

        [SerializeField] private GameObject ragdollCubePrefab;
        private WorldMeshView worldView;

        private IWorld world;

        [Inject]
        public void Init(IWorld world)
        {
            this.world = world;
            this.world.OnCollapseLandMass += CollapseLandMass;
        }

        void Start()
        {
            worldView = GetComponent<WorldMeshView>();
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
                ragdollCubeScript.terrain = worldView.gameObject;
                var textureVector = worldView.textureVectorMap[block.Type];
                rigidBody.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX;
                ragdollCube.GetComponent<Renderer>().material.mainTextureOffset = textureVector * worldView.tUnit;
                ragdollCube.GetComponent<Renderer>().material.mainTextureScale =
                    new Vector2(worldView.tUnit, worldView.tUnit);
                //StartCoroutine(DestroyRagdollCube(ragdollCube));
            }
        }


    }
}