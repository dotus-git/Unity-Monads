using UniMediator;
using UnityUtils;

public class GameManager : Singleton<GameManager>
{
    private void NewGame()
    {
        Mediator.Publish(new NewGame());
    }

}
