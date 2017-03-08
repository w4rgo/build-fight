using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.CustomObjects.VoxelEngine
{
    public class World : IWorld
    {
        public event Action<List<BlockInfo>> OnCollapseLandMass = delegate(List<BlockInfo> list) {  };
        public event Action Updated = delegate { };
        private byte[,] Blocks;
        private byte[,] visited;

        public void SetBlock(int x, int y, byte value)
        {
            Blocks[x, y] = value;
            Updated();
        }

        public void DestroyBlock(int x, int y)
        {
            SetBlock(x, y, 0);
            CollapseDisconnectedLandmasses(x, y);
        }

        public byte GetBlock(int x, int y)
        {
            return  Blocks[x, y];
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

        public List<BlockInfo> ProcessLandMass(int x, int y, Color color)
        {
            visited = new byte[32, 32];
            var connectedLandmass = CalculateConnectedBlocks(x, y, color);
            return connectedLandmass;
        }

        private List<BlockInfo> CalculateConnectedBlocks(int x, int y, Color debugColor)
        {
            var added = new List<BlockInfo>();

            if (IsOutOfBounds(x, y))
            {
                visited[x, y] = 1;
                if (GetBlock(x, y) == 0)
                {
                    return new List<BlockInfo>();
                }

                added.Add(new BlockInfo(GetBlock(x, y), new Vector2(x, y)));
                if (y > 0 && visited[x, y - 1] == 0)
                {
                    if (GetBlock(x, y - 1) != 0)
                    {
                        added.AddRange(CalculateConnectedBlocks(x, y - 1, debugColor));
                    }
                }

                if (y < GetHeight() - 1 && visited[x, y + 1] == 0)
                {
                    if (GetBlock(x, y + 1) != 0)
                    {
                        added.AddRange(CalculateConnectedBlocks(x, y + 1, debugColor));
                    }
                }

                if (x < GetWidth() - 1 && visited[x + 1, y] == 0)
                {
                    if (GetBlock(x + 1, y) != 0)
                    {
                        added.AddRange(CalculateConnectedBlocks(x + 1, y, debugColor));
                    }
                }
                if (x > 0 && visited[x - 1, y] == 0)
                {
                    if (GetBlock(x - 1, y) != 0)
                    {
                        added.AddRange(CalculateConnectedBlocks(x - 1, y, debugColor));
                    }
                }
            }
            else
            {
                return new List<BlockInfo>();
            }
            PaintCross(x, y, debugColor);


            return added;
        }

        public void CollapseDisconnectedLandmasses(int x, int y)
        {
            var upLandmass = ProcessLandMass(x, y + 1, Color.black);
            var leftLandMass = ProcessLandMass(x - 1, y, Color.cyan);
            var rightLandmass = ProcessLandMass(x + 1, y, Color.green);
            var downLandmass = ProcessLandMass(x, y - 1, Color.blue);

            var biggerLandmassCount = Math.Max(Math.Max(upLandmass.Count, leftLandMass.Count),
                Math.Max(rightLandmass.Count, downLandmass.Count));

            if (upLandmass.Count < biggerLandmassCount)
            {
                OnCollapseLandMass(upLandmass);
            }
            if (leftLandMass.Count < biggerLandmassCount)
            {
                OnCollapseLandMass(leftLandMass);
            }
            if (rightLandmass.Count < biggerLandmassCount)
            {
                OnCollapseLandMass(rightLandmass);
            }
            if (downLandmass.Count < biggerLandmassCount)
            {
                OnCollapseLandMass(downLandmass);
            }
        }

        private static void PaintCross(int x, int y, Color color)
        {
            Debug.DrawLine(new Vector3(x, y, -10), new Vector3(x + 1, y - 1, -10), color);
            Debug.DrawLine(new Vector3(x, y - 1, -10), new Vector3(x + 1, y, -10), color);
        }
    }
}

