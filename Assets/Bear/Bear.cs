using UnityEngine;
using UnityEngine.AI;

public class Bear : MonoBehaviour
{
    public GameObject player;
    public float sight = 12f;
    public float range = 2.7f;
    public float pushtime = 0.2f;
    public float attPushtime = 1.5f;

    float currentTime = 0;
    float attackDelay = 3f;

    Vector3 originPos;
    Quaternion originRot;
    NavMeshAgent agent;
    Animator anim;

    enum State
    {
        Idle,
        Chase,
        Attack,
        Return
    }

    State stat;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stat = State.Idle;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        originPos = transform.position;
        originRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        switch(stat)
        {
            case State.Idle:
                Idle();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Attack:
                Attack();
                break;
            case State.Return:
                Return();
                break;
        }
    }

    void Idle()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < sight)
        {
            stat = State.Chase;
            anim.SetTrigger("Chase");
        }
    }

    void Chase()
    {
        if (Vector3.Distance(transform.position, player.transform.position) > sight)
        {
            stat = State.Return;
            anim.SetTrigger("Return");
        }
        else if(Vector3.Distance(transform.position, player.transform.position) > range)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.stoppingDistance = range;
            agent.SetDestination(player.transform.position);
        }
        else
        {
            stat = State.Attack;
            currentTime = attackDelay;
            anim.SetTrigger("AttackDelay");
        }
    }

    void Attack()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < range)
        {
            currentTime += Time.deltaTime;
            if (currentTime > attackDelay)
            {
                currentTime = 0;
                anim.SetTrigger("Attack");
            }
        }
        else
        {
            stat = State.Chase;
            currentTime = 0;
            anim.SetTrigger("Chase");
        }
    }

    public void AttackAction()
    {
        //플레이어 래그돌화 추가하기
        //플레이어 밀치기 추가하기
    }

    void Return()
    {
        if (Vector3.Distance(transform.position, originPos) > 0.1f)
        {
            currentTime += Time.deltaTime;

            agent.SetDestination(originPos);
            agent.stoppingDistance = 0;

            if (currentTime > 1f && Vector3.Distance(transform.position, player.transform.position) < sight)
            {
                currentTime = 0;
                stat = State.Chase;
                anim.SetTrigger("Chase");
            }
        }
        else
        {
            currentTime = 0;

            agent.isStopped = true;
            agent.ResetPath();

            transform.position = originPos;
            transform.rotation = originRot;

            stat = State.Idle;
            anim.SetTrigger("Idle");
        }
    }
}
