namespace FWTL.Domain.Traits
{
    public interface IPagingTrait
    {
        int Start { get; set; }
        int Limit { get; set; }
    }
}