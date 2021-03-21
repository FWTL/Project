namespace FWTL.Core.Services.Telegram.Dto
{
    public class Dialog
    {
        public int Id { get; set; }
        public DialogType Type { get; set; }

        public enum DialogType
        {
            User = 1,
            Chat = 2,
            Channel = 3
        }
    }
}