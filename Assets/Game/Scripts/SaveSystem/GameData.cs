[System.Serializable]
public class GameData
{
    // Mode
    public bool isFreeMode;

    // Spline mode
    public string splineName;
    public float distanceOnSpline;

    // Free mode
    public float posX;
    public float posY;
    public float posZ;

    // Commun
    public int enemiesAttached;
}