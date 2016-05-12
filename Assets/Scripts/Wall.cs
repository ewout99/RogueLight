using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {

   // Health of the wall
    public int hp = 4;

    // Refences
    private SpriteRenderer spriteRef;
    public Sprite dmgSprite;

    //Audio
    public AudioClip chop1;
    public AudioClip chop2;

    // Use this for initialization
    void Awake () {
        spriteRef = GetComponent<SpriteRenderer>();
	}

    public void DamageWall(int loss)
    {
        spriteRef.sprite = dmgSprite;
        hp -= loss;
        SoundManager.instance.RandomSfx(chop1, chop2);
        if (hp <= 0)
            gameObject.SetActive(false);
    }
}
