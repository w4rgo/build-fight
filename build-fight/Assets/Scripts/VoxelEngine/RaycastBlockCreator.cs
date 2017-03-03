using System.Linq;
using UnityEditor;

namespace Assets.Scripts.CustomObjects.VoxelEngine
{
    using UnityEngine;
    using System.Collections;

    public class RaycastBlockCreator : MonoBehaviour {

        public GameObject terrain;
        private PolygonGenerator tScript;
        public GameObject target;
        private LayerMask layerMask = (1 << 0);

        // Use this for initialization
        void Start () {

            tScript=terrain.GetComponent<PolygonGenerator>();

        }

        // Update is called once per frame
        public void CreateBlock () {
            Vector2 point = new Vector2(transform.position.x, transform.position.y);

            Collider [] colliders = Physics.OverlapBox(transform.position, transform.localScale/2 );

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

        private void OnDrawGizmos()
        {
            Gizmos.DrawCube(transform.position, transform.localScale);
        }
    }
}