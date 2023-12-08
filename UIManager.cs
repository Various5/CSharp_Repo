using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI logText;
    public TextMeshProUGUI animalCountText;
    public TextMeshProUGUI killsHighscoreText;

    private List<string> logMessages = new List<string>();
    private int maxLogMessages = 5; // Maximum number of log messages to display

    private void Awake()
    {
        Animal.OnAnimalBirth += UpdateLog;
        Animal.OnAnimalDeath += UpdateLog;
    }

    private void OnDestroy()
    {
        Animal.OnAnimalBirth -= UpdateLog;
        Animal.OnAnimalDeath -= UpdateLog;
    }

    private void UpdateLog(string message)
    {
        logMessages.Add(message);
        UpdateLogText();
    }

    private void UpdateLogText()
    {
        while (logMessages.Count > maxLogMessages)
        {
            logMessages.RemoveAt(0); // Remove the oldest message
        }

        logText.text = string.Join("\n", logMessages.ToArray());
    }

    public void UpdateAnimalCountScoreboard(Dictionary<string, int> animalCounts)
    {
        animalCountText.text = "Animal Counts:\n";
        foreach (var count in Animal.SpeciesCount)
        {
            animalCountText.text += count.Key + ": " + count.Value + "\n";
        }
    }

    public void UpdateKillsScoreboard()
    {
        killsHighscoreText.text = "Kills Highscore:\n";
        foreach (var item in Animal.KillCounts.OrderByDescending(key => key.Value))
        {
            killsHighscoreText.text += item.Key + ": " + item.Value + "\n";
        }
    }
}
