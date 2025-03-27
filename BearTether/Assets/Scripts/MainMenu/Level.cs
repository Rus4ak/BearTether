[System.Serializable]
public class Level
{
    public int LevelID;
    public int countStars;
    public bool isCompleted;

    public Level(int levelID, int countStars, bool isCompleted = false)
    {
        this.LevelID = levelID;
        this.countStars = countStars;
        this.isCompleted = isCompleted;
    }
}
