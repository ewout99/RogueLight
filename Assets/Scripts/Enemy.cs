using UnityEngine;
using System.Collections;
using System;

public class Enemy : MoveObject{

    // Damage
    public int playerDmg;

    //Refrences
    private Animator aniRef;
    private Transform target;
    private bool skipBool;

    // Sfx
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;

    // Use this for initialization
    protected override void Start () {

        GameManager.instance.AddEnemyList(this);
        aniRef = GetComponent<Animator>();
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

        AttemptMove<Player>(xDir, yDir);
    }
}
