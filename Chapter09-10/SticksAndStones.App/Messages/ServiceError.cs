using CommunityToolkit.Mvvm.Messaging.Messages;

namespace SticksAndStones.Messages;

internal class ServiceError : ValueChangedMessage<AsyncError>
{
    public ServiceError(AsyncError error) : base(error)
    {
    }
}
