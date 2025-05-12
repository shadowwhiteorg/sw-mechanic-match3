namespace _Game.Systems.UISystem.Move
{
    public class MoveUIModel : BaseUIModel
    {
        public int MovesLeft { get; private set; }
        
        public void SetMoves(int moves)
        {
            MovesLeft = moves;
            NotifyUpdated();
        }
    }
}