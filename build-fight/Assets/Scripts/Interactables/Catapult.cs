using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.CustomObjects.Interactables;
using Assets.Scripts.CustomObjects.VoxelEngine;
using UnityEngine;

public class Catapult : MonoBehaviour
{
    [SerializeField]
    private string shootTrigger= "shoot";
    [SerializeField]
    private string reloadTrigger = "reload";

    [SerializeField] private CatapultRock rockPrefab;

    [SerializeField] private Transform rockSpawnPoint;

    [SerializeField] private float rockForce;

    [SerializeField] private Vector3 rockDirection;

    [SerializeField] private RaycastBlockCreator destructor;

    private Animator animator;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
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
        Debug.Log("Spanwing rock");
        var rock = Instantiate<CatapultRock>(rockPrefab);
        rock.destructor = destructor;
        rock.transform.position = rockSpawnPoint.position;
        rock.GetComponent<Rigidbody>().AddForce(rockDirection*rockForce,ForceMode.Impulse);

    }
}