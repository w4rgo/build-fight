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

                visited = new byte[32, 32];
                Debug.Log("Connected1 : " + CalcConnected(x+1,y+1));
                visited = new byte[32, 32];

                Debug.Log("Connected 2: " + CalcConnected(x-1,y+1));
                visited = new byte[32, 32];

                Debug.Log("Connected 3: " + CalcConnected(x+1,y-1));
                visited = new byte[32, 32];

                Debug.Log("Connected4 : " + CalcConnected(x-1,y-1));
            }
        }

        private int CalcConnected(int x, int y)
        {

            tooMuch++;
            if (tooMuch > 10000)
            {
                tooMuch = 0;
                Debug.Log("it was too much");
                return 0;
            }
            if (tScript.blocks[x, y] == 0)
            {
                return 0;
            }
            var added = 0;
            if (x > 0 && y > 0 && visited[x - 1, y - 1] == 0)
            {
                if (tScript.blocks[x - 1, y - 1] != 0)
                {
                    visited[x - 1, y - 1] = 1;
                    added += 1 + CalcConnected(x - 1, y - 1);
                }
            }

            if (x > 0 && y < tScript.blocks.GetLength(1) - 1 && visited[x - 1, y + 1] == 0)
            {
                if (tScript.blocks[x - 1, y + 1] != 0)
                {
                    visited[x - 1, y + 1] = 1;
                    added += 1 + CalcConnected(x - 1, y + 1);
                }
            }

            if (x < tScript.blocks.GetLength(0) - 1 && y > 0 && visited[x + 1, y - 1] == 0)
            {
                if (tScript.blocks[x + 1, y - 1] != 0)
                {
                    visited[x + 1, y - 1] = 1;
                    added += 1 + CalcConnected(x + 1, y - 1);
                }
            }


            if (x < tScript.blocks.GetLength(0) - 1 && y < tScript.blocks.GetLength(1) - 1 && visited[x + 1, y + 1] == 0)
            {
                if (tScript.blocks[x + 1, y + 1] != 0)
                {
                    visited[x + 1, y + 1] = 1;
                    added += 1 + CalcConnected(x + 1, y + 1);
                }
            }

            return added;
        }

        private void CalculateConnectedParts()
        {
            var connected = 0;
            for (int i = 0; i < tScript.blocks.GetLength(0); i++)
            {
                for (int j = 0; j < tScript.blocks.GetLength(1); j++)
                {
                    Debug.Log(i + " " + j);
                    if (i > 0 && i < tScript.blocks.GetLength(0) - 1 && j > 0 && j < tScript.blocks.GetLength(1) - 1)
                    {
                        if (tScript.blocks[i - 1, j - 1] != 0)
                        {
                            connected++;
                        }
                        else if (tScript.blocks[i - 1, j + 1] != 0)
                        {
                            connected++;
                        }
                        else if (tScript.blocks[i + 1, j - 1] != 0)
                        {
                            connected++;
                        }
                        else if (tScript.blocks[i + 1, j + 1] != 0)
                        {
                            connected++;
                        }
                    }
                }
            }
            Debug.Log("Connected blocks:" + connected);
            connected = 0;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawCube(transform.position, transform.localScale);
        }
    }
}