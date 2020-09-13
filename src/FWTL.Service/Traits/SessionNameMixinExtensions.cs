namespace FWTL.Domain.Traits
{
    public static class SessionNameMixinExtensions
    {
        public static string SessionName(this ISessionNameTrait @that)
        {
            return @that.SessionName;
        }
    }
}