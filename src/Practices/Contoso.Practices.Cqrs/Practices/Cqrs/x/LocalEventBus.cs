//#region License
// *
//The MIT License

//Copyright (c) 2008 Sky Morey

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.
//*/
//#endregion
//namespace Contoso.Practices.Cqrs.Event
//{
//    public class LocalEventBus : IEventBus
//    {
//        private readonly IDomainEventHandlerFactory eventHandlerBuilder;
//        private IDictionary<Type, EventHandlerInvoker> eventHandlerInvokers;

//        public LocalEventBus(IEnumerable<Type> eventHandlerTypes, IDomainEventHandlerFactory eventHandlerBuilder)
//        {
//            this.eventHandlerBuilder = eventHandlerBuilder;
//            BuildEventInvokers(eventHandlerTypes);
//        }

//        public void PublishEvent(DomainEvent domainEvent)
//        {
//            if(!eventHandlerInvokers.ContainsKey(domainEvent.GetType())) return;

//            var eventHandlerInvoker = eventHandlerInvokers[domainEvent.GetType()];
//            eventHandlerInvoker.Publish(domainEvent);
//        }

//        public void PublishEvents(IEnumerable<DomainEvent> domainEvents)
//        {
//            foreach(var domainEvent in domainEvents)
//                PublishEvent(domainEvent);
//        }

//        private void BuildEventInvokers(IEnumerable<Type> eventHandlerTypes)
//        {
//            eventHandlerInvokers = new Dictionary<Type, EventHandlerInvoker>();
//            foreach(var eventHandlerType in eventHandlerTypes)
//            {
//                foreach(var domainEventType in GetDomainEventTypes(eventHandlerType))
//                {
//                    EventHandlerInvoker eventInvoker;
//                    if(!eventHandlerInvokers.TryGetValue(domainEventType, out eventInvoker))
//                        eventInvoker = new EventHandlerInvoker(eventHandlerBuilder, domainEventType);

//                    eventInvoker.AddEventHandlerType(eventHandlerType);
//                    eventHandlerInvokers[domainEventType] = eventInvoker;
//                }
//            }
//        }

//        private static IEnumerable<Type> GetDomainEventTypes(Type eventHandlerType)
//        {
//            return from interfaceType in eventHandlerType.GetInterfaces()
//                   where interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IHandleDomainEvents<>)
//                   select interfaceType.GetGenericArguments()[0];
//        }

//        private class EventHandlerInvoker
//        {
//            private readonly IDomainEventHandlerFactory eventHandlerFactory;
//            private readonly Type domainEventType;
//            private readonly List<Type> eventHandlerTypes;

//            public EventHandlerInvoker(IDomainEventHandlerFactory eventHandlerFactory, Type domainEventType)
//            {
//                this.eventHandlerFactory = eventHandlerFactory;
//                this.domainEventType = domainEventType;
//                eventHandlerTypes = new List<Type>();
//            }

//            public void AddEventHandlerType(Type eventHandlerType)
//            {
//                eventHandlerTypes.Add(eventHandlerType);
//            }

//            public void Publish(DomainEvent domainEvent)
//            {
//                var handleMethod = typeof(IHandleDomainEvents<>).MakeGenericType(domainEventType).GetMethod("Handle");
//                foreach(var eventHandlerType in eventHandlerTypes)
//                {
//                    var eventHandler = eventHandlerFactory.Create(eventHandlerType);
//                    handleMethod.Invoke(eventHandler, new object[] {domainEvent});
//                }
//            }
//        }
//    }
//}