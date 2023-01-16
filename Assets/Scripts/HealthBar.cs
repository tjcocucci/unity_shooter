using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{

    public RectTransform healthBarRect;
    Player player;
    float healthPercent;

    void Start() {
        player = FindObjectOfType<Player>();
    }

    void Update() {
        healthPercent = 0;
        if (player != null) {
            healthPercent = player.health / player.startingHealth;
        }
        healthBarRect.localScale = new Vector3 (healthPercent, 1, 1);
    }
}
