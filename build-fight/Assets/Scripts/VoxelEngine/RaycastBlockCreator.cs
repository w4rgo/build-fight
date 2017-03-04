using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Assets.Scripts.CustomObjects.VoxelEngine
{
    using UnityEngine;


    public class RaycastBlockCreator : MonoBehaviour
    {
        public GameObject terrain;
        private PolygonGenerator tScript;
        private LayerMask layerMask = (1 << 0);
        private byte[,] visited;

        public Sprite matTexture;
//        public Texture2D matTexture;
        private int tooMuch = 0;

        // Use this for initialization
        void Start()
        {
            tScript = terrain.GetComponent<PolygonGenerator>();
        }

        // Update is called once per frame
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
                tScript.blocks[x, y] = 1;
                tScript.update = true;
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
                tScript.blocks[x, y] = 0;
                tScript.update = true;

                CollapseDisconnectedLandmasses(x, y);

                PaintCross(x, y, Color.red);

                CalculateRealtotal();
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

            Debug.Log("The bigger landmass count is : " + biggerLandmassCount);
        }

        private void CollapseLandMass(List<BlockInfo> landmass)
        {
            Debug.Log("Collapsing landmass with blocks: " + landmass.Count);
            foreach (var block in landmass)
            {
                tScript.blocks[(int) block.Location.x, (int) block.Location.y] = 0;

                var ragdollCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                ragdollCube.transform.position = new Vector3(block.Location.x + 0.5f, block.Location.y - 0.5f);
                var rigidBody = ragdollCube.AddComponent<Rigidbody>();
                ragdollCube.AddComponent<Collider>();

                ragdollCube.GetComponent<MeshRenderer>().material = tScript.GetComponent<Material>();


//                newUV.Add(new Vector2 (tUnit * texture.x, tUnit * texture.y + tUnit));
//                newUV.Add(new Vector2 (tUnit * texture.x + tUnit, tUnit * texture.y + tUnit));
//                newUV.Add(new Vector2 (tUnit * texture.x + tUnit, tUnit * texture.y));
//                newUV.Add(new Vector2 (tUnit * texture.x, tUnit * texture.y));

                var textureVector = tScript.textureVectorMap[block.Type];

                var textureVectorX = (int) textureVector.x;
                var textureVectorY = (int) textureVector.y;

                Debug.Log(textureVector);


                var newTexture = new Texture2D(32, 32);
                newTexture.SetPixels(0,0,32,32, matTexture.texture.GetPixels(0,0,32,32));

                rigidBody.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX;
//                ragdollCube.GetComponent<MeshRenderer>().material = tScript.GetComponent<Material>();
                ragdollCube.GetComponent<MeshRenderer>().material.mainTexture = newTexture;

                StartCoroutine(DestroyRagdollCube(ragdollCube));
            }
        }

        private IEnumerator DestroyRagdollCube(GameObject ragdollCube)
        {
            yield return new WaitForSeconds(Random.Range(3, 6));

            Destroy(ragdollCube);
        }

        private void CalculateRealtotal()
        {
            var count = 0;
            for (int i = 0; i < tScript.blocks.GetLength(0); i++)
            {
                for (int j = 0; j < tScript.blocks.GetLength(1); j++)
                {
                    if (tScript.blocks[i, j] != 0)
                    {
                        count++;
                    }
                }
            }
            Debug.Log("REAL TOTAL: " + count);
        }

        private List<BlockInfo> ProcessLandMass(int x, int y, Color color)
        {
            visited = new byte[32, 32];
            var connectedLandmass = CalcConnected(x, y, color);
            Debug.Log("Connected1 : " + connectedLandmass.Count);
            return connectedLandmass;
        }

        private static void PaintCross(int x, int y, Color color)
        {
            Debug.DrawLine(new Vector3(x, y, -10), new Vector3(x + 1, y - 1, -10), color);
            Debug.DrawLine(new Vector3(x, y - 1, -10), new Vector3(x + 1, y, -10), color);
        }


        private List<BlockInfo> CalcConnected(int x, int y, Color debugColor)
        {
            visited[x, y] = 1;
            PaintCross(x, y, debugColor);
//            tooMuch++;
//            if (tooMuch > 10000)
//            {
//                tooMuch = 0;
//                Debug.Log("it was too much");
//                return new List<Vector2>();
//            }
            if (tScript.blocks[x, y] == 0)
            {
                return new List<BlockInfo>();
            }

            var added = new List<BlockInfo>();
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