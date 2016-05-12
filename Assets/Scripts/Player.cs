using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class Player : MoveObject {

    // Vraibles for gameplay
    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;

    //Mobile
    private Vector2 touchOrigin = -Vector2.one;

    // UI refrences
    public Text foodText;

    // animations
    private Animator AniRef;
    private int food;

    // Audio Clips
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;


    // Use this for initialization
    protected override void Start ()
    {
        AniRef = GetComponent<Animator>();
        food = GameManager.instance.playerFoodPoints;
        foodText.text = "Food: " + food;
        base.Start();
	}
	
    private void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food;
    }

	// Update is called once per frame
	void Update () {

        if (!GameManager.instance.playersTurn)
        {
            return;
        }

        int horizontal = 0;
        int vertical = 0;

#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR

        horizontal = (int) (Input.GetAxisRaw("Horizontal"));
        vertical = (int) (Input.GetAxisRaw("Vertical"));
        // Debug.Log("Hor input: " + horizontal + " Ver input: " + vertical);

        if (horizontal != 0)
        {
            vertical = 0;
        }
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        if (Input.touchCount > 0)
        {
            Touch firstTouch = Input.touches[0];
            if (firstTouch.phase == TouchPhase.Began)
            {
                touchOrigin = firstTouch.position;
            }
            else if (firstTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
            {
                Vector2 touchEnd = firstTouch.position;
                float x = touchEnd.x - touchOrigin.x;
                float y = touchEnd.y - touchOrigin.y;
                touchOrigin.x = -1;
                if (Mathf.Abs(x) > Mathf.Abs(y))
                    horizontal = x > 0 ? 1 : -1;
                else
                    vertical = y > 0 ? 1 : -1;
            }
        }
#endif
        if (horizontal != 0 || vertical != 0)
        {
            AttemptMove<Wall>(horizontal, vertical);
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        AniRef.SetTrigger("PlayerChop");
    }

    private void Restart()
    {
        Application.LoadLevel(Application.loadedLevel);  
    }

    public void LoseFood (int loss)
    {
        AniRef.SetTrigger("PlayerHit");
        food -= loss;
        foodText.text = "- " + loss + "Food: " + food;
        StartCoroutine(StartScreenShake());
        CheckIfGameOver();
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        food--;
        foodText.text = "Food: " + food;

        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;

        if (Move(xDir, yDir, out hit))
        {
            SoundManager.instance.RandomSfx(moveSound1, moveSound2);
        }

        CheckIfGameOver();

        GameManager.instance.playersTurn = false;
    }

    private void OnTriggerEnter2D (Collider2D other)
    {
        if (other.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else if (other.tag == "Food")
        {
            food += pointsPerFood;
            foodText.text = "+" + pointsPerFood + ", Food: " + food;
            SoundManager.instance.RandomSfx(eatSound1, eatSound2);
            other.gameObject.SetActive(false);
        }
        else if (other.tag =="Soda")
        {
            food += pointsPerSoda;
            foodText.text = "+" + pointsPerSoda + ", Food: " + food;
            SoundManager.instance.RandomSfx(drinkSound1, drinkSound2);
            other.gameObject.SetActive(false);
        }
    }

    private void CheckIfGameOver()
    {
        if (food <= 0)
        {
            SoundManager.instance.PlaySingle(gameOverSound);
            GameManager.instance.GameOver();
        }
    }

    
    IEnumerator StartScreenShake()
    {
        Vector3 camRef = Camera.main.transform.position;
        Vector3 ogPos = camRef;
        for (int i = 0; i < 10; i++)
        {
            float shakeAmount = Random.Range(-1,1) * Time.deltaTime;
            camRef.x += shakeAmount;
            camRef.y += shakeAmount;
            Camera.main.transform.position = camRef;
            yield return null;
        }
        Camera.main.transform.position = ogPos;
    }
}
