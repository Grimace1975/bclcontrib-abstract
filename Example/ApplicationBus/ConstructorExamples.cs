﻿using System;
using System.Abstract;
using Contoso.Abstract;
namespace Example.ApplicationBus
{
    public class MyMessage : IServiceMessage
    {
        public string Value { get; set; }
    }

    public class MyMessageHandler : IServiceMessageHandler<MyMessage>
    {
        public void Handle(MyMessage message) { /* DO WORK */ }
    }

    public class ConstructorExamples
    {
        public void Constructor()
        {
            // set service bus
            ServiceBusManager.SetProvider(() =>
                new ApplicationServiceBus()
                    .Add<MyMessageHandler>()
            );
        }
    }
}