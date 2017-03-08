using System;
using UnityEngine;

namespace Assets.Scripts.CustomObjects.VoxelEngine
{
    public interface IWorldModifierPM
    {
        event Action<Vector3> OnCreateBlock;
        event Action<Vector3> OnDestroyBlock;
        event Action<Collision, int> OnDestroyAreaAroundCollision;
    }
}