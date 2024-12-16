using UnityUtils;

public class GameManager : Singleton<GameManager>
{
    private void NewGame()
    {
        DataMediator.Instance.Publish(new NewGame());
    }

}
