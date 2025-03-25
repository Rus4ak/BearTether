[System.Serializable]
public class Level
{
    public int LevelID;
    public int countStars;

    public Level(int levelID, int countStars)
    {
        this.LevelID = levelID;
        this.countStars = countStars;
    }
}
