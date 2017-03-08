using System;
using UnityEngine;

namespace Assets.Scripts.CustomObjects.VoxelEngine
{
    public class WorldModifier : IWorldModifier, IWorldModifierPM
    {
        public event Action<Vector3> OnCreateBlock;
        public event Action<Vector3> OnDestroyBlock;
        public event Action<Collision, int> OnDestroyAreaAroundCollision;

        public void CreateBlock(Vector3 location)
        {
            OnCreateBlock(location);
        }

        public void DestroyBlock(Vector3 location)
        {
            OnDestroyBlock(location);
        }

        public void DestroyAreaAroundCollision(Collision collision, int radius)
        {
            OnDestroyAreaAroundCollision(collision, radius);
        }
    }
}