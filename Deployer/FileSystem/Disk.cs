﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.FileSystem.Gpt;

namespace Deployer.FileSystem
{
    public class Disk
    {
        public Disk(IDiskApi diskApi, DiskInfo diskProps)
        {
            DiskApi = diskApi;
            FriendlyName = diskProps.FriendlyName;
            Number = diskProps.Number;
            Size = diskProps.Size;
            AllocatedSize = diskProps.AllocatedSize;
            FriendlyName = diskProps.FriendlyName;
            IsSystem = diskProps.IsSystem;
            IsBoot = diskProps.IsBoot;
            IsReadOnly = diskProps.IsReadOnly;
            IsOffline = diskProps.IsOffline;
            UniqueId = diskProps.UniqueId;
        }

        public ByteSize Size { get; }

        public bool IsBoot { get; }

        public bool IsReadOnly { get; }

        public bool IsOffline { get; }

        public bool IsSystem { get; }

        public ByteSize AllocatedSize { get; }

        public string FriendlyName { get; }

        public IDiskApi DiskApi { get; }

        public uint Number { get; }
        public ByteSize AvailableSize => Size - AllocatedSize;
        public string UniqueId { get; }

        public async Task<List<Partition>> GetPartitions()
        {
            using (var context = await GptContextFactory.Create(Number, FileAccess.Read))
            {
                return context.Partitions.Select(x => x.AsCommon(this)).ToList();
            }
        }
        
        public Task Refresh()
        {
            return DiskApi.RefreshDisk(this);
        }

        public Task SetGuid(Guid guid)
        {
            return DiskApi.ChangeDiskId(this, guid);
        }

        public override string ToString()
        {
            return $"Disk {Number} ({FriendlyName})";
        }

        public async Task<Partition> AddPartition(ByteSize size)
        {
            throw new NotImplementedException();
        }
    }
}