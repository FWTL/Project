using FWTL.Core.Events;

namespace FWTL.Events
{
    public class UserRegistered : IEvent
    {
        public string Email { get; set; }

        public string ActivationCode { get; set; }
    }
}