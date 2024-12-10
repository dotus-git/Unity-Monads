using Monads;
using TMPro;
using UniMediator;
using UnityEngine;

public class ScoreSystem : 
    MonoBehaviour, 
    IMulticastMessageHandler<AddScore>,
    IMulticastMessageHandler<NewGame>
{
    public TextMeshProUGUI ScoreText; // Assign this in the inspector

    private int score;

    private void Start()
    {
        ScoreText.text = score.ToString();
    }

    public void Handle(AddScore message)
    {
        ScoreText
            .ToResult() // if we forgot to assign the ScoreText, we don't want to crash the game
            .Do(ui => {
                score += message.Amount;
                ui.text = score.ToString();
            });
    }

    public void Handle(NewGame message)
    {
        score = 0;
    }
}
