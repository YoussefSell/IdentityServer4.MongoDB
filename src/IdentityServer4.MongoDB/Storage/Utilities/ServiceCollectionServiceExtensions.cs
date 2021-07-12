namespace Microsoft.Extensions.DependencyInjection
{
    using IdentityServer4.MongoDB.Options;
    using System;

    /// <summary>
    /// Extension methods for adding services to an Microsoft.Extensions.DependencyInjection.IServiceCollection.
    /// </summary>
    public static class ScopeServiceCollectionServiceExtensions
    {
        /// <summary>
        /// Adds a scoped service of the type specified in serviceType to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        /// </summary>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service to.</param>
        /// <param name="scope">the scope to register the service with</param>
        /// <param name="serviceType">The type of the service to register and the implementation to use.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection Add(this IServiceCollection services, RegistrationScope scope, Type serviceType)
            => scope switch
            {
                RegistrationScope.Transient => services.AddTransient(serviceType),
                RegistrationScope.Scoped => services.AddScoped(serviceType),
                RegistrationScope.Singleton => services.AddSingleton(serviceType),
                _ => services,
            };

        /// <summary>
        /// Adds a scoped service of the type specified in serviceType with a factory specified
        /// in implementationFactory to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        /// </summary>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service to.</param>
        /// <param name="scope">the scope to register with.</param>
        /// <param name="serviceType">The type of the service to register.</param>
        /// <param name="implementationFactory">The factory that creates the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection Add(this IServiceCollection services, RegistrationScope scope, Type serviceType, Func<IServiceProvider, object> implementationFactory)
            => scope switch
            {
                RegistrationScope.Transient => services.AddTransient(serviceType, implementationFactory),
                RegistrationScope.Scoped => services.AddScoped(serviceType, implementationFactory),
                RegistrationScope.Singleton => services.AddSingleton(serviceType, implementationFactory),
                _ => services,
            };

        /// <summary>
        /// Adds a scoped service of the type specified in serviceType with an implementation
        /// of the type specified in implementationType to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        /// </summary>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service to.</param>
        /// <param name="scope">the scope to register with.</param>
        /// <param name="serviceType">The type of the service to register.</param>
        /// <param name="implementationType">The implementation type of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection Add(this IServiceCollection services, RegistrationScope scope, Type serviceType, Type implementationType)
            => scope switch
            {
                RegistrationScope.Transient => services.AddTransient(serviceType, implementationType),
                RegistrationScope.Scoped => services.AddScoped(serviceType, implementationType),
                RegistrationScope.Singleton => services.AddSingleton(serviceType, implementationType),
                _ => services,
            };

        /// <summary>
        /// Adds a scoped service of the type specified in TService to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service to.</param>
        /// <param name="scope">the scope to register with.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection Add<TService>(this IServiceCollection services, RegistrationScope scope) where TService : class
            => scope switch
            {
                RegistrationScope.Transient => services.AddTransient<TService>(),
                RegistrationScope.Scoped => services.AddScoped<TService>(),
                RegistrationScope.Singleton => services.AddSingleton<TService>(),
                _ => services,
            };

        /// <summary>
        /// Adds a scoped service of the type specified in TService with a factory specified
        /// in implementationFactory to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service to.</param>
        /// <param name="scope">the scope to register with.</param>
        /// <param name="implementationFactory">The factory that creates the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection Add<TService>(this IServiceCollection services, RegistrationScope scope, Func<IServiceProvider, TService> implementationFactory)
            where TService : class => scope switch
            {
                RegistrationScope.Transient => services.AddTransient(implementationFactory),
                RegistrationScope.Scoped => services.AddScoped(implementationFactory),
                RegistrationScope.Singleton => services.AddSingleton(implementationFactory),
                _ => services,
            };

        /// <summary>
        /// Adds a scoped service of the type specified in TService with an implementation
        /// type specified in TImplementation to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service to.</param>
        /// <param name="scope">the scope to register with.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection Add<TService, TImplementation>(this IServiceCollection services, RegistrationScope scope)
            where TService : class
            where TImplementation : class, TService => scope switch
            {
                RegistrationScope.Transient => services.AddTransient<TService, TImplementation>(),
                RegistrationScope.Scoped => services.AddScoped<TService, TImplementation>(),
                RegistrationScope.Singleton => services.AddSingleton<TService, TImplementation>(),
                _ => services,
            };

        /// <summary>
        /// Adds a scoped service of the type specified in TService with an implementation
        /// type specified in TImplementation using the factory specified in implementationFactory
        /// to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service to.</param>
        /// <param name="scope">the scope to register with</param>
        /// <param name="implementationFactory">The factory that creates the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection Add<TService, TImplementation>(this IServiceCollection services, RegistrationScope scope, Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService => scope switch
            {
                RegistrationScope.Transient => services.AddTransient(implementationFactory),
                RegistrationScope.Scoped => services.AddScoped(implementationFactory),
                RegistrationScope.Singleton => services.AddSingleton(implementationFactory),
                _ => services,
            };

    }
}
