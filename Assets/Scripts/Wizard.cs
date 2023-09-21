using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : MonoBehaviour
{
    [Header("Graphics")]
    [SerializeField] private Sprite wizard;
    [SerializeField] private float speed;
    [SerializeField] private float range;
    [SerializeField] private float maxDistance;

    public AudioSource wizardSounds;
    public AudioClip targetSound;
    private SpriteRenderer spriteRenderer;
    Vector2 wayPoint;
    
    [Header("GameManager")]
    [SerializeField] private GameManager gameManager;

    void Start() {
        SetNewDestination();
    }

    void Update() {
        transform.position = Vector2.MoveTowards(transform.position, wayPoint, speed * Time.deltaTime);
        if(Vector2.Distance(transform.position, wayPoint) < range) {
            SetNewDestination();
        }
    }

    void SetNewDestination() {
        wayPoint = new Vector2(Random.Range(-maxDistance - 3, maxDistance + 3), Random.Range(-maxDistance + 1, maxDistance - 2));
        }

    private void OnMouseDown() {
        //play wizard sound and add point to score everytime user clicks wizard
        wizardSounds.PlayOneShot(targetSound);
        gameManager.AddBonusScore();
    }

    public void Activate() {
        spriteRenderer.sprite = wizard;
    }
    
    // Freeze game when finish
    public void StopGame() {
        StopAllCoroutines();
    }
}
