using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ScoreEntryUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI playTimeText;

    public void SetScoreInfo(int score, string characterName, TimeSpan playTime)
    {
        scoreText.text = score.ToString();
        characterNameText.text = characterName;
        playTimeText.text = $"{playTime.Hours:D2}:{playTime.Minutes:D2}:{playTime.Seconds:D2}";
    }
}
