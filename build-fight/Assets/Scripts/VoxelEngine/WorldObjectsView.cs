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
        public void Init(IWorld world, Dictionary<int, BlockTextureInfo> textureMap, float tUnit,
            IWorldModifierPM worldModifier)
        {
            this.world = world;
            this.world.OnCollapseLandMass += CollapseLandMass;
            this.textureMap = textureMap;
            this.tUnit = tUnit;
            worldModifier.OnCreateBlock += CreateBlockOnSelector;
            worldModifier.OnDestroyAreaAroundCollision += DestructAreaAroundCollision;
            worldModifier.OnDestroyBlock += DestructBlockOnSelector;
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

        public void CreateBlockOnSelector(Vector3 location)
        {
            Vector2 point = new Vector2(location.x, location.y);
            Collider[] colliders = Physics.OverlapBox(location, Vector3.one / 2);
            if (colliders.Length > 0)
            {
                foreach (var collider in colliders)
                {
                    if (collider.gameObject.GetComponent<WorldMeshView>() == null)
                    {
                        return;
                    }
                }
                var x = Mathf.RoundToInt(point.x - .5f);
                var y = Mathf.RoundToInt(point.y + .5f);
                if (world.IsOutOfBounds(x, y))
                {
                    world.SetBlock(x, y, 1);
                }
            }
        }

        public void DestructBlockOnSelector(Vector3 location)
        {
            Vector2 point = new Vector2(location.x, location.y);

            Collider[] colliders = Physics.OverlapBox(location, Vector3.one / 2);

            if (colliders.Length > 0)
            {
                foreach (var collider in colliders)
                {
                    if (collider.gameObject.GetComponent<WorldMeshView>() == null)
                    {
                        return;
                    }
                }
                DestroyPoint(point);
            }
        }

        private void DestroyPoint(Vector2 point)
        {
            var x = Mathf.RoundToInt(point.x - .5f);
            var y = Mathf.RoundToInt(point.y + .5f);

            if (world.IsOutOfBounds(x, y))
            {
                world.DestroyBlock(x, y);
            }
        }

        public void DestructAreaAroundCollision(Collision collision, int radius)
        {
            if (collision.gameObject.GetComponent<WorldMeshView>() == null)
            {
                return;
            }
            var hit = collision.contacts[0];
            for (float i = hit.point.x - radius; i < hit.point.x + radius; i += 0.5f)
            {
                for (float j = hit.point.y - radius; j < hit.point.y + radius; j += 0.5f)
                {
                    var vector2 = new Vector2(i, j);
                    DestroyPoint(vector2);
                }
            }
        }
    }
}