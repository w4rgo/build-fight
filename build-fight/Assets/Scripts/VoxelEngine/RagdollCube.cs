using UnityEngine;

namespace Assets.Scripts.CustomObjects.VoxelEngine
{
    public class RagdollCube : MonoBehaviour
    {
        public GameObject terrain;

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject == terrain)
            {
                PoolManager.ReleaseObject(this.gameObject);
            }
            Debug.Log(other);
        }
    }
}