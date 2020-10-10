﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using TNDStudios.Apps.GitScanner.Helpers;

namespace TNDStudios.Apps.GitScanner
{
    /// <summary>
    /// https://github.com/sonatype-nexus-community/audit.net/blob/master/src/NugetAuditor/NugetAuditor.Lib/OSSIndex/Vulnerability.cs
    /// https://ossindex.sonatype.org/component/pkg:nuget/Harmony.Infrastructure.netcoreapp3.1
    /// https://nvd.nist.gov/vuln/data-feeds
    /// owasp.org/index.php/OWASP_Dependency_Check – paj28 Oct 23 '15 at 13:57
    /// NuGetDefense 
    /// </summary>
    class Program
    {
        static IConfigurationRoot configuration;

        static void Main(string[] args)
        {
            // Build configuration
            configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", false)
                .AddEnvironmentVariables()
                .Build();

            // DI
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddSingleton<IConfiguration>(configuration)
                .AddTransient<IGitHelper, Lib2GitHelper>()
                .BuildServiceProvider();

            string userName = configuration["Git::UserName"];
            string password = configuration["Git::Password"];
            String[] repositoriesToScan = { "https://github.com/TNDStudios/TNDStudios.Azure.FunctionApp.git" };

            IGitHelper gitHelper = serviceProvider.GetRequiredService<IGitHelper>();
            gitHelper.Connect(userName, password);

            foreach(string repositoryUrl in repositoriesToScan)
            {
                string localPath = @"c:\temp";
                gitHelper.Clone(repositoryUrl, localPath, true);
                List<string> commits = gitHelper.History(localPath);
            }
        }
    }
}
