using Zenject;

namespace Assets.Scripts.CustomObjects.VoxelEngine
{
    using UnityEngine;
    using System.Collections.Generic;

    public class WorldMeshView : MonoBehaviour
    {
        //studentgamedev tutorial 4
        private List<Vector3> newVertices = new List<Vector3>();

        private List<int> newTriangles = new List<int>();
        private List<Vector2> newUV = new List<Vector2>();

        private List<Vector3> colVertices = new List<Vector3>();
        private List<int> colTriangles = new List<int>();
        private int colCount;

        private Mesh mesh;
        private MeshCollider col;

        private float tUnit = 0.25f;
        private int squareCount;

        private IWorld world;

        private Dictionary<BlockType, BlockTextureInfo> textureInfos;

        [Inject]
        public void Init(IWorld world, Dictionary<BlockType, BlockTextureInfo> textureInfos, float tUnit)
        {
            this.world = world;
            world.Updated += OnWorldUpdated;
            this.textureInfos = textureInfos;
            this.tUnit = tUnit;
        }

        private void OnWorldUpdated()
        {
            BuildMesh();
            UpdateMesh();
        }

        // Use this for initialization
        void Start()
        {
            mesh = GetComponent<MeshFilter>().mesh;
            col = GetComponent<MeshCollider>();


            world.GenTerrain();
            BuildMesh();
            UpdateMesh();
        }

        void BuildMesh()
        {
            for (int px = 0; px < world.GetWidth(); px++)
            {
                for (int py = 0; py < world.GetHeight(); py++)
                {
                    if (world.GetBlock(px, py) != 0)
                    {
                        GenCollider(px, py);

                        if (world.GetBlock(px, py) == textureInfos[BlockType.STONE].id)
                        {
                            GenSquare(px, py, textureInfos[BlockType.STONE].vectorOnAtlas);
                        }
                        else if (world.GetBlock(px, py) == textureInfos[BlockType.GRASS].id)
                        {
                            GenSquare(px, py, textureInfos[BlockType.GRASS].vectorOnAtlas);
                        }
                    }
                }
            }
        }

        byte Block(int x, int y)
        {
            if (x == -1 || x == world.GetWidth() || y == -1 || y == world.GetHeight())
            {
                return (byte) 1;
            }

            return world.GetBlock(x, y);
        }

        void GenCollider(int x, int y)
        {
            //Top
            if (Block(x, y + 1) == 0)
            {
                colVertices.Add(new Vector3(x, y, 1));
                colVertices.Add(new Vector3(x + 1, y, 1));
                colVertices.Add(new Vector3(x + 1, y, 0));
                colVertices.Add(new Vector3(x, y, 0));
                ColliderTriangles();
                colCount++;
            }

            //bot
            if (Block(x, y - 1) == 0)
            {
                colVertices.Add(new Vector3(x, y - 1, 0));
                colVertices.Add(new Vector3(x + 1, y - 1, 0));
                colVertices.Add(new Vector3(x + 1, y - 1, 1));
                colVertices.Add(new Vector3(x, y - 1, 1));
                ColliderTriangles();
                colCount++;
            }

            //left
            if (Block(x - 1, y) == 0)
            {
                colVertices.Add(new Vector3(x, y - 1, 1));
                colVertices.Add(new Vector3(x, y, 1));
                colVertices.Add(new Vector3(x, y, 0));
                colVertices.Add(new Vector3(x, y - 1, 0));
                ColliderTriangles();
                colCount++;
            }

            //right
            if (Block(x + 1, y) == 0)
            {
                colVertices.Add(new Vector3(x + 1, y, 1));
                colVertices.Add(new Vector3(x + 1, y - 1, 1));
                colVertices.Add(new Vector3(x + 1, y - 1, 0));
                colVertices.Add(new Vector3(x + 1, y, 0));
                ColliderTriangles();
                colCount++;
            }
        }

        void ColliderTriangles()
        {
            colTriangles.Add(colCount * 4);
            colTriangles.Add((colCount * 4) + 1);
            colTriangles.Add((colCount * 4) + 3);
            colTriangles.Add((colCount * 4) + 1);
            colTriangles.Add((colCount * 4) + 2);
            colTriangles.Add((colCount * 4) + 3);
        }

        void GenSquare(int x, int y, Vector2 texture)
        {
            newVertices.Add(new Vector3(x, y, 0));
            newVertices.Add(new Vector3(x + 1, y, 0));
            newVertices.Add(new Vector3(x + 1, y - 1, 0));
            newVertices.Add(new Vector3(x, y - 1, 0));

            newTriangles.Add(squareCount * 4);
            newTriangles.Add((squareCount * 4) + 1);
            newTriangles.Add((squareCount * 4) + 3);
            newTriangles.Add((squareCount * 4) + 1);
            newTriangles.Add((squareCount * 4) + 2);
            newTriangles.Add((squareCount * 4) + 3);

            newUV.Add(new Vector2(tUnit * texture.x, tUnit * texture.y + tUnit));
            newUV.Add(new Vector2(tUnit * texture.x + tUnit, tUnit * texture.y + tUnit));
            newUV.Add(new Vector2(tUnit * texture.x + tUnit, tUnit * texture.y));
            newUV.Add(new Vector2(tUnit * texture.x, tUnit * texture.y));

            squareCount++;
        }

        void UpdateMesh()
        {
            mesh.Clear();
            mesh.vertices = newVertices.ToArray();
            mesh.triangles = newTriangles.ToArray();
            mesh.uv = newUV.ToArray();
            mesh.RecalculateNormals();

            newVertices.Clear();
            newTriangles.Clear();
            newUV.Clear();
            squareCount = 0;

            Mesh newMesh = new Mesh();
            newMesh.vertices = colVertices.ToArray();
            newMesh.triangles = colTriangles.ToArray();
            col.sharedMesh = newMesh;

            colVertices.Clear();
            colTriangles.Clear();
            colCount = 0;
        }
    }
}