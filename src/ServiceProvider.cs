﻿using Microsoft.Extensions.DependencyInjection;
using System;

namespace Unity.Microsoft.DependencyInjection
{
    public class ServiceProvider : IServiceProvider, IServiceScopeFactory, IServiceScope, IDisposable
    {
        protected IUnityContainer _container;


        internal ServiceProvider(IUnityContainer container)
        {
            _container = container;
            _container.RegisterInstance<IServiceScope>(this);
            _container.RegisterInstance<IServiceProvider>(this);
            _container.RegisterInstance<IServiceScopeFactory>(this);
        }

        #region IServiceProvider

        public object GetService(Type serviceType)
        {
            try
            {
                return _container.Resolve(serviceType);
            }
            catch  { /* Ignore */}

            return null;
        }

        #endregion


        #region IServiceScopeFactory

        public IServiceScope CreateScope()
        {
            return new ServiceProvider(_container.CreateChildContainer()
                                                 .AddNewExtension<MDIExtension>());
        }

        #endregion


        #region IServiceScope

        IServiceProvider IServiceScope.ServiceProvider => this;

        #endregion


        #region ConfigureServices

        public static IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return new ServiceProvider(new UnityContainer().AddNewExtension<MDIExtension>()
                                                           .AddServices(services));
        }

        #endregion


        #region Disposable

        ~ServiceProvider()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool mode)
        {
            IDisposable disposable = _container;
            _container = null;
            disposable?.Dispose();
        }

        #endregion
    }


    public static class ServiceProviderExtension
    {
        public static IServiceProvider ConfigureServices(this IUnityContainer container, IServiceCollection services)
        {
            return new ServiceProvider(container.CreateChildContainer()
                                                .AddNewExtension<MDIExtension>()
                                                .AddServices(services));
        }
    }
}