using UnityEngine;
using System.Collections;
using System;

public class Enemy : MoveObject{

    // Damage
    public int playerDmg;
    public int health = 3;

    //Refrences
    private Animator aniRef;
    private SpriteRenderer spRef;
    private Transform target;
    private bool skipBool;

    // Sfx
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;

    // Blood particle
    [SerializeField]
    private GameObject permenantBlood;

    // Use this for initialization
    protected override void Start () {

        GameManager.instance.AddEnemyList(this);
        aniRef = GetComponent<Animator>();
        spRef = GetComponent<SpriteRenderer>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
	}

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (skipBool)
        {
            skipBool = false;
            return;
        }

        base.AttemptMove<T>(xDir, yDir);

        skipBool = true;
    }

    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;

        aniRef.SetTrigger("EnemyAttack");
        SoundManager.instance.RandomSfx(enemyAttack1, enemyAttack2);
        hitPlayer.LoseFood(playerDmg);
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;
        if (Mathf.Abs (target.position.x-transform.position.x)< float.Epsilon)
        {
            yDir = target.position.y > transform.position.y ? 1 : -1;
        }
        else
        {
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }

        // Flip sprite over x axis depeding on the move direction
        if ( xDir == -1)
        {
            spRef.flipX = false;
        }
        else
        {
            spRef.flipX = true;
        }

        AttemptMove<Player>(xDir, yDir);
    }
    
    public void DamageEnemy(int amount)
    {
        health -= amount;
        Instantiate(permenantBlood, transform.position, Quaternion.identity);
        StartCoroutine(CharacterFlash());
    }

    IEnumerator CharacterFlash()
    {
        if (health >= 0)
        {
            GetComponent<SpriteRenderer>().color = Color.yellow;
            yield return new WaitForSeconds(0.1f);
            GetComponent<SpriteRenderer>().color = Color.white;
        }

        if (health == 0)
        {
            GetComponent<SpriteRenderer>().color = Color.red;
            GameManager.instance.enemiesList.Remove(this);
            Destroy(gameObject, 0.1f);
        }
    }
}
