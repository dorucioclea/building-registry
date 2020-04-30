namespace BuildingRegistry.Projector.Infrastructure.Modules
{
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Be.Vlaanderen.Basisregisters.DataDog.Tracing.Autofac;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.EventHandling.Autofac;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Autofac;
    using Be.Vlaanderen.Basisregisters.Projector;
    using Be.Vlaanderen.Basisregisters.Projector.Modules;
    using Be.Vlaanderen.Basisregisters.Shaperon;
    using BuildingRegistry.Infrastructure;
    using BuildingRegistry.Projections.Extract;
    using BuildingRegistry.Projections.Extract.BuildingExtract;
    using BuildingRegistry.Projections.Extract.BuildingUnitExtract;
    using BuildingRegistry.Projections.LastChangedList;
    using BuildingRegistry.Projections.Legacy;
    using BuildingRegistry.Projections.Legacy.BuildingDetail;
    using BuildingRegistry.Projections.Legacy.BuildingPersistentIdCrabIdMapping;
    using BuildingRegistry.Projections.Legacy.BuildingSyndication;
    using BuildingRegistry.Projections.Legacy.BuildingUnitDetail;
    using BuildingRegistry.Projections.Legacy.PersistentLocalIdMigration;
    using BuildingRegistry.Projections.Wms;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class ApiModule : Module
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceCollection _services;
        private readonly ILoggerFactory _loggerFactory;

        public ApiModule(
            IConfiguration configuration,
            IServiceCollection services,
            ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _services = services;
            _loggerFactory = loggerFactory;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new DataDogModule(_configuration));

            RegisterProjectionSetup(builder);

            builder.Populate(_services);
        }

        private void RegisterProjectionSetup(ContainerBuilder builder)
        {
            builder
                .RegisterModule(
                    new EventHandlingModule(
                        typeof(DomainAssemblyMarker).Assembly,
                        EventsJsonSerializerSettingsProvider.CreateSerializerSettings()))

                .RegisterModule<EnvelopeModule>()

                .RegisterEventstreamModule(_configuration)

                .RegisterModule<ProjectorModule>();

            RegisterExtractProjections(builder);
            RegisterLastChangedProjections(builder);
            RegisterLegacyProjections(builder);
            RegisterWmsProjections(builder);
        }

        private void RegisterExtractProjections(ContainerBuilder builder)
        {
            builder
                .RegisterModule(
                    new ExtractModule(
                        _configuration,
                        _services,
                        _loggerFactory));

            builder
                .RegisterProjectionMigrator<ExtractContextMigrationFactory>(
                    _configuration,
                    _loggerFactory)

                .RegisterProjections<BuildingExtractProjections, ExtractContext>(
                    context =>
                        new BuildingExtractProjections(
                            context.Resolve<IOptions<ExtractConfig>>(),
                            DbaseCodePage.Western_European_ANSI.ToEncoding(),
                            WKBReaderFactory.Create()))

                .RegisterProjections<BuildingUnitExtractProjections, ExtractContext>(
                    context =>
                        new BuildingUnitExtractProjections(
                            context.Resolve<IOptions<ExtractConfig>>(),
                            DbaseCodePage.Western_European_ANSI.ToEncoding(),
                            WKBReaderFactory.Create()));
        }

        private void RegisterLastChangedProjections(ContainerBuilder builder)
        {
            builder
                .RegisterModule(
                    new LastChangedListModule(
                        _configuration.GetConnectionString("LastChangedList"),
                        _configuration["DataDog:ServiceName"],
                        _services,
                        _loggerFactory));

            builder
                .RegisterProjectionMigrator<BuildingRegistry.Projections.LastChangedList.LastChangedListContextMigrationFactory>(
                    _configuration,
                    _loggerFactory)

                //.RegisterProjections<BuildingProjections, LastChangedListContext>()
                .RegisterProjections<BuildingUnitProjections, LastChangedListContext>();
        }

        private void RegisterLegacyProjections(ContainerBuilder builder)
        {
            builder
                .RegisterModule(
                    new LegacyModule(
                        _configuration,
                        _services,
                        _loggerFactory));
            builder
                .RegisterProjectionMigrator<LegacyContextMigrationFactory>(
                    _configuration,
                    _loggerFactory)

                .RegisterProjections<BuildingDetailProjections, LegacyContext>(() => new BuildingDetailProjections())
                .RegisterProjections<BuildingSyndicationProjections, LegacyContext>(() => new BuildingSyndicationProjections())
                .RegisterProjections<BuildingUnitDetailProjections, LegacyContext>(() => new BuildingUnitDetailProjections())
                .RegisterProjections<RemovedPersistentLocalIdProjections, LegacyContext>(() => new RemovedPersistentLocalIdProjections())
                .RegisterProjections<DuplicatedPersistentLocalIdProjections, LegacyContext>(() => new DuplicatedPersistentLocalIdProjections())
                .RegisterProjections<BuildingPersistenLocalIdCrabIdProjections, LegacyContext>(() => new BuildingPersistenLocalIdCrabIdProjections());
        }

        private void RegisterWmsProjections(ContainerBuilder builder)
        {
            builder
                .RegisterModule(
                    new WmsModule(
                        _configuration,
                        _services,
                        _loggerFactory));
            builder
                .RegisterProjectionMigrator<WmsContextMigrationFactory>(
                    _configuration,
                    _loggerFactory)

                .RegisterProjections<BuildingRegistry.Projections.Wms.Building.BuildingProjections, WmsContext>(() =>
                    new BuildingRegistry.Projections.Wms.Building.BuildingProjections(WKBReaderFactory.Create()))

                .RegisterProjections<BuildingRegistry.Projections.Wms.BuildingUnit.BuildingUnitProjections, WmsContext>(() =>
                    new BuildingRegistry.Projections.Wms.BuildingUnit.BuildingUnitProjections(WKBReaderFactory.Create()));
        }
    }
}
