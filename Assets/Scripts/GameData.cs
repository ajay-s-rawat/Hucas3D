using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    public int MatchesFound { get; set; }
    public int TotalPairs { get; set; }
    public int MovesMade { get; set; }
    public int Score { get; set; }
    public int Combo { get; set; }
    public CardData[] CardStates { get; set; }

}

[Serializable]
public class CardData
{
    public int CardID { get; set; }
    public bool IsFaceUp { get; set; }

}
