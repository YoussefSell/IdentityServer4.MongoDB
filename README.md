# IdentityServer4.MongoDB

IdentityServer4.MongoDB is a package that provides the Storage implementation on top of MongoDB database driver to add data persistence for IdentityServer.

## Quick setup

to get started install the package using the Nuget package manager `Install-Package YoussefSell.IdentityServer4.MongoDB`.

then add the following configuration to your IdentityServer4 registration:

```csharp
public void ConfigureServices()
{
    // ... other code

    const string connectionString = @"mongodb://localhost:27017";
    const string databaseName = "IdentityServer";

    services.AddIdentityServer()
        .AddTestUsers(TestUsers.Users)
        .AddDeveloperSigningCredential()
        // add the configuration store
        .AddConfigurationStore(databaseName, connectionString)
        // add the operational store
        .AddOperationalStore(databaseName, connectionString);

    // ... other code
}
```

both methods have an overload that takes an action to configure your database and collections.

you can also find a quick starter template ([here](https://github.com/YoussefSell/IdentityServer4.MongoDB/tree/master/samples/Quickstarts)) that you can leverage to spin a new project.

the project has been built using the same concepts found in the EF core implementation of IdentityServer storage. this why you will find the same configuration if you already used EF core.

a more detailed configuration can be found on this blog post ([Using MongoDB as a data store for IdentityServer4](https://youssefsellami.com/using-mongodb-with-identityserver4))
