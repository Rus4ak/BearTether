[System.Serializable]
public class MultiplayerLevel
{
    public int LevelID;
    public int countStars;
    public bool isCompleted;

    public MultiplayerLevel(int levelID, int countStars, bool isCompleted = false)
    {
        this.LevelID = levelID;
        this.countStars = countStars;
        this.isCompleted = isCompleted;
    }
}
