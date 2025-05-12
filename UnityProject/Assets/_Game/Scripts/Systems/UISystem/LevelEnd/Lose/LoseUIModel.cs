namespace _Game.Systems.UISystem
{
    public class LoseUIModel : BaseUIModel
    {
        public string Message { get; private set; }

        public void SetMessage(string message)
        {
            Message = message;
            NotifyUpdated();
        }
    }
}