﻿// ==========================================================================
//  EnrichWithAggregateIdHandler.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Squidex.Infrastructure;
using Squidex.Infrastructure.CQRS.Commands;
using Squidex.Read.Schemas.Services;
using Squidex.Write;
using Squidex.Write.Schemas;

// ReSharper disable InvertIf

namespace Squidex.Pipeline.CommandHandlers
{
    public sealed class EnrichWithAggregateIdHandler : ICommandHandler
    {
        private readonly ISchemaProvider schemaProvider;
        private readonly IActionContextAccessor actionContextAccessor;

        public EnrichWithAggregateIdHandler(ISchemaProvider schemaProvider, IActionContextAccessor actionContextAccessor)
        {
            this.schemaProvider = schemaProvider;

            this.actionContextAccessor = actionContextAccessor;
        }

        public async Task<bool> HandleAsync(CommandContext context)
        {
            var aggregateCommand = context.Command as IAggregateCommand;

            if (aggregateCommand == null || aggregateCommand.AggregateId != Guid.Empty)
            {
                return false;
            }
            
            var appCommand = context.Command as IAppCommand;

            if (appCommand == null)
            {
                return false;
            }

            var routeValues = actionContextAccessor.ActionContext.RouteData.Values;

            if (routeValues.ContainsKey("name"))
            {
                var schemaName = routeValues["name"].ToString();

                var id = await schemaProvider.FindSchemaIdByNameAsync(appCommand.AppId, schemaName);

                if (!id.HasValue)
                {
                    throw new DomainObjectNotFoundException(schemaName, typeof(SchemaDomainObject));
                }

                aggregateCommand.AggregateId = id.Value;
            }

            return false;
        }
    }
}
