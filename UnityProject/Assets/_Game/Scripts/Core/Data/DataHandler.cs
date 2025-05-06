using System;
using System.Collections.Generic;
using System.IO;
using _Game.Systems.FinanceSystem;
using UnityEngine;

namespace _Game.Systems.Core.Data
{
    public class DataHandler
    {
        private const string SaveFileName = "GameData.json";
        public GameData Data { get; private set; }

        public DataHandler()
        {
            Debug.Log("[GameDataManager] Initialized.");
            Load();
        }

        public void Load()
        {
            string path = Path.Combine(Application.dataPath, "_Game/Data/SaveFiles", SaveFileName);
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                Data = JsonUtility.FromJson<GameData>(json);
                Debug.Log("[GameDataManager] Loaded data: " + json);
            }
            else
            {
                Data = new GameData()
                {
                    PlayerName = "Player",
                    Level = 1,
                    Score = 0,
                };
                Debug.Log("[DataHandler] No saved file found. Initializing with default data.");
                Save();
            }
        }

        public void Reset()
        {
            Debug.Log("[DataHandler] Resetting financial data.");
            Data = new GameData()
            {
                PlayerName = "Player",
                Level = 1,
                Score = 0,
            };
            Save();
        }

        public void Save()
        {
            string json = JsonUtility.ToJson(Data, true);
            string path = Path.Combine(Application.dataPath, "_Game/Data/SaveFiles", SaveFileName);
            File.WriteAllText(path, json);
            Debug.Log("[DataHandler] Saved data: " + json);
        }

        public void UpdateScore(int amount, string description)
        {
            Data.Score += amount;
            ScoreEntry entry = new ScoreEntry
            {
                // Store the current time in ISO‑8601 (round-trip) format.
                Timestamp = DateTime.Now.ToString("o"),
                Description = description,
                Amount = amount
            };
            Data.Scores.Add(entry);
            Save();
        }
    }
}