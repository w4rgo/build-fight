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
                Debug.Log("Connected1 : " + CalcConnected(x, y + 1, Color.black));
                visited = new byte[32, 32];

                Debug.Log("Connected 2: " + CalcConnected(x - 1, y, Color.cyan));
                visited = new byte[32, 32];

                Debug.Log("Connected 3: " + CalcConnected(x + 1, y, Color.green));
                visited = new byte[32, 32];

                Debug.Log("Connected4 : " + CalcConnected(x, y - 1, Color.blue));


                PaintCross(x, y, Color.red);

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
        }

        private static void PaintCross(int x, int y, Color color)
        {
            Debug.DrawLine(new Vector3(x, y, -10), new Vector3(x + 1, y - 1, -10), color);
            Debug.DrawLine(new Vector3(x, y - 1, -10), new Vector3(x + 1, y, -10), color);

        }


        private int CalcConnected(int x, int y, Color debugColor)
        {
            PaintCross(x, y, debugColor);
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
            if (y > 0 && visited[x, y - 1] == 0)
            {
                if (tScript.blocks[x, y - 1] != 0)
                {
                    visited[x, y - 1] = 1;
                    added += 1 + CalcConnected(x, y - 1, debugColor);
                }
            }

            if (y < tScript.blocks.GetLength(1) - 1 && visited[x, y + 1] == 0)
            {
                if (tScript.blocks[x, y + 1] != 0)
                {
                    visited[x, y + 1] = 1;
                    added += 1 + CalcConnected(x, y + 1, debugColor);
                }
            }

            if (x < tScript.blocks.GetLength(0) - 1 && visited[x + 1, y] == 0)
            {
                if (tScript.blocks[x + 1, y] != 0)
                {
                    visited[x + 1, y] = 1;
                    added += 1 + CalcConnected(x + 1, y, debugColor);
                }
            }


            if (x > 0 &&
                visited[x - 1, y] == 0)
            {
                if (tScript.blocks[x - 1, y] != 0)
                {
                    visited[x - 1, y] = 1;
                    added += 1 + CalcConnected(x - 1, y, debugColor);
                }
            }

            return added;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawCube(transform.position, transform.localScale);
        }
    }
}