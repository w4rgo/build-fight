using Assets.Scripts.CustomObjects.VoxelEngine;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.CustomObjects.Interactables
{
    public class CatapultRock : MonoBehaviour
    {
        [SerializeField] private int destructionRadius = 1;
        private IWorldModifier worldModifier;

        [Inject]
        public void Init(IWorldModifier worldModifier)
        {
            this.worldModifier = worldModifier;
        }

        private void OnCollisionEnter(Collision other)
        {

            Debug.Log("Colisione con : " + other.gameObject.name);
            worldModifier.DestroyAreaAroundCollision(other,destructionRadius);
            Destroy(this.gameObject);
        }
    }
}