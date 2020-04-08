// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MaikeBing.EntityFrameworkCore.NoSqLiteDB.Infrastructure.Internal;
using MaikeBing.EntityFrameworkCore.NoSqLiteDB.Metadata.Conventions.Internal;
using MaikeBing.EntityFrameworkCore.NoSqLiteDB.Query.ExpressionVisitors.Internal;
using MaikeBing.EntityFrameworkCore.NoSqLiteDB.Query.Internal;
using MaikeBing.EntityFrameworkCore.NoSqLiteDB.Storage.Internal;
using MaikeBing.EntityFrameworkCore.NoSqLiteDB.ValueGeneration.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Remotion.Linq.Parsing.ExpressionVisitors.TreeEvaluation;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace MaikeBing.Extensions.DependencyInjection
{
    /// <summary>
    ///     In-memory specific extension methods for <see cref="IServiceCollection" />.
    /// </summary>
    public static class LiteDBServiceCollectionExtensions
    {
        /// <summary>
        ///     <para>
        ///         Adds the services required by the in-memory database provider for Entity Framework
        ///         to an <see cref="IServiceCollection" />. You use this method when using dependency injection
        ///         in your application, such as with ASP.NET. For more information on setting up dependency
        ///         injection, see http://go.microsoft.com/fwlink/?LinkId=526890.
        ///     </para>
        ///     <para>
        ///         You only need to use this functionality when you want Entity Framework to resolve the services it uses
        ///         from an external dependency injection container. If you are not using an external
        ///         dependency injection container, Entity Framework will take care of creating the services it requires.
        ///     </para>
        /// </summary>
        /// <example>
        ///     <code>
        ///         public void ConfigureServices(IServiceCollection services)
        ///         {
        ///             services
        ///                 .AddEntityFrameworkLiteDBDatabase()
        ///                 .AddDbContext&lt;MyContext&gt;((serviceProvider, options) =>
        ///                     options.UseLiteDB("MyDatabase")
        ///                            .UseInternalServiceProvider(serviceProvider));
        ///         }
        ///     </code>
        /// </example>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection" /> to add services to. </param>
        /// <returns>
        ///     The same service collection so that multiple calls can be chained.
        /// </returns>
        public static IServiceCollection AddEntityFrameworkLiteDBDatabase([NotNull] this IServiceCollection serviceCollection)
        {
            Check.NotNull(serviceCollection, nameof(serviceCollection));

            var builder = new EntityFrameworkServicesBuilder(serviceCollection)
                //.TryAdd<IDatabaseProvider, DatabaseProvider<LiteDBOptionsExtension>>()
                .TryAdd<IValueGeneratorSelector, LiteDBValueGeneratorSelector>()
                .TryAdd<IDatabase>(p => p.GetService<ILiteDBDatabase>())
                .TryAdd<IDbContextTransactionManager, LiteDBTransactionManager>()
                //.TryAdd<IDatabaseCreator, LiteDBDatabaseCreator>()
                //.TryAdd<IQueryContextFactory, LiteDBQueryContextFactory>()
                //.TryAdd<IEntityQueryModelVisitorFactory, LiteDBQueryModelVisitorFactory>()
                //.TryAdd<IEntityQueryableExpressionVisitorFactory, LiteDBEntityQueryableExpressionVisitorFactory>()
                .TryAdd<IEvaluatableExpressionFilter, EvaluatableExpressionFilter>()
                //.TryAdd<IConventionSetBuilder, LiteDBConventionSetBuilder>()
                .TryAdd<ISingletonOptions, ILiteDBSingletonOptions>(p => p.GetService<ILiteDBSingletonOptions>())
                .TryAdd<ITypeMappingSource, LiteDBTypeMappingSource>()
                .TryAddProviderSpecificServices(
                    b => b
                        .TryAddSingleton<ILiteDBSingletonOptions, LiteDBSingletonOptions>()
                        .TryAddSingleton<ILiteDBStoreCache, LiteDBStoreCache>()
                        .TryAddSingleton<ILiteDBTableFactory, LiteDBTableFactory>()
                        .TryAddScoped<ILiteDBDatabase, LiteDBDatabase>()
                        .TryAddScoped<ILiteDBMaterializerFactory, LiteDBMaterializerFactory>());

            builder.TryAddCoreServices();

            return serviceCollection;
        }
    }
}
