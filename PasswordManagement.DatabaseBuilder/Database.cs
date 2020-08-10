﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace PasswordManagement.DatabaseBuilder
{
    public class Database
    {
        public void Build<T>(bool deleteExisting, SqlConnectionStringBuilder connectionStringBuilder, params Assembly[] assemblies) where T : class
        {
            BuilderContext<T> context;
            if (!connectionStringBuilder.IntegratedSecurity)
            {
                context = new BuilderContext<T>(assemblies,
                        connectionStringBuilder.DataSource,
                        connectionStringBuilder.InitialCatalog,
                        connectionStringBuilder.UserID,
                        connectionStringBuilder.Password);
            }
            else
            {
                context = new BuilderContext<T>(assemblies,
                    connectionStringBuilder.DataSource,
                    connectionStringBuilder.InitialCatalog);
            }

            if (deleteExisting)
            {
                context.Database.EnsureDeleted();
            }

            context.Database.EnsureCreated();
            context.SaveChanges();
        }
    }
}