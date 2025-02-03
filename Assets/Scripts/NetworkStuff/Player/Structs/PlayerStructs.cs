using Fusion; 
public struct PlayerScore
{
    public int kills;
    public int deahts;
}

public struct PlayerInfo
{
    public PlayerRef id;
    public PlayerScore score;

    public PlayerInfo(PlayerRef p, PlayerScore s)
    {
        id = p;
        score = s;
    }
}