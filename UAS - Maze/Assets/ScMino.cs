using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ScMino : MonoBehaviour
{
    [Header("Minotaur Settings")]
    public float walkSpeed = 4f;
    public float runSpeed = 8f;
    public float detectionRange = 30f;
    public float attackRange = 3f;
    public int damage = 50;
    public AudioClip roarSound;

    private Animator anim;
    private Transform runner;
    private AudioSource audioSource;
    private NavMeshAgent navAgent; // Komponen NavMeshAgent
    private bool isAttacking = false;
    private bool isYelling = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        runner = GameObject.FindWithTag("Runner").transform;
        audioSource = GetComponent<AudioSource>();

        // Set kecepatan jalan awal
        navAgent.speed = walkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (runner == null) return;

        float distanceToRunner = Vector3.Distance(transform.position, runner.position);

        if (distanceToRunner <= attackRange)
        {
            if (!isAttacking)
            {
                StartCoroutine(AttackRunner());
            }
        }
        else if (distanceToRunner <= detectionRange)
        {
            if (!isYelling)
            {
                StartCoroutine(YellAndChase());
            }
            else
            {
                ChaseRunner();
            }
        }
        else
        {
            Patrol();
        }
    }

    private IEnumerator YellAndChase()
    {
        isYelling = true;
        anim.SetBool("isYell", true);

        // Mainkan suara roar
        if (audioSource != null && roarSound != null)
        {
            audioSource.PlayOneShot(roarSound);
        }

        // Tunggu hingga animasi yell selesai
        yield return new WaitForSeconds(1.5f);

        anim.SetBool("isYell", false);
        anim.SetBool("isRun", true);

        // Ubah kecepatan lari
        navAgent.speed = runSpeed;
        isYelling = false;
    }

    private void ChaseRunner()
    {
        anim.SetBool("isRun", true);
        if (navAgent != null)
        {
            navAgent.SetDestination(runner.position);
        }
    }

    private IEnumerator AttackRunner()
    {
        isAttacking = true;
        anim.SetBool("isAtk", true);

        // Tunggu animasi attack selesai
        yield return new WaitForSeconds(1f);

        float distanceToRunner = Vector3.Distance(transform.position, runner.position);
        if (distanceToRunner <= attackRange)
        {
            ScRunner runnerScript = runner.GetComponent<ScRunner>();
            if (runnerScript != null)
            {
                runnerScript.TakeDamage(damage);
            }
        }

        anim.SetBool("isAtk", false);
        isAttacking = false;
    }

    private void Patrol()
    {
        anim.SetBool("isWalk", true);
        anim.SetBool("isRun", false);

        if (navAgent != null && navAgent.remainingDistance < 0.5f)
        {
            // Tentukan titik tujuan baru untuk patrol
            Vector3 randomPoint = GetRandomNavMeshPoint(transform.position, 10f);
            navAgent.SetDestination(randomPoint);
        }
    }

    private Vector3 GetRandomNavMeshPoint(Vector3 center, float range)
    {
        Vector3 randomDirection = Random.insideUnitSphere * range;
        randomDirection += center;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, range, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return center;
    }
}
