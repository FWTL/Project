using System;

namespace FWTL.Common.Exceptions
{
    public class TelegramSessionNotFoundException : Exception
    {
        public TelegramSessionNotFoundException() : base($"Session not found")
        {
        }
    }
}