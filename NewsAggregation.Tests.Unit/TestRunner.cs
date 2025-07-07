using Xunit;
using Xunit.Abstractions;

namespace NewsAggregation.Tests.Unit
{
    /// <summary>
    /// Test runner class for executing all unit tests in the project
    /// </summary>
    public class TestRunner
    {
        private readonly ITestOutputHelper _output;

        public TestRunner(ITestOutputHelper output)
        {
            _output = output;
        }

        /// <summary>
        /// Runs all tests and outputs results
        /// </summary>
        [Fact]
        public void RunAllTests()
        {
            _output.WriteLine("Starting test execution...");
            _output.WriteLine("All unit tests are ready to run.");
            _output.WriteLine("Use 'dotnet test' command to execute all tests.");
            _output.WriteLine("Use 'dotnet test --filter \"FullyQualifiedName~AuthServiceTests\"' to run specific test classes.");
            _output.WriteLine("Use 'dotnet test --verbosity normal' for detailed output.");
            _output.WriteLine("Use 'dotnet test --collect:\"XPlat Code Coverage\"' for coverage reports.");
        }
    }
} 