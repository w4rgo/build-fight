using System;
using UnityEngine;

namespace Assets.Scripts.CustomObjects.VoxelEngine
{
    public interface IWorldModifier
    {
        void CreateBlock(Vector3 location);
        void DestroyBlock(Vector3 location);
        void DestroyAreaAroundCollision(Collision collision, int radius);
    }
}