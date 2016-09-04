﻿// ==========================================================================
//  InfrastructureDependencies.cs
//  PinkParrot Headless CMS
// ==========================================================================
//  Copyright (c) PinkParrot Group
//  All rights reserved.
// ==========================================================================

using System.Net;
using Autofac;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Microsoft.AspNetCore.Builder;
using MongoDB.Driver;
using PinkParrot.Infrastructure.CQRS.Autofac;
using PinkParrot.Infrastructure.CQRS.Commands;
using PinkParrot.Infrastructure.CQRS.EventStore;
using PinkParrot.Read.Services.Implementations;

namespace PinkParrot.Configurations
{
    public class InfrastructureDependencies : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var eventStore = 
                EventStoreConnection.Create(
                   ConnectionSettings.Create()
                       .UseConsoleLogger()
                       .UseDebugLogger()
                       .KeepReconnecting()
                       .KeepRetrying(),
                   new IPEndPoint(IPAddress.Loopback, 1113));

            var mongoDbClient = new MongoClient("mongodb://localhost");
            var mongoDatabase = mongoDbClient.GetDatabase("PinkParrot");

            eventStore.ConnectAsync().Wait();

            builder.RegisterInstance(new UserCredentials("admin", "changeit"))
                .AsSelf()
                .SingleInstance();

            builder.RegisterInstance(mongoDatabase)
                .As<IMongoDatabase>()
                .SingleInstance();

            builder.RegisterType<MongoStreamPositionsStorage>()
                .As<IStreamPositionStorage>()
                .SingleInstance();

            builder.RegisterType<AutofacDomainObjectFactory>()
                .As<IDomainObjectFactory>()
                .SingleInstance();

            builder.RegisterType<DefaultNameResolver>()
                .As<IStreamNameResolver>()
                .SingleInstance();

            builder.RegisterInstance(eventStore)
                .As<IEventStoreConnection>()
                .SingleInstance();

            builder.RegisterType<EventStoreDomainObjectRepository>()
                .As<IDomainObjectRepository>()
                .SingleInstance();

            builder.RegisterType<InMemoryCommandBus>()
                .As<ICommandBus>()
                .SingleInstance();

            builder.RegisterType<EventStoreBus>()
                .AsSelf()
                .SingleInstance();
        }
    }

    public static class InfrastructureDependencie
    {
        public static void UseAppEventBus(this IApplicationBuilder app)
        {
            app.ApplicationServices.GetService(typeof(EventStoreBus));
        }
    }
}
