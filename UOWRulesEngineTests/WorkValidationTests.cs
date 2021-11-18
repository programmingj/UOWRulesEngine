using Moq;
using System.Collections.Generic;
using Xunit;
using UOWRulesEngine;
using UOWRulesEngine.Rules;

namespace UOWRulesEngineTests
{
	public class WorkValidationTests : UOWRulesEngineTestsBase
	{
		public WorkValidationTests()
		{
		}

		[Fact]
		public void WorkValidation_ValidateRules_StopsOnFirstError()
		{
			WorkActionConfiguration config = new WorkActionConfiguration();
			WorkValidation validation = new WorkValidation();

			// Make sure the WorkActionConfiguration is set up to stop upon the first error encountered.
			config.StopRuleProcessingOnFirstFailure = true;

			// Set up the WorkValidation object so that we can test the method.
			validation.SetConfiguration(config);

			// Get the list of rules with failures in it to test with.
			validation.Rules = GetRulesList(true);

			// Execute the ValidateRules we're testing to make sure that the conditions being tested check out.
			validation.ValidateRules();

			Assert.Equal(1, validation.FailedResults.Count);
		}

		[Fact]
		public void WorkValidation_ValidateRules_RunsAllRulesOnError()
		{
			WorkActionConfiguration config = new WorkActionConfiguration();
			WorkValidation validation = new WorkValidation();

			// Make sure the WorkActionConfiguration is set up to stop upon the first error encountered.
			config.StopRuleProcessingOnFirstFailure = false;

			// Set up the WorkValidation object so that we can test the method.
			validation.SetConfiguration(config);

			// Get the list of rules with failures in it to test with.
			validation.Rules = GetRulesList(true);

			// Execute the ValidateRules we're testing to make sure that the conditions being tested check out.
			validation.ValidateRules();

			Assert.Equal(2, validation.FailedResults.Count);
		}
	}
}
