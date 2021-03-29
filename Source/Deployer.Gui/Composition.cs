﻿using System;
using Deployer.Core;
using Deployer.Core.Compiler;
using Deployer.Core.DeploymentLibrary;
using Deployer.Core.Interaction;
using Deployer.Core.Requirements;
using Deployer.Core.Services;
using Deployer.Functions;
using Deployer.Gui.ViewModels.Sections;
using Deployer.Wpf;
using Grace.DependencyInjection;
using Iridio.Binding;
using Iridio.Parsing;
using Zafiro.Core;
using Zafiro.Core.Files;
using Zafiro.Core.FileSystem;
using Zafiro.UI;
using Zafiro.UI.Wpf;

namespace Deployer.Gui
{
    public class Composition
    {
        private readonly DependencyInjectionContainer container;

        public Composition()
        {
            container = CreateContainer();
        }

        public MainViewModel Root => container.Locate<MainViewModel>();

        public static DependencyInjectionContainer CreateContainer()
        {
            var container = new DependencyInjectionContainer();
            container.Configure(c =>
            {
                c.Export<FileSystemOperations>().As<IFileSystemOperations>().Lifestyle.Singleton();
                c.Export<Preprocessor>().As<IPreprocessor>().Lifestyle.Singleton();
                c.Export<Parser>().As<IParser>().Lifestyle.Singleton();
                c.Export<Binder>().As<IBinder>().Lifestyle.Singleton();
                c.Export<DeployerCompiler>().As<IDeployerCompiler>().Lifestyle.Singleton();
                c.Export<Popup>().As<IPopup>().Lifestyle.Singleton();
                c.Export<MarkdownService>().As<IMarkdownService>().Lifestyle.Singleton();
                c.Export<OpenFilePicker>().As<IOpenFilePicker>().Lifestyle.Singleton();
                c.Export<Downloader>().As<IDownloader>().Lifestyle.Singleton();
                c.ExportFactory<string, IFileSystemOperations, IDownloader, IZafiroFile>((path, fo, dl) => new DesktopZafiroFile(new Uri(path), fo, dl));
                c.Export<IridioRequirementsAnalyzer>().As<IRequirementsAnalyzer>().Lifestyle.Singleton();
                c.ConfigureMediator();
                c.ExportFactory(() => new WoaDeployerWpf(new [] {typeof(Anchor).Assembly})).As<IWoaDeployer>().Lifestyle.Singleton();
                c.ExportFactory((IWoaDeployer d) => d.OperationProgress).As<IOperationProgress>().Lifestyle
                    .Singleton();
                
                // Difference between IDE and GUI
                ExportSections(c);
                c.Export<DeviceDeployer>().As<IDeviceDeployer>().Lifestyle.Singleton();
                c.Export<Interaction>().As<IInteraction>().Lifestyle.Singleton();
                c.Export<PopupWindow>().As<IView>();
                c.Export<SettingsService>().As<ISettingsService>().Lifestyle.Singleton();
                c.ExportFactory((IDownloader downloader, IFileSystemOperations ops) => GetDeploymentLibrary(downloader, ops)).As<IDeploymentLibrary>().Lifestyle
                    .Singleton();
            });

            return container;
        }

        private static IDeploymentLibrary GetDeploymentLibrary(IDownloader downloader, IFileSystemOperations ops)
        {
            return new XmlDeploymentLibrary("Feed\\Deployments.xml", ops);
        }

        private static void ExportSections(IExportRegistrationBlock block)
        {
            block.ExportAssemblies(new[] { typeof(MainViewModel).Assembly })
                .Where(y =>
                {
                    var isSection = typeof(ISection).IsAssignableFrom(y);
                    return isSection;
                })
                .ByInterface<ISection>()
                .ByInterface<IBusy>()
                .ByType()
                .ExportAttributedTypes()
                .Lifestyle.Singleton();
        }
    }
}