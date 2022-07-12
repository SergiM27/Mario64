using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoombaEnemy : MonoBehaviour, GameManager.IRestartGameElement
{
    public int life = 1;
    public int startLife = 1;
    public Vector3 startPosition;
    public Quaternion startRotation;

    private NavMeshAgent agent;
    private Transform player;
    private Animator animator;
    public LayerMask whatIsGround, whatIsPlayer;
    public AudioSource enemyDeathSound;
    

    [Header("Patroling")]
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    [Header("Attacking")]
    public float cooldownAttack;
    bool alreadyAttacked;

    [Header("States")]
    public float sightRange, attackRange;
    public bool playerInAttackRange, playerInSightRange;
    public float rotationSpeed;
    private bool isDead;
    [SerializeField] private GameObject dieParticles;


    // Start is called before the first frame update
    void Start()
    {
        enemyDeathSound = SoundManager.instance.enemyDies;
        player = FindObjectOfType<PlayerController>().gameObject.transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        startPosition = transform.position;
        startRotation = transform.rotation;
        isDead = false;
        GameManager.instance.AddRestartGameElement(this);
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!isDead)
        {
            if (!playerInSightRange && !playerInAttackRange) Patrol();

            if (playerInSightRange && !playerInAttackRange) Alert();

            if (playerInSightRange && playerInAttackRange) Attack();
        }
       
    }


    private void Patrol()
    {
        animator.SetBool("Run", false);
        animator.SetBool("Alert", false);
        agent.speed = 2;
        if (!walkPointSet) SearchWalkPoint();
        else
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //He llegado al walkpoint
        if(distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if(Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    private void Alert()
    {
        agent.SetDestination(player.position);
        animator.SetBool("Alert",true);
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    void StartRun()
    {
        animator.SetBool("Run", true);
        agent.speed = 10;
    }

    private void Attack()
    {
        animator.SetBool("Run", false);
        animator.SetBool("Alert", false);
        agent.SetDestination(transform.position);
        Quaternion lookOnLook = Quaternion.LookRotation(player.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime * 5);
        if (!alreadyAttacked)
        {
            if (GameManager.instance.isDead == false)
            {
                //Codigo de que animacion o que ataque tiene que hacer aqui y hacer daño
               
                FindObjectOfType<HealthController>().SubstractHealth();
                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), cooldownAttack);
            }
        }
        
        
    }

    void ResetAttack()
    {
        alreadyAttacked = false;
    }


    public void RestartGame()
    {
        transform.rotation = startRotation;
        transform.position = startPosition;
        animator.SetBool("Run", false);
        animator.SetBool("Alert", false);
        life = startLife;
        isDead = false;
        GetComponent<Collider>().enabled = true ;
        gameObject.SetActive(true);
        animator.SetTrigger("Restart");
    }

    public void Kill()
    {
        enemyDeathSound.Play();
        GameObject go = Instantiate(dieParticles, transform.position, Quaternion.identity);
        Destroy(go, 5);
        gameObject.SetActive(false);   
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    public void ReciveHit(GameObject mario, float knockbackForce)
    {
        agent.isStopped = true;
        GetComponent<Collider>().enabled = false;
        Vector3 knockbackDirection = transform.position - mario.transform.position;
        knockbackDirection.Normalize();
        StartCoroutine(Knockback(knockbackForce, -transform.forward));
        animator.SetTrigger("Die");
        isDead = true;

    }

    private IEnumerator Knockback(float force, Vector3 direction)
    {
        float speed = force;
        float damp = 2;
        while (speed > 0)
        {
            transform.Translate(direction * speed * Time.deltaTime);
            speed -= Time.deltaTime * damp;
            yield return null;
        }
        Kill();
        yield return null;
    }

}
