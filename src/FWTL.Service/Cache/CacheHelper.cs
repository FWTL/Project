using FWTL.Domain.Traits;
using FWTL.Domain.Users;
using FWTL.TelegramClient.Responses;

namespace FWTL.Domain.Cache
{
    public static class CacheHelper
    {
        public static string GetDialogsAsync(GetDialogs.Query query) => $"GetDialogsAsync.{query.SessionName()}";

        public static string GetInfoAsync(GetDialogs.Query query, Dialog dialog) => $"GetInfoAsync.{query.SessionName()}.{dialog.Type}.{dialog.Id}";
    }
}