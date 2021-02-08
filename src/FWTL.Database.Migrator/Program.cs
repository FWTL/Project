using System;
using System.IO;
using DbUp;
using DbUp.Engine;
using DbUp.Helpers;
using DbUp.ScriptProviders;

internal class Program
{
    private static int Main(string[] args)
    {
        if (Environment.UserInteractive)
        {
            args = new string[2];

            string catalog = "FWTL.App";
            string user = "sa";
            string password = "!QAZ2wsx";
            args[0] = $"Server=tcp:localhost,1433;Initial Catalog={catalog};Persist Security Info=False;User ID={user};Password={password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;";
            string currentDictionary = Directory.GetCurrentDirectory();
            string basePath = currentDictionary.Substring(0, currentDictionary.IndexOf("FWTL.Database.Migrator", StringComparison.Ordinal));
            args[1] = Path.Combine(basePath, "FWTL.Database2");
        }

        if (args.Length != 2)
        {
            return ReturnError(
                "Invalid args. You have to specify connection string and scripts path");
        }

        var connectionString = args[0];
        var scriptsPath = args[1];

        Console.WriteLine("Start executing preDeployment scripts...");
        string preDeploymentScriptsPath = Path.Combine(scriptsPath, "PreDeployment");
        UpgradeEngine preDeploymentScriptsExecutor =
            DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsFromFileSystem(preDeploymentScriptsPath, new FileSystemScriptOptions
                {
                    IncludeSubDirectories = true
                })
                .LogToConsole()
                .JournalTo(new NullJournal())
                .Build();

        var preDeploymentUpgradeResult = preDeploymentScriptsExecutor.PerformUpgrade();

        if (!preDeploymentUpgradeResult.Successful)
        {
            return ReturnError(preDeploymentUpgradeResult.Error.ToString());
        }

        ShowSuccess();

        Console.WriteLine("Start executing migration scripts...");
        var migrationScriptsPath = Path.Combine(scriptsPath, "Migrations");
        var upgrader =
            DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsFromFileSystem(migrationScriptsPath, new FileSystemScriptOptions
                {
                    IncludeSubDirectories = true
                })
                .LogToConsole()
                .JournalToSqlTable("app", "MigrationsJournal")
                .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            return ReturnError(result.Error.ToString());
        }

        ShowSuccess();

        Console.WriteLine("Start executing postDeployment scripts...");
        string postDeploymentScriptsPath = Path.Combine(scriptsPath, "PostDeployment");
        var postDeploymentScriptsExecutor =
            DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsFromFileSystem(postDeploymentScriptsPath, new FileSystemScriptOptions
                {
                    IncludeSubDirectories = true
                })
                .LogToConsole()
                .JournalTo(new NullJournal())
                .Build();

        var postDeploymentUpgradeResult = postDeploymentScriptsExecutor.PerformUpgrade();

        if (!postDeploymentUpgradeResult.Successful)
        {
            return ReturnError(result.Error.ToString());
        }

        ShowSuccess();

        return 0;
    }

    private static void ShowSuccess()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Success!");
        Console.ResetColor();
    }

    private static int ReturnError(string error)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(error);
        Console.ResetColor();
        return -1;
    }
}