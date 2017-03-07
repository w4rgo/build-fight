using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.CustomObjects.VoxelEngine
{
    public interface IWorld
    {
        event Action Updated;
        bool IsOutOfBounds(int x, int y);
        void GenTerrain();
        void SetBlock(int x, int y, byte value);
        void DestroyBlock(int x, int y);
        byte GetBlock(int x, int y);
        int GetHeight();
        int GetWidth();
        List<BlockInfo> ProcessLandMass(int x, int y, Color black);
        event Action<List<BlockInfo>> OnCollapseLandMass;
        void CollapseDisconnectedLandmasses(int x, int y);
    }
}