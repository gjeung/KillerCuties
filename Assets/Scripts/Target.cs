using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour{
    [Header("Graphics")]
    [SerializeField] private Sprite target;
    [SerializeField] private Sprite targetHit;
    public AudioSource animalSounds;
    public AudioClip targetSound;
    
    [Header("GameManager")]
    [SerializeField] private GameManager gameManager;

    // hide target
    private Vector2 startPosition = new Vector2(0f, -1.35f);
    private Vector2 endPosition = Vector2.zero;
    // popup duration
    private float showDuration = 0.5f;
    private float duration = 2f;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;
    private Vector2 boxOffset;
    private Vector2 boxSize;
    private Vector2 boxOffsetHidden;
    private Vector2 boxSizeHidden;

    // target parameters
    private bool hittable = true;
    private int targetIndex = 0;

    private IEnumerator ShowHide(Vector2 start, Vector2 end) {
        transform.localPosition = start;

        // show target
        float elapsed = 0f;
        while (elapsed < showDuration) {
            transform.localPosition = Vector2.Lerp(start, end, elapsed / showDuration);
            boxCollider2D.offset = Vector2.Lerp(boxOffsetHidden, boxOffset, elapsed / showDuration);
            boxCollider2D.size = Vector2.Lerp(boxSizeHidden, boxSize, elapsed / showDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = end;
        boxCollider2D.offset = boxOffset;
        boxCollider2D.size = boxSize;
        yield return new WaitForSeconds(duration);

        // hide target
        elapsed = 0f;
        while (elapsed < showDuration) {
            transform.localPosition = Vector2.Lerp(end, start, elapsed / showDuration);
            boxCollider2D.offset = Vector2.Lerp(boxOffset, boxOffsetHidden, elapsed / showDuration);
            boxCollider2D.size = Vector2.Lerp(boxSize, boxSizeHidden, elapsed / showDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = start;
        boxCollider2D.offset = boxOffsetHidden;
        boxCollider2D.size = boxSizeHidden;

        // if a target is still hittable at the end of duration, set to "missed"
        if (hittable) {
            hittable = false;
            gameManager.Missed(targetIndex);
        }
    }

    private void Awake() {
        // set target image
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        // set collider values
        boxOffset = boxCollider2D.offset;
        boxSize = boxCollider2D.size;
        boxOffsetHidden = new Vector2(boxOffset.x, -startPosition.y / 2f);
        boxSizeHidden = new Vector2(boxSize.x, 0f);
    }

    private void OnMouseDown() {
        if (hittable) {
            spriteRenderer.sprite = targetHit;
            animalSounds.PlayOneShot(targetSound);
            gameManager.AddScore(targetIndex);
            // stop movement
            StopAllCoroutines();
            StartCoroutine(QuickHide());
            // turn off hittable so that user can't click target anymore
            hittable = false;
        }
    }

    private IEnumerator QuickHide() {
        yield return new WaitForSeconds(0.5f);
        // prevent flickering if it spawns in this location again
        if (!hittable) {
            Hide();
        }
    }

    public void Hide() {
        // set target parameters to hide it
        transform.localPosition = startPosition;
        boxCollider2D.offset = boxOffsetHidden;
        boxCollider2D.size = boxSizeHidden;
    }

    private void CreateNext() {
        spriteRenderer.sprite = target;
        // set new target as hittable
        hittable = true;
    }

    // make targets show up more for a shorter duration as levels increase
    private void SetLevel(int level) {
        // time target is revealed decreases as levels increase
        float durationMin = Mathf.Clamp(1 - level * 0.1f, 0.01f, 1f);
        float durationMax = Mathf.Clamp(2 - level * 0.1f, 0.01f, 2f);
        duration = Random.Range(durationMin, durationMax);
    }

    // used by game manager to identify targets
    public void SetIndex(int index) {
        targetIndex = index;
    }

    public void Activate(int level) {
        SetLevel(level);
        CreateNext();
        StartCoroutine(ShowHide(startPosition, endPosition));
    }
    
    // freeze game when finish
    public void StopGame() {
        hittable = false;
        StopAllCoroutines();
    }
}
