namespace MassTransitTests.DataTransferObjects.Commands
{
    /// <summary>
    /// This command is supposed to be sent to only one receiver, while multiple may be listening.
    /// The message broker decides which subscriber should read the message by using the round robin algorithm.
    /// </summary>
    public class SendToOneOfManyCommand
    {
    }
}
