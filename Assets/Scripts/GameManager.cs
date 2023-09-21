using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    [SerializeField] private List<Target> targets;

    [Header("UI objects")]
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject menuButton;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private Image uiFill;
    [SerializeField] private GameObject instruction;
    [SerializeField] private TMPro.TextMeshProUGUI timeText;
    [SerializeField] private TMPro.TextMeshProUGUI totalScoreText;
    [SerializeField] private TMPro.TextMeshProUGUI purifyScoreText;
    [SerializeField] private TMPro.TextMeshProUGUI bonusScoreText;
    [SerializeField] private TMPro.TextMeshProUGUI gameOverTotalScoreText;
    [SerializeField] private GameObject rabbitBush;
    [SerializeField] private GameObject foxTent;
    [SerializeField] private GameObject gooseBasket;
    [SerializeField] private GameObject skunkFire;
    [SerializeField] private GameObject bonusMessage;
    [SerializeField] private GameObject wizard;

    // set starting time and bonus round time
    private float startingTime = 31f;
    private float bonusTime = 11f;

    private float timeRemaining;
    private HashSet<Target> currentTargets = new HashSet<Target>();
    private int totalScore;
    private int purifyScore;
    private int bonusScore;
    private bool playing = false;
    private float bonusTimeRemaining;

    // public so play button can see this info
    public void StartGame() {
        // hide the UI after user clicks play button
        playButton.SetActive(false);
        menuButton.SetActive(false);
        instruction.SetActive(false);
        gameOver.SetActive(false);
        wizard.SetActive(false);
        gameOverTotalScoreText.text = string.Empty;
        purifyScoreText.text = string.Empty;
        bonusScoreText.text = string.Empty;
        // show hidden animals and objects
        rabbitBush.SetActive(true);
        foxTent.SetActive(true);
        gooseBasket.SetActive(true);
        skunkFire.SetActive(true);

        // hide all targets
        for (int i = 0; i < targets.Count; i++) {
            targets[i].Hide();
            targets[i].SetIndex(i);
        }
        // remove old game stats, set start time = 30, bonus time = 10
        currentTargets.Clear();
        timeRemaining = startingTime;
        bonusTimeRemaining = bonusTime;
        totalScore = 0;
        purifyScore = 0;
        bonusScore = 0;
        totalScoreText.text = "0";
        playing = true;
    }

    public void GameOver(int type) {
        // show gameover message and game stats
        if (type == 0) {
            gameOver.SetActive(true);
            gameOverTotalScoreText.text = $"{totalScore}";
            purifyScoreText.text = $"{purifyScore}";
            bonusScoreText.text = $"{bonusScore}";
        } 
        // stop game, show UI, hide assets covering gameover menu
        playing = false;
        playButton.SetActive(true);
        menuButton.SetActive(true);
        rabbitBush.SetActive(false);
        foxTent.SetActive(false);
        gooseBasket.SetActive(false);
        skunkFire.SetActive(false);
        bonusMessage.SetActive(false);
        wizard.SetActive(false);
    }

    void BonusRound(int type) {
        // show bonus round message
        bonusMessage.SetActive(true);
        // hide all targets
        foreach (Target target in targets) {
            target.StopGame();
        }
        for (int i = 0; i < targets.Count; i++) {
            targets[i].Hide();
            targets[i].SetIndex(i);
        }

        // when bonus time duration ends, go to gameover, else wizard is true
        if (bonusTimeRemaining <= 0) {
                bonusTimeRemaining = 0;
                // go to GameOver
                GameOver(0);
        } else {
            wizard.SetActive(true);
        }
    }

    void Update() {
        if (playing) {
            // update time and timer visuals for main time and bonus round time
            if (timeRemaining <= 0) {
                timeRemaining = 0;
                timeText.text = $"{(int)bonusTimeRemaining % 60:D2}";
                uiFill.fillAmount = Mathf.InverseLerp(0, bonusTime, bonusTimeRemaining);
                bonusTimeRemaining -= Time.deltaTime;
                // Go to BonusRound
                BonusRound(0);
            } else {
                timeText.text = $"{(int)timeRemaining % 60:D2}";
                uiFill.fillAmount = Mathf.InverseLerp(0, startingTime, timeRemaining);
                timeRemaining -= Time.deltaTime;
            }

            // make a target pop up
            if (currentTargets.Count <= (totalScore / 10)) {
                // choose random targets
                int index = Random.Range(0, targets.Count);
                if (!currentTargets.Contains(targets[index])) {
                    currentTargets.Add(targets[index]);
                    targets[index].Activate(totalScore / 10);
                }
            }
        }
    }

    // main score and total score
    public void AddScore(int targetIndex) {
        purifyScore += 1;
        // add and update total score
        totalScore += 1;
        totalScoreText.text = $"{totalScore}";
        // remove target from active targets
        currentTargets.Remove(targets[targetIndex]);
    }

    // bonus score
    public void AddBonusScore() {
        bonusScore += 1;
        // add and update total score
        totalScore += 1;
        totalScoreText.text = $"{totalScore}";
    }

    public void Missed(int targetIndex) {
        // remove from active targets
        currentTargets.Remove(targets[targetIndex]);
    }
}