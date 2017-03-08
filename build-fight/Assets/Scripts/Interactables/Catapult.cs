using Assets.Scripts.CustomObjects.Interactables;
using UnityEngine;
using Zenject;

public class Catapult : MonoBehaviour
{
    [SerializeField] private string shootTrigger = "shoot";
    [SerializeField] private string reloadTrigger = "reload";

    [SerializeField] private CatapultRock rockPrefab;

    [SerializeField] private Transform rockSpawnPoint;

    [SerializeField] private float rockForce;

    [SerializeField] private Vector3 rockDirection;

    private Animator animator;
    private IInstantiator instantiator;

    [Inject]
    public void Init(IInstantiator instantiator)
    {
        this.instantiator = instantiator;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Shoot()
    {
        animator.SetTrigger(shootTrigger);
    }

    public void Reload()
    {
        animator.SetTrigger(reloadTrigger);
    }

    public void SpawnRock()
    {
        var rock = instantiator.InstantiatePrefab(rockPrefab);
        rock.transform.position = rockSpawnPoint.position;
        rock.GetComponent<Rigidbody>().AddForce(rockDirection * rockForce, ForceMode.Impulse);
    }
}