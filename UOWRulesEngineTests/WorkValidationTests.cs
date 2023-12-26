using Xunit;
using UOWRulesEngineCore;
using UOWRulesEngineTests.Setup;

namespace UOWRulesEngineTests
{
	/// <summary>
	/// Collection of tests for the <see cref="WorkValidation"/> objects.
	/// </summary>
	public class WorkValidationTests : UOWRulesEngineTestsBase
	{
		/// <summary>
		/// Default constructor. Instantiates a new <see cref="WorkValidationTests"/> object.
		/// </summary>
		public WorkValidationTests()
		{
		}

		/// <summary>
		/// Tests to ensure the code honors the short circuit pattern implementation for the StopRuleProcessingOnFirstFailure property.
		/// </summary>
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
			validation.Rules = UOWTestDataSetup.GetRulesList(true);

			// Execute the ValidateRules we're testing to make sure that the conditions being tested check out.
			validation.ValidateRules();

			Assert.Equal(1, validation.FailedResults.Count);
		}

		/// <summary>
		/// Tests to make sure the code executes all rules even if one fails.
		/// </summary>
		[Fact]
		public void WorkValidation_ValidateRules_RunsAllRulesOnError()
		{
			WorkActionConfiguration config = new WorkActionConfiguration();
			WorkValidation validation = new WorkValidation();

			// Make sure the WorkActionConfiguration is set up to process all rules.
			config.StopRuleProcessingOnFirstFailure = false;

			// Set up the WorkValidation object so that we can test the method.
			validation.SetConfiguration(config);

			// Get the list of rules with failures in it to test with.
			validation.Rules = UOWTestDataSetup.GetRulesList(true);

			// Execute the ValidateRules we're testing to make sure that the conditions being tested check out.
			validation.ValidateRules();

			Assert.Equal(2, validation.FailedResults.Count);
		}
	}
}
