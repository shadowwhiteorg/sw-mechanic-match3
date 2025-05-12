namespace _Game.Systems.UISystem.Move
{
    public class MoveUIModel : BaseUIModel
    {
        public int MovesLeft { get; private set; }

        /// <summary>
        /// Update the moves‐left count and notify any bound views.
        /// </summary>
        public void SetMoves(int moves)
        {
            MovesLeft = moves;
            NotifyUpdated();
        }
    }
}