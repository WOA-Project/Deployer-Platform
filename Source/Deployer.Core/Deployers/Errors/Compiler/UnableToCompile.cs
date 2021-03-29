using System.Collections.Generic;
using System.Linq;

namespace Deployer.Core.Deployers.Errors.Compiler
{
    public class UnableToCompile : DeployerCompilerError
    {
        public Iridio.Common.Errors Errors { get; }

        public UnableToCompile(Iridio.Common.Errors errors)
        {
            Errors = errors;
        }

        public override string ToString()
        {
            return string.Join(", ", Errors);
        }

        public override IEnumerable<string> Items => Errors.Select(s => s.ToString());
    }
}