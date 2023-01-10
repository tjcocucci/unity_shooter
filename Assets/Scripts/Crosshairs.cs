using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshairs : MonoBehaviour
{

    public float rotationSpeed = 50;
    public LayerMask enemyMask;
    public SpriteRenderer dot;
    Color originalColor;
    public Color overEnemyColor;


    void Start() {
        print(dot.color);
        originalColor = dot.color;
    }
    void Update() {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    public void DetectTargets(Ray ray) {
        if (Physics.Raycast(ray, 100, enemyMask)) {
            dot.color = overEnemyColor;
        } else {
            dot.color = originalColor;
        }
    }

}
