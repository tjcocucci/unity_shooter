using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public float forceMin = 4;
    public float forceMax = 7;
    public float fadeTime = 1;
    public float lifetime = 3;

    Rigidbody myRigidbody;

    void Start() {
        myRigidbody = GetComponentInChildren<Rigidbody> ();
        DiscardShell();
    }

    void DiscardShell() {
        float forceMagnitude = Random.Range(forceMin, forceMax);
        myRigidbody.AddForce(transform.right * forceMagnitude, ForceMode.Impulse);
        myRigidbody.AddTorque(Random.insideUnitSphere * forceMagnitude);

        StartCoroutine(Fade());
    }


    IEnumerator Fade () {
        yield return new WaitForSeconds(lifetime);

        float fadeSpeed = 1 / fadeTime;
        float percent = 0;
        Material mat = GetComponentInChildren<Renderer>().material;
        Color originalColor = mat.color;
        Color transparentColor = originalColor;
        transparentColor.a = 0;

        while (percent <= 1) {
            percent += Time.deltaTime;
            mat.color = Color.Lerp(originalColor, transparentColor, percent);
            yield return null;
        }
        Destroy(gameObject);

    }
}
