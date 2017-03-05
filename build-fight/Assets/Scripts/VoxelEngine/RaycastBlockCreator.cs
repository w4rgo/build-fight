using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.CustomObjects.VoxelEngine
{
    using UnityEngine;


    public class RaycastBlockCreator : MonoBehaviour
    {
        public GameObject terrain;
        private PolygonGenerator tScript;
        private LayerMask layerMask = (1 << 0);
        private byte[,] visited;

        private int tooMuch = 0;

        [SerializeField] private GameObject ragdollCubePrefab;

        void Start()
        {
            PoolManager.WarmPool(ragdollCubePrefab, 100);
            tScript = terrain.GetComponent<PolygonGenerator>();
        }

        public void CreateBlock()
        {
            Vector2 point = new Vector2(transform.position.x, transform.position.y);
            Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 2);
            if (colliders.Length > 0)
            {
                foreach (var collider in colliders)
                {
                    if (collider.gameObject.GetComponent<PolygonGenerator>() == null)
                    {
                        return;
                    }
                }
                var x = Mathf.RoundToInt(point.x - .5f);
                var y = Mathf.RoundToInt(point.y + .5f);
                if (tScript.IsOutOfBounds(x,y))
                {
                    tScript.blocks[x, y] = 1;
                    tScript.update = true;
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
                    if (collider.gameObject.GetComponent<PolygonGenerator>() == null)
                    {
                        return;
                    }
                }
                var x = Mathf.RoundToInt(point.x - .5f);
                var y = Mathf.RoundToInt(point.y + .5f);

                if (tScript.IsOutOfBounds(x,y))
                {
                    tScript.blocks[x, y] = 0;
                    tScript.update = true;

                    CollapseDisconnectedLandmasses(x, y);
                    PaintCross(x, y, Color.red);
                }
            }
        }

        private void CollapseDisconnectedLandmasses(int x, int y)
        {
            var upLandmass = ProcessLandMass(x, y + 1, Color.black);
            var leftLandMass = ProcessLandMass(x - 1, y, Color.cyan);
            var rightLandmass = ProcessLandMass(x + 1, y, Color.green);
            var downLandmass = ProcessLandMass(x, y - 1, Color.blue);

            var biggerLandmassCount = Math.Max(Math.Max(upLandmass.Count, leftLandMass.Count),
                Math.Max(rightLandmass.Count, downLandmass.Count));

            if (upLandmass.Count < biggerLandmassCount)
            {
                CollapseLandMass(upLandmass);
            }
            if (leftLandMass.Count < biggerLandmassCount)
            {
                CollapseLandMass(leftLandMass);
            }
            if (rightLandmass.Count < biggerLandmassCount)
            {
                CollapseLandMass(rightLandmass);
            }
            if (downLandmass.Count < biggerLandmassCount)
            {
                CollapseLandMass(downLandmass);
            }
        }

        private void CollapseLandMass(List<BlockInfo> landmass)
        {
            foreach (var block in landmass)
            {
                tScript.blocks[(int) block.Location.x, (int) block.Location.y] = 0;

                var position = new Vector3(block.Location.x + 0.5f, block.Location.y - 0.5f);

                var ragdollCube = PoolManager.SpawnObject(ragdollCubePrefab, position, Quaternion.identity);
                var rigidBody = ragdollCube.GetComponent<Rigidbody>();
                var ragdollCubeScript = ragdollCube.GetComponent<RagdollCube>();
                ragdollCubeScript.terrain = tScript.gameObject;
                var textureVector = tScript.textureVectorMap[block.Type];
                rigidBody.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX;
                ragdollCube.GetComponent<Renderer>().material.mainTextureOffset = textureVector * tScript.tUnit;
                ragdollCube.GetComponent<Renderer>().material.mainTextureScale =
                    new Vector2(tScript.tUnit, tScript.tUnit);
                //StartCoroutine(DestroyRagdollCube(ragdollCube));
            }
        }


        private List<BlockInfo> ProcessLandMass(int x, int y, Color color)
        {
            visited = new byte[32, 32];
            var connectedLandmass = CalcConnected(x, y, color);
            return connectedLandmass;
        }

        private static void PaintCross(int x, int y, Color color)
        {
            Debug.DrawLine(new Vector3(x, y, -10), new Vector3(x + 1, y - 1, -10), color);
            Debug.DrawLine(new Vector3(x, y - 1, -10), new Vector3(x + 1, y, -10), color);
        }


        private List<BlockInfo> CalcConnected(int x, int y, Color debugColor)
        {
            var added = new List<BlockInfo>();

            if (tScript.IsOutOfBounds(x,y))
            {
                visited[x, y] = 1;
                if (tScript.blocks[x, y] == 0)
                {
                    return new List<BlockInfo>();
                }

                added.Add(new BlockInfo(tScript.blocks[x, y], new Vector2(x, y)));
                if (y > 0 && visited[x, y - 1] == 0)
                {
                    if (tScript.blocks[x, y - 1] != 0)
                    {
                        added.AddRange(CalcConnected(x, y - 1, debugColor));
                    }
                }

                if (y < tScript.blocks.GetLength(1) - 1 && visited[x, y + 1] == 0)
                {
                    if (tScript.blocks[x, y + 1] != 0)
                    {
                        added.AddRange(CalcConnected(x, y + 1, debugColor));
                    }
                }

                if (x < tScript.blocks.GetLength(0) - 1 && visited[x + 1, y] == 0)
                {
                    if (tScript.blocks[x + 1, y] != 0)
                    {
                        added.AddRange(CalcConnected(x + 1, y, debugColor));
                    }
                }
                if (x > 0 && visited[x - 1, y] == 0)
                {
                    if (tScript.blocks[x - 1, y] != 0)
                    {
                        added.AddRange(CalcConnected(x - 1, y, debugColor));
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
    }

    public class BlockInfo
    {
        private int type;
        private Vector2 location;

        public BlockInfo(int type, Vector2 location)
        {
            this.type = type;
            this.location = location;
        }

        public int Type
        {
            get { return type; }
        }

        public Vector2 Location
        {
            get { return location; }
        }
    }
}