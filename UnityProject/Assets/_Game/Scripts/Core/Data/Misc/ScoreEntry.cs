using System;

namespace _Game.Core.Data
{
    [Serializable]
    public class ScoreEntry
    {
        public string Timestamp;
        public string Description;
        public int Amount;

        // Optional: Helper property to parse the timestamp back to a DateTime.
        public DateTime DateTimeValue
        {
            get
            {
                DateTime dt;
                return DateTime.TryParse(Timestamp, null, System.Globalization.DateTimeStyles.RoundtripKind, out dt)
                    ? dt
                    : default(DateTime);
            }
        }
    }
}