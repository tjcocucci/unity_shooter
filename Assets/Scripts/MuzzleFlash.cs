using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{

    public GameObject muzzleFlashPrefab;
    public Sprite[] sprites;
    public SpriteRenderer[] spriteRenderersHolder;
    float flashDuration = 0.1f;


    void Start() {
        Deactivate();
    }

    public void Activate() {
        muzzleFlashPrefab.SetActive(true);
        int randomIndex = Random.Range(0, sprites.Length);
        for (int i=0; i < spriteRenderersHolder.Length; i++) {
            spriteRenderersHolder[i].sprite = sprites[randomIndex];
        }
        Invoke("Deactivate", flashDuration);
    }

    void Deactivate() {
        muzzleFlashPrefab.SetActive(false);
        
    }
}
