using System;
using System.Collections.Generic;
using _Game.Systems.Core.Data;

namespace _Game.Systems.FinanceSystem
{
    [Serializable]
    public class GameData
    {
        public string PlayerName;
        public int Level;
        public int Score;
        public  List<ScoreEntry> Scores = new List<ScoreEntry>();
        
    }
}