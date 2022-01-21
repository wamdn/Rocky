# Rocky
## Install
  1. To get started we will need to install dotnet SDK 5.0 you can find it here <https://dotnet.microsoft.com/en-us/download/dotnet/5.0>.
  2. We will also need to install SQL server locally <https://www.microsoft.com/fr-be/sql-server/sql-server-downloads> or change the "ConnectionStrings"."DefaultConnection" in appsetting.json to a remote SQL server connection string.
  3. To use the entity framework commands we will need to add the tooling, to install it globally ``dotnet tool install --global dotnet-ef``, run the following commands to verify that EF Core CLI tools are correctly installed: ``dotnet ef``, more info here <https://docs.microsoft.com/en-us/ef/core/cli/dotnet>.
  4. Now that everything is installed, let's create the database and schema from the migration, first ``cd`` into the project root folder and run ``dotnet ef database update``.
  5. Finally to run the project, in the poject folder run the following command ``dotnet run`` or from anywhere ``dotnet run --project <project_path>``.
  6. If everything worked the application should look something like this. ![Rocky app screenshot](https://zupimages.net/up/22/03/2rwb.png)