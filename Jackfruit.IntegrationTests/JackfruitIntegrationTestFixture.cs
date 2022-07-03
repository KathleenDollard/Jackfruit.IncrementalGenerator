using Jackfruit.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jackfruit.IntegrationTests
{
    public abstract class JackfruitIntegrationTestFixture
    {
        public E2EConfiguration Configuration { get; }

        public JackfruitIntegrationTestFixture() 
            => Configuration = new ();

        public string TestSetName
        {
            get => Configuration.TestSetName;
            set => Configuration.TestSetName = value;
        }

        public string? RunProject(string arguments)
        {

            return IntegrationHelpers.RunGeneratedProject(arguments, Configuration.TestSetName, Configuration.TestBuildPath);
        }
    }
}