using Assets.Scripts.CustomObjects.VoxelEngine;
using UnityEngine;

namespace Assets.Scripts.CustomObjects.Interactables
{
    public class CatapultRock : MonoBehaviour
    {

        public RaycastBlockCreator destructor;

        private void OnCollisionEnter(Collision other)
        {

            Debug.Log("Colisione con : " + other.gameObject.name);
            destructor.DestructAreaAroundCollision(other);
            Destroy(this.gameObject);
        }
    }
}