using Monads;
using TMPro;
using UnityUtils;

public class ScoreSystem : 
    Singleton<ScoreSystem>
{
    public TextMeshProUGUI ScoreText; // Assign this in the inspector

    private int score;

    private void Start()
    {
        ScoreText.text = score.ToString();
    }

    [MediatorHandler]
    public void Handle(AddScore message)
    {
        ScoreText
            .ToResult() // if we forgot to assign the ScoreText, we don't want to crash the game
            .OnSuccess(ui => {
                score += message.Amount;
                ui.text = score.ToString();
            });
    }

    [MediatorHandler]
    public void Handle(NewGame message)
    {
        score = 0;
    }
}
