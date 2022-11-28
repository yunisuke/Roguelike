using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public Sprite dmgSprite;
    public int hp =3;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void DamageWall(int losss)
    {
        spriteRenderer.sprite = dmgSprite;

        hp -= losss;
        if (hp <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
