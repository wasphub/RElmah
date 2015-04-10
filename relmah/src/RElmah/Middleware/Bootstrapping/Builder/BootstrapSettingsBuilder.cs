using System;

namespace RElmah.Middleware.Bootstrapping.Builder
{
    public class BootstrapSettingsBuilder
    {
        readonly BootstrapSettings _settings;

        internal BootstrapSettingsBuilder(BootstrapSettings settings)
        {
            _settings = settings;
        }

        public BootstrapSettingsBuilder()
        {
            _settings = new BootstrapSettings();
        }

        internal BootstrapSettings Build()
        {
            return _settings;
        }

        public BootstrapSettingsBuilder WithOptions(BootstrapOptions options)
        {
            if (options.IdentityProviderBuilderSetter != null)
            {
                var identityProviderBuilderSetter = options.IdentityProviderBuilderSetter();
                if (identityProviderBuilderSetter == null)
                    throw new InvalidOperationException("Identity provider must be not null");

                _settings.IdentityProviderBuilder = identityProviderBuilderSetter;

            }

            if (options.DomainStoreBuilder != null)
            {
                _settings.DomainStoreBuilder = options.DomainStoreBuilder;
            }

            if (options.RegistryConfigurator != null)
            {
                _settings.RegistryConfigurator = options.RegistryConfigurator;
            }

            return new BootstrapSettingsBuilder(_settings);
        }

        public FrontendSettingsBuilder RunFrontend(FrontendOptions options = null)
        {
            _settings.Side = Side.Frontend;

            if (options != null && options.TargetBackendEndpointSetter != null)
            {
                var targetBackendEndpointSetter = options.TargetBackendEndpointSetter();
                if (targetBackendEndpointSetter == null)
                    throw new InvalidOperationException("Target backend endpoint must be a valid Uri");

                _settings.TargetBackendEndpoint = targetBackendEndpointSetter.ToString();
            }

            return new FrontendSettingsBuilder(_settings);
        }

        public BackendSettingsBuilder RunBackend()
        {
            _settings.Side = Side.Backend;

            return new BackendSettingsBuilder(_settings);
        }

        #region Inner builders

        public class FrontendSettingsBuilder
        {
            readonly BootstrapSettings _settings;

            internal FrontendSettingsBuilder(BootstrapSettings settings)
            {
                _settings = settings;
            }

            public FrontendForErrorsSettingsBuilder ForErrors(ErrorsOptions options = null)
            {
                return ForErrors(true, options);
            }

            public FrontendForErrorsSettingsBuilder ForErrors(bool active, ErrorsOptions options = null)
            {
                _settings.ForErrors = active;

                _settings.ErrorsPrefix  = options != null && options.PrefixSetter != null
                                        ? options.PrefixSetter()
                                        : "relmah-errors";

                _settings.UseRandomizer = options != null && options.UseRandomizerSetter != null
                                        ? options.UseRandomizerSetter()
                                        : _settings.UseRandomizer;

                return new FrontendForErrorsSettingsBuilder(_settings);
            }

            public FrontendForDomainSettingsBuilder ForDomain(DomainOptions options = null)
            {
                return ForDomain(true, options);
            }

            public FrontendForDomainSettingsBuilder ForDomain(bool active, DomainOptions options = null)
            {
                _settings.ForDomain = active;

                _settings.DomainPrefix = options != null && options.PrefixSetter != null
                                       ? options.PrefixSetter()
                                       : "relmah-domain";

                if (options != null && options.DomainConfigurator != null)
                    _settings.DomainConfigurator = options.DomainConfigurator;

                return new FrontendForDomainSettingsBuilder(_settings);
            }

            public BootstrapSettings Build()
            {
                return _settings;
            }
        }

        public class FrontendForErrorsSettingsBuilder
        {
            readonly BootstrapSettings _settings;

            internal FrontendForErrorsSettingsBuilder(BootstrapSettings settings)
            {
                _settings = settings;
            }

            public FrontendForDomainFinalSettingsBuilder ForDomain(DomainOptions options = null)
            {
                return ForDomain(true, options);
            }

            public FrontendForDomainFinalSettingsBuilder ForDomain(bool active, DomainOptions options = null)
            {
                _settings.ForDomain = active;

                _settings.DomainPrefix = options != null && options.PrefixSetter != null
                                       ? options.PrefixSetter()
                                       : "relmah-domain";

                if (options != null && options.DomainConfigurator != null)
                    _settings.DomainConfigurator = options.DomainConfigurator;

                return new FrontendForDomainFinalSettingsBuilder(_settings);
            }

            public BootstrapSettings Build()
            {
                return _settings;
            }
        }

        public class FrontendForErrorsFinalSettingsBuilder
        {
            readonly BootstrapSettings _settings;

            internal FrontendForErrorsFinalSettingsBuilder(BootstrapSettings settings)
            {
                _settings = settings;
            }

            public BootstrapSettings Build()
            {
                return _settings;
            }
        }

        public class FrontendForDomainSettingsBuilder
        {
            readonly BootstrapSettings _settings;

            internal FrontendForDomainSettingsBuilder(BootstrapSettings settings)
            {
                _settings = settings;
            }

            public FrontendForErrorsFinalSettingsBuilder ForErrors(ErrorsOptions options = null)
            {
                return ForErrors(true, options);
            }

            public FrontendForErrorsFinalSettingsBuilder ForErrors(bool active, ErrorsOptions options = null)
            {
                _settings.ForErrors = true;

                _settings.ErrorsPrefix  = options != null && options.PrefixSetter != null
                                        ? options.PrefixSetter()
                                        : "relmah-errors";

                _settings.UseRandomizer = options != null && options.UseRandomizerSetter != null
                                        ? options.UseRandomizerSetter()
                                        : _settings.UseRandomizer;

                return new FrontendForErrorsFinalSettingsBuilder(_settings);
            }

            public BootstrapSettings Build()
            {
                return _settings;
            }
        }

        public class FrontendForDomainFinalSettingsBuilder
        {
            readonly BootstrapSettings _settings;

            internal FrontendForDomainFinalSettingsBuilder(BootstrapSettings settings)
            {
                _settings = settings;
            }

            public BootstrapSettings Build()
            {
                return _settings;
            }
        }

        public class BackendSettingsBuilder
        {
            readonly BootstrapSettings _settings;

            internal BackendSettingsBuilder(BootstrapSettings settings)
            {
                _settings = settings;
            }

            public BootstrapSettings Build()
            {
                return _settings;
            }
        }

        #endregion
    }
}