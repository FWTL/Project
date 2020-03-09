using System;
using System.Collections.Generic;
using System.Text;
using FWTL.Common.Credentials;

namespace FWTL.Auth.Database
{
    public class AuthIdentityServerDatabaseCredentials
    {
        public AuthIdentityServerDatabaseCredentials(LocalDatabaseCredentials credentials)
        {
            ConnectionString = credentials.ConnectionString;
        }

        public AuthIdentityServerDatabaseCredentials(SqlServerDatabaseCredentials credentials)
        {
            ConnectionString = credentials.ConnectionString;
        }

        public string ConnectionString { get; private set; }
    }
}
