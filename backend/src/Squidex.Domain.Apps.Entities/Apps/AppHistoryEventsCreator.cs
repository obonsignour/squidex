﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.Domain.Apps.Entities.History;
using Squidex.Domain.Apps.Events.Apps;
using Squidex.Infrastructure;
using Squidex.Infrastructure.EventSourcing;
using Squidex.Infrastructure.Reflection;

namespace Squidex.Domain.Apps.Entities.Apps
{
    public sealed class AppHistoryEventsCreator : HistoryEventsCreatorBase
    {
        public AppHistoryEventsCreator(TypeNameRegistry typeNameRegistry)
            : base(typeNameRegistry)
        {
            AddEventMessage<AppContributorAssigned>(
                "history.apps.contributoreAssigned");

            AddEventMessage<AppContributorRemoved>(
                "history.apps.contributoreRemoved");

            AddEventMessage<AppClientAttached>(
                "history.apps.clientAdded");

            AddEventMessage<AppClientRevoked>(
                "history.apps.clientRevoked");

            AddEventMessage<AppClientUpdated>(
                "history.apps.clientUpdated");

            AddEventMessage<AppPlanChanged>(
                "history.apps.planChanged");

            AddEventMessage<AppPlanReset>(
                "history.apps.planReset");

            AddEventMessage<AppLanguageAdded>(
                "history.apps.languagedAdded");

            AddEventMessage<AppLanguageRemoved>(
                "history.apps.languagedRemoved");

            AddEventMessage<AppLanguageUpdated>(
                "history.apps.languagedUpdated");

            AddEventMessage<AppMasterLanguageSet>(
                "history.apps.languagedSetToMaster");

            AddEventMessage<AppSettingsUpdated>(
                "history.apps.settingsUpdated");

            AddEventMessage<AppRoleAdded>(
                "history.apps.roleAdded");

            AddEventMessage<AppRoleDeleted>(
                "history.apps.roleDeleted");

            AddEventMessage<AppRoleUpdated>(
                "history.apps.roleUpdated");

            AddEventMessage<AppAssetsScriptsConfigured>(
                "history.apps.assetScriptsConfigured");

            AddEventMessage<AppUpdated>(
                "history.apps.updated");

            AddEventMessage<AppTransfered>(
                "history.apps.transfered");

            AddEventMessage<AppImageUploaded>(
                "history.apps.imageUploaded");

            AddEventMessage<AppImageRemoved>(
                "history.apps.imageRemoved");
        }

        private HistoryEvent? CreateEvent(IEvent @event)
        {
            switch (@event)
            {
                case AppContributorAssigned e:
                    return CreateContributorsEvent(e, e.ContributorId, e.Role);
                case AppContributorRemoved e:
                    return CreateContributorsEvent(e, e.ContributorId);
                case AppClientAttached e:
                    return CreateClientsEvent(e, e.Id);
                case AppClientUpdated e:
                    return CreateClientsEvent(e, e.Id);
                case AppClientRevoked e:
                    return CreateClientsEvent(e, e.Id);
                case AppLanguageAdded e:
                    return CreateLanguagesEvent(e, e.Language);
                case AppLanguageUpdated e:
                    return CreateLanguagesEvent(e, e.Language);
                case AppMasterLanguageSet e:
                    return CreateLanguagesEvent(e, e.Language);
                case AppLanguageRemoved e:
                    return CreateLanguagesEvent(e, e.Language);
                case AppRoleAdded e:
                    return CreateRolesEvent(e, e.Name);
                case AppRoleUpdated e:
                    return CreateRolesEvent(e, e.Name);
                case AppRoleDeleted e:
                    return CreateRolesEvent(e, e.Name);
                case AppPlanChanged e:
                    return CreatePlansEvent(e, e.PlanId);
                case AppPlanReset e:
                    return CreatePlansEvent(e);
                case AppSettingsUpdated e:
                    return CreateAssetScriptsEvent(e);
                case AppAssetsScriptsConfigured e:
                    return CreateGeneralEvent(e);
                case AppUpdated e:
                    return CreateGeneralEvent(e);
                case AppTransfered e:
                    return CreateGeneralEvent(e);
                case AppImageUploaded e:
                    return CreateGeneralEvent(e);
                case AppImageRemoved e:
                    return CreateGeneralEvent(e);
            }

            return null;
        }

        private HistoryEvent CreateGeneralEvent(IEvent e)
        {
            return ForEvent(e, "general");
        }

        private HistoryEvent CreateAppSettingsEvent(IEvent e)
        {
            return ForEvent(e, "settings.appSettings");
        }

        private HistoryEvent CreateContributorsEvent(IEvent e, string contributor, string? role = null)
        {
            return ForEvent(e, "settings.contributors").Param("Contributor", contributor).Param("Role", role);
        }

        private HistoryEvent CreateLanguagesEvent(IEvent e, Language language)
        {
            return ForEvent(e, "settings.languages").Param("Language", language);
        }

        private HistoryEvent CreateRolesEvent(IEvent e, string name)
        {
            return ForEvent(e, "settings.roles").Param("Name", name);
        }

        private HistoryEvent CreateClientsEvent(IEvent e, string id)
        {
            return ForEvent(e, "settings.clients").Param("Id", id);
        }

        private HistoryEvent CreatePlansEvent(IEvent e, string? plan = null)
        {
            return ForEvent(e, "settings.plan").Param("Plan", plan);
        }

        private HistoryEvent CreateAssetScriptsEvent(IEvent e)
        {
            return ForEvent(e, "settings.assetScripts");
        }

        protected override Task<HistoryEvent?> CreateEventCoreAsync(Envelope<IEvent> @event)
        {
            return Task.FromResult(CreateEvent(@event.Payload));
        }
    }
}
