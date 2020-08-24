using System;

namespace FWTL.TelegramClient.Exceptions
{
    public class TelegramSessionNotFoundException : Exception
    {
        public TelegramSessionNotFoundException(string url) : base($"Session not found for request: {url}")
        {
        }
    }
}