using System;

namespace FWTL.TelegramClient.Exceptions
{
    public class TelegramSessionNotFoundException : Exception
    {
        public TelegramSessionNotFoundException() : base($"Session not found")
        {
        }
    }
}