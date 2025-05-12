using System;
using System.Collections.Generic;

namespace _Game.Core.Data
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