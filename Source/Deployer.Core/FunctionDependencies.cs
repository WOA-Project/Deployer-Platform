using System;
using System.Net.Http;
using Deployer.Core.DeploymentLibrary;
using Deployer.Core.Scripting;
using Deployer.Core.Services;
using Deployer.Tools.AzureDevOps;
using Deployer.Tools.Bcd;
using Deployer.Tools.Dism;
using Deployer.Tools.ImageFlashing;
using Deployer.Tools.Wim;
using Grace.DependencyInjection;
using Octokit;
using Octokit.Internal;
using Zafiro.Core;
using Zafiro.Core.FileSystem;

namespace Deployer.Core
{
    public class FunctionDependencies
    {
        public const string EncryptedGitHubToken = "vkGU7tqfkG0qlOK/vKVhgfhDTNwiQwAutIjfL8KS9xN9B1rPmQjpcZSNs2Y6Z3RHqmsbltzcVLjPCLGAjOF073we96nyLFHcKUh/OhLoLAF7QXumPFvwrk1evMtaexhz";
        
        public static void Configure(IExportRegistrationBlock block)
        {
            var token = Encryption.Decrypt(EncryptedGitHubToken);
            var gitHubClient = new GitHubClient(new ProductHeaderValue("WOADeployer"), new InMemoryCredentialStore(new Credentials(token)));
            
            block.Export<ZipExtractor>().As<IZipExtractor>();
            block.Export<FileSystemOperations>().As<IFileSystemOperations>().Lifestyle.Singleton();
            block.Export<Downloader>().As<IDownloader>().Lifestyle.Singleton();
            block.ExportFactory(() => new HttpClient { Timeout = TimeSpan.FromMinutes(30) }).Lifestyle.Singleton();
            block.ExportFactory(() => gitHubClient).As<IGitHubClient>().Lifestyle.Singleton();
            block.ExportFactory(() => AzureDevOpsBuildClient.Create(new Uri("https://dev.azure.com")))
                .As<IAzureDevOpsBuildClient>().Lifestyle.Singleton();
            block.Export<BootCreator>().As<IBootCreator>().Lifestyle.Singleton();
            block.ExportFactory((string store) => new BcdInvoker(store)).As<IBcdInvoker>();
            block.Export<ImageFlasher>().As<IImageFlasher>().Lifestyle.Singleton();
            block.Export<DismImageService>().As<IWindowsImageService>().Lifestyle.Singleton();
            block.Export<WindowsImageMetadataReader>().As<IWindowsImageMetadataReader>().Lifestyle.Singleton();
        }
    }
}