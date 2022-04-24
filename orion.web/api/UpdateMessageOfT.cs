using System;
namespace orion.web.api
{
    public class UpdateMessage
    {
        public static UpdateMessage<TNewValue,TExisting> CreateFrom<TNewValue,TExisting>(TNewValue newValue, TExisting existing)
        {
            return new UpdateMessage<TNewValue, TExisting>(newValue, existing);
        }
    }
    public class UpdateMessage<TNewValue, TExisting>
    {
        public UpdateMessage(TNewValue newValue, TExisting existing)
        {
            NewValue = newValue;
            Existing = existing;
        }

        public TNewValue NewValue { get; }
        public TExisting Existing { get; }
    }
}

