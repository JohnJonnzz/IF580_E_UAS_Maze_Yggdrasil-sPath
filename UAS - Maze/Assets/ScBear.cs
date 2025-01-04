using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // Tambahkan ini untuk menggunakan NavMesh

public class ScBear : MonoBehaviour
{
    [Header("Bear Settings")]
    public float walkSpeed = 6f;
    public float detectionRange = 50f;
    public float attackRange = 2f;
    public int damage = 30;
    public float patrolRadius = 20f; // Radius untuk area patroli
    public float patrolWaitTime = 3f; // Waktu berhenti di setiap titik patroli

    private Animator anim;
    private Transform runner;
    private NavMeshAgent agent; // Tambahkan NavMeshAgent
    private bool isAttacking = false;
    private Vector3 patrolDestination; // Tujuan berikutnya untuk patroli
    private bool isPatrolling = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>(); // Ambil NavMeshAgent
        agent.speed = walkSpeed; // Sesuaikan kecepatan dari NavMeshAgent
        runner = GameObject.FindWithTag("Runner").transform;

        StartCoroutine(Patrol()); // Mulai patroli
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
            agent.isStopped = true; // Hentikan pergerakan saat menyerang
        }
        else if (distanceToRunner <= detectionRange)
        {
            StopPatrolling(); // Hentikan patroli jika runner terdeteksi
            anim.SetBool("isWalk", true);
            agent.isStopped = false; // Lanjutkan pergerakan
            agent.SetDestination(runner.position); // Gunakan NavMesh untuk bergerak ke posisi runner
        }
        else
        {
            if (!isPatrolling)
            {
                StartCoroutine(Patrol()); // Mulai patroli jika runner tidak terdeteksi
            }
        }
    }

    private IEnumerator AttackRunner()
    {
        isAttacking = true;
        anim.SetBool("isAtk", true);

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

    private IEnumerator Patrol()
    {
        isPatrolling = true;
        while (true)
        {
            // Pilih tujuan acak dalam radius patroli
            patrolDestination = GetRandomPatrolPoint();
            agent.SetDestination(patrolDestination);
            anim.SetBool("isWalk", true);
            agent.isStopped = false;

            // Tunggu sampai mencapai tujuan atau jika tujuan terlalu jauh
            while (agent.pathPending || agent.remainingDistance > 0.5f)
            {
                yield return null;
            }

            anim.SetBool("isWalk", false);
            agent.isStopped = true;

            // Tunggu sebelum berpindah ke tujuan berikutnya
            yield return new WaitForSeconds(patrolWaitTime);
        }
    }

    private void StopPatrolling()
    {
        isPatrolling = false;
        StopCoroutine(Patrol());
        agent.isStopped = true;
    }

    private Vector3 GetRandomPatrolPoint()
    {
        // Tentukan titik acak dalam radius patroli
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return transform.position; // Jika tidak menemukan titik, tetap di posisi sekarang
    }
}
