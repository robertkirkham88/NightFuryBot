namespace NFB.Service.Flight.Tests.TestFactory
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using GreenPipes;

    using MassTransit;

    public class ConsumerContextTestObject<TMessage> : ConsumeContext<TMessage> where TMessage : class
    {
        #region Public Constructors

        public ConsumerContextTestObject(TMessage message)
        {
            this.Message = message;
        }

        #endregion Public Constructors

        #region Public Properties

        public virtual CancellationToken CancellationToken { get; }

        public virtual Task ConsumeCompleted { get; }

        public virtual Guid? ConversationId { get; }

        public virtual Guid? CorrelationId { get; }

        public virtual Uri DestinationAddress { get; }

        public virtual DateTime? ExpirationTime { get; }

        public virtual Uri FaultAddress { get; }

        public virtual Headers Headers { get; }

        public virtual HostInfo Host { get; }

        public virtual Guid? InitiatorId { get; }

        public TMessage Message { get; }

        public virtual Guid? MessageId { get; }

        public virtual ReceiveContext ReceiveContext { get; }

        public virtual Guid? RequestId { get; }

        public virtual Uri ResponseAddress { get; }

        public virtual DateTime? SentTime { get; }

        public virtual Uri SourceAddress { get; }

        public virtual IEnumerable<string> SupportedMessageTypes { get; }

        #endregion Public Properties

        #region Public Methods

        public virtual void AddConsumeTask(Task task)
        {
            throw new NotImplementedException();
        }

        public virtual T AddOrUpdatePayload<T>(PayloadFactory<T> addFactory, UpdatePayloadFactory<T> updateFactory)
            where T : class
        {
            throw new NotImplementedException();
        }

        public virtual ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            throw new NotImplementedException();
        }

        public virtual ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            throw new NotImplementedException();
        }

        public virtual T GetOrAddPayload<T>(PayloadFactory<T> payloadFactory)
            where T : class
        {
            throw new NotImplementedException();
        }

        public virtual Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            throw new NotImplementedException();
        }

        public virtual bool HasMessageType(Type messageType)
        {
            throw new NotImplementedException();
        }

        public virtual bool HasPayloadType(Type payloadType)
        {
            throw new NotImplementedException();
        }

        public virtual Task NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
            where T : class
        {
            throw new NotImplementedException();
        }

        public virtual Task NotifyConsumed(TimeSpan duration, string consumerType)
        {
            throw new NotImplementedException();
        }

        public virtual Task NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
            where T : class
        {
            throw new NotImplementedException();
        }

        public virtual Task NotifyFaulted(TimeSpan duration, string consumerType, Exception exception)
        {
            throw new NotImplementedException();
        }

        public virtual Task Publish<T>(T message, CancellationToken cancellationToken = new CancellationToken())
            where T : class
        {
            throw new NotImplementedException();
        }

        public virtual Task Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = new CancellationToken())
            where T : class
        {
            throw new NotImplementedException();
        }

        public virtual Task Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = new CancellationToken())
            where T : class
        {
            throw new NotImplementedException();
        }

        public virtual Task Publish(object message, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public virtual Task Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public virtual Task Publish(object message, Type messageType, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public virtual Task Publish(
            object message,
            Type messageType,
            IPipe<PublishContext> publishPipe,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public virtual Task Publish<T>(object values, CancellationToken cancellationToken = new CancellationToken())
            where T : class
        {
            throw new NotImplementedException();
        }

        public virtual Task Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = new CancellationToken())
            where T : class
        {
            throw new NotImplementedException();
        }

        public virtual Task Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = new CancellationToken())
            where T : class
        {
            throw new NotImplementedException();
        }

        public virtual void Respond<T>(T message)
            where T : class
        {
            throw new NotImplementedException();
        }

        public virtual Task RespondAsync<T>(T message)
            where T : class
        {
            throw new NotImplementedException();
        }

        public virtual Task RespondAsync<T>(T message, IPipe<SendContext<T>> sendPipe)
            where T : class
        {
            throw new NotImplementedException();
        }

        public virtual Task RespondAsync<T>(T message, IPipe<SendContext> sendPipe)
            where T : class
        {
            throw new NotImplementedException();
        }

        public virtual Task RespondAsync(object message)
        {
            throw new NotImplementedException();
        }

        public virtual Task RespondAsync(object message, Type messageType)
        {
            throw new NotImplementedException();
        }

        public virtual Task RespondAsync(object message, IPipe<SendContext> sendPipe)
        {
            throw new NotImplementedException();
        }

        public virtual Task RespondAsync(object message, Type messageType, IPipe<SendContext> sendPipe)
        {
            throw new NotImplementedException();
        }

        public virtual Task RespondAsync<T>(object values)
            where T : class
        {
            throw new NotImplementedException();
        }

        public virtual Task RespondAsync<T>(object values, IPipe<SendContext<T>> sendPipe)
            where T : class
        {
            throw new NotImplementedException();
        }

        public virtual Task RespondAsync<T>(object values, IPipe<SendContext> sendPipe)
            where T : class
        {
            throw new NotImplementedException();
        }

        public virtual bool TryGetMessage<T>(out ConsumeContext<T> consumeContext)
            where T : class
        {
            throw new NotImplementedException();
        }

        public virtual bool TryGetPayload<T>(out T payload)
                                                                                                                                                                                                                            where T : class
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods
    }
}