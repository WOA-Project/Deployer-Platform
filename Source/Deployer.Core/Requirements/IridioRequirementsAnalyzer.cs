﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Optional.Unsafe;
using Zafiro.Core.Patterns.Either;

namespace Deployer.Core.Requirements
{
    public class IridioRequirementsAnalyzer : IRequirementsAnalyzer
    {
        public Either<ErrorList, IEnumerable<MissingRequirement>> GetRequirements(string content)
        {
            try
            {
                var pattern = @"(?i)\s*//\s*Requires\s+(\S*)\s+""(\S*)""\s+as\s+""([^""]*)""";
                var matches = Regex.Matches(content, pattern);
                var missingRequirements = matches.Cast<Match>()
                    .Select(m => new MissingRequirement(m.Groups[2].Value, RequirementDefinition.Parse(m.Groups[1].Value).ValueOrFailure(), m.Groups[3].Value));
                return Either.Success<ErrorList, IEnumerable<MissingRequirement>>(missingRequirements);
            }
            catch (Exception e)
            {
                return Either.Error<ErrorList, IEnumerable<MissingRequirement>>(new ErrorList($"Error parsing requirements: {e}"));
            }
        }
    }
}