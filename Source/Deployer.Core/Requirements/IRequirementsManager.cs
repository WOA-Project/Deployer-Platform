﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Zafiro.Core.Patterns.Either;

namespace Deployer.Core.Requirements
{
    public interface IRequirementsManager
    {
        Task<Either<ErrorList, IEnumerable<FulfilledRequirement>>> Satisfy(string path);
    }
}