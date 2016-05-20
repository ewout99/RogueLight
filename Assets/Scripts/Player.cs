using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class Player : MoveObject {

    // Vraibles for gameplay
    public int wallDamage = 1;
    public int enemyDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    public float shakeIntesity;

    //Mobile
    private Vector2 touchOrigin = -Vector2.one;

    // UI refrences
    private Text foodText;

    // animations
    private Animator aniRef;
    private SpriteRenderer spRef;
    private ParticleSystem parSysRef;
    private Camera camRef;
    public int food;

    // Audio Clips
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;

    // Blood referencs
    [SerializeField]
    private GameObject permenantBlood;
    [SerializeField]
    private GameObject slashSprite;

    // Use this for initialization
    protected override void Start()
    {
        foodText = GameObject.Find("Food Text").GetComponent<Text>();
        aniRef = GetComponent<Animator>();
        spRef = GetComponent<SpriteRenderer>();
        camRef = GetComponentInChildren<Camera>();
        food = GameManager.instance.playerFoodPoints;
        foodText.text = "Food: " + food;
        base.Start();
    }

    private void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food;
    }

    // Update is called once per frame
    void Update() {

        if (!GameManager.instance.playersTurn)
        {
            aniRef.SetBool("Walking", false);
            return;
        }

        int horizontal = 0;
        int vertical = 0;

#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR

        horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        vertical = (int)(Input.GetAxisRaw("Vertical"));
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

        Vector2 moveVecInput = new Vector2(horizontal, vertical);

        if (moveVecInput.x > 0 || moveVecInput.x < 0 || moveVecInput.y > 0 || moveVecInput.y < 0)
        {
            aniRef.SetBool("Walking", true);
         

            if (moveVecInput.x < 0)
            {
                spRef.flipX = true;
            }
            else if (moveVecInput.x > 0)
            {
                spRef.flipX = false;
            }
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        aniRef.SetTrigger("PlayerChop");
    }

    private void Restart()
    {
        SceneManager.LoadScene("Starting Scene");
        //Application.LoadLevel(Application.loadedLevel);  
    }

    public void LoseFood(int loss)
    {
        aniRef.SetTrigger("PlayerHit");
        food -= loss;
        foodText.text = "-" + loss + ", Food: " + food;
        StartCoroutine(StartScreenShake());
        StartCoroutine(ColorFlash());
        CheckIfGameOver();
        Instantiate(permenantBlood, transform.position, Quaternion.identity);
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
        else
        {
            // Raycast in the walk direction
            if (hit.transform.tag == "Enemy")
            {
                aniRef.SetTrigger("PlayerChop");
                hit.transform.gameObject.GetComponent<Enemy>().DamageEnemy(enemyDamage);
                slashSprite.SetActive(true);
                Invoke("DeActivate", 0.1f);
            }
        }

        CheckIfGameOver();

        GameManager.instance.playersTurn = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Exit")
        {
            Debug.Log("Fade Called");
            GameManager.instance.levelImage.SetActive(true);
            FadeInAndOut.instance.Reset();
            FadeInAndOut.instance.NextPanel();
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
        else if (other.tag == "Soda")
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
        Vector3 ogPos = camRef.transform.position;
        Vector3 newPos = ogPos;
        for (int i = 0; i < 10; i++)
        {
            float shakeAmount = Random.Range(-shakeIntesity, shakeIntesity) * Time.deltaTime;
            if (i % 2 > 0)
            {
                newPos.x += shakeAmount;
                newPos.y += shakeAmount;
            }
            else
            {
                newPos.x -= shakeAmount;
                newPos.y -= shakeAmount;
            }
            camRef.gameObject.transform.position = newPos;
            yield return new WaitForSeconds(0.03f);
        }
        camRef.gameObject.transform.position = gameObject.transform.position;
    }

    IEnumerator ColorFlash()
    {
        Color Hold = camRef.backgroundColor;
        spRef.color = Color.yellow;
        camRef.backgroundColor = Color.white;
        yield return new WaitForSeconds(0.1f);
        spRef.color = Color.white;
        camRef.backgroundColor = Hold;
    }

    void DeActivate()
    {
        slashSprite.SetActive(false);
    }
}
