using System;
using System.Collections.Generic;
using Zenject;

namespace Assets.Scripts.CustomObjects.VoxelEngine
{
    using UnityEngine;

    public class RaycastBlockCreator : MonoBehaviour
    {
        private IWorld world;
        private LayerMask layerMask = (1 << 0);

        private int tooMuch = 0;


        [Inject]
        public void Init(IWorld world)
        {
            this.world = world;
        }



        public void CreateBlock()
        {
            Vector2 point = new Vector2(transform.position.x, transform.position.y);
            Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 2);
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

        public void DestructBlock()
        {
            Vector2 point = new Vector2(transform.position.x, transform.position.y);

            Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 2);

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
                world.DestroyBlock(x,y);
            }
        }

        public void DestructAreaAroundCollision(Collision collision)
        {

            if (collision.gameObject.GetComponent<WorldMeshView>() == null)
            {
                return;
            }
            var radius = 1;
            var hit = collision.contacts[0];
            var point = hit.point;
            for (float i = hit.point.x - radius; i < hit.point.x + radius; i += 0.5f)
            {
                for (float j = hit.point.y - radius; j < hit.point.y + radius; j += 0.5f)
                {

                    var vector2 = new Vector2(i, j);
                    Debug.Log(vector2);

                    DestroyPoint(vector2);
                }
            }
        }






    }
}