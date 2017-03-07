using System;

namespace Assets.Scripts.CustomObjects.VoxelEngine
{
    public interface IWorld
    {
        event Action Updated;
        bool IsOutOfBounds(int x, int y);
        void GenTerrain();
        byte[,] Blocks { get; }
        void SetBlock(int x, int y, int value);
        int GetBlock(int x, int y);
        int GetHeight();
        int GetWidth();
    }
}