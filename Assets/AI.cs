using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Panda;

public class AI : MonoBehaviour
{
    public Transform player;
    public Transform bulletSpawn;
    public Slider healthBar;   
    public GameObject bulletPrefab;
    public bool encostou;

    NavMeshAgent agent;
    public Vector3 destination; // The movement destination.
    public Vector3 target;      // The position to aim to.
    float health = 100.0f;
    float rotSpeed = 5.0f;

    float visibleRange = 80.0f;
    float shotRange = 40.0f;
    GameObject bullet;
    Vector3 dest;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            encostou = true;
        }
    }

    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        agent.stoppingDistance = shotRange - 5; //for a little buffer
        InvokeRepeating("UpdateHealth",5,0.5f);
        encostou = false;
    }

    void Update()
    {
        dest = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().position;
        encostou = false;
        Vector3 healthBarPos = Camera.main.WorldToScreenPoint(this.transform.position);
        healthBar.value = (int)health;
        healthBar.transform.position = healthBarPos + new Vector3(0,60,0);
        if (bullet != null)
        {
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 2000);
        }
    }

    void UpdateHealth()
    {
       if(health < 100)
        health ++;
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "bullet")
        {
            health -= 10;
        }
    }

    [Task]
    public void PickRandomDestination()
    {
        Vector3 dest = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
        agent.SetDestination(dest);
        Task.current.Succeed();
    }
    [Task]
    public void MoveToDestination()
    {
        if (Task.isInspected)
            Task.current.debugInfo = string.Format("t={0:0.00}", Time.time);
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            Task.current.Succeed();
        }
    }

    [Task]
    public bool IsHealthLessThan(float health)
    {
        return this.health < health;
    }


    //metodo que auto destroi
    [Task]
    public bool Explode()
    {
        Destroy(healthBar.gameObject);
        Destroy(this.gameObject);
        return true;
    }

    //metodo que faz ver o jogador
    [Task]
    public bool SeePlayer()
    {
        return true;
    }


    //metodo que faz ir até o jogador
    [Task]
    public void TargetPlayer()
    {
        agent.SetDestination(dest);
        Task.current.Succeed();
    }

    [Task]

    //metodo que faz olhar o jogador
    public void LookAtTarget()
    {
        Task.current.Succeed();
    }

    //metodo que faz atirar
    [Task]
    public void Fire()
    {
        bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
        Task.current.Succeed();
    }
}

