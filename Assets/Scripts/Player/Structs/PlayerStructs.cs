using Fusion; 
public struct PlayerScore : INetworkStruct
{
    public int kills;
    public int deahts;
}

public struct PlayerInfo : INetworkStruct
{
    public PlayerRef id;
    public PlayerScore score;
    public bool PlayerActive; 

    public PlayerInfo(PlayerRef p, PlayerScore s)
    {
        id = p;
        score = s;
        PlayerActive = false; 
    }
}