using System;
using UnityEngine;

namespace Assets.Scripts.CustomObjects.VoxelEngine
{
    public class World : IWorld
    {
        public event Action Updated = delegate {  };
        public byte[,] Blocks { get; private set; }

        public void SetBlock(int x, int y, int value)
        {
            Blocks[x, y] = (byte) value;
            Updated();
        }

        public int GetBlock(int x, int y)
        {
            return (byte) Blocks[x, y];
        }

        public int GetHeight()
        {
            return Blocks.GetLength(1);
        }

        public int GetWidth()
        {
            return Blocks.GetLength(0);
        }

        public bool IsOutOfBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Blocks.GetLength(0) && y < Blocks.GetLength(1);
        }

        int NoiseInt(int x, int y, float scale, float mag, float exp)
        {
            return (int) (Mathf.Pow((Mathf.PerlinNoise(x / scale, y / scale) * mag), (exp)));
        }


        public void GenTerrain()
        {
            Debug.Log("Generating terrain");

            Blocks = new byte[32, 32];

            for (int px = 0; px < Blocks.GetLength(0); px++)
            {
                int stone = NoiseInt(px, 0, 80, 15, 1);
                stone += NoiseInt(px, 0, 50, 30, 1);
                stone += NoiseInt(px, 0, 10, 10, 1);
                stone += 75;


                int dirt = NoiseInt(px, 0, 100f, 35, 1);
                dirt += NoiseInt(px, 100, 50, 30, 1);
                dirt += 75;

                for (int py = 0; py < Blocks.GetLength(1); py++)
                {
                    if (py < Blocks.GetLength(1) / 2)
                    {
                        if (py < stone)
                        {
                            Blocks[px, py] = 1;

                            if (NoiseInt(px, py, 12, 16, 1) > 10)
                            {
                                //dirt spots
                                Blocks[px, py] = 2;
                            }

                            if (NoiseInt(px, py * 2, 16, 14, 1) > 10)
                            {
                                //Caves
                                Blocks[px, py] = 0;
                            }
                        }
                        else if (py < dirt)
                        {
                            Blocks[px, py] = 2;
                        }
                    }
                    else
                    {
                        Blocks[px, py] = 0;
                    }
                }
            }
            Updated();
        }

    }
}