using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Dragon : MonoBehaviour
{
    [SerializeField] Transform target;
    private NavMeshAgent agent;


    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update()
    {
        agent.SetDestination(target.position);
    }
}
