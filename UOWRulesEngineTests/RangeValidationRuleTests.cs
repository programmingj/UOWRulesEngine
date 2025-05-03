using UOWRulesEngineCore.Rules;
using Xunit;

namespace UOWRulesEngineTests
{
	/// <summary>
	/// Contains unit tests for the <see cref="RangeValidationRule{T}"/> class.
	/// </summary>
	/// <remarks>
	/// These tests validate the behavior of the range validation logic, ensuring
	/// that values are correctly identified as being within or outside the specified range.
	/// </remarks>
	public class RangeValidationRuleTests
	{
		/// <summary>
		/// Tests the <see cref="RangeValidationRule{T}"/> to ensure it validates whether a value is within the specified range.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <param name="min">The minimum value of the range.</param>
		/// <param name="max">The maximum value of the range.</param>
		/// <param name="expectedIsValid">The expected result indicating if the value is valid.</param>
		/// <param name="expectedRuleName">The expected rule name.</param>
		/// <param name="expectedMessage">The expected validation message.</param>
		[Theory]
		[InlineData(50, 10, 100, true, "RangeValidationRule_Success", "Value 50 is outside of the range 10 to 100.")]
		[InlineData(5, 10, 100, false, "RangeValidationRule_Failure", "Value 5 is outside of the range 10 to 100.")]
		[InlineData(150, 10, 100, false, "RangeValidationRule_Failure", "Value 150 is out of the range 10 to 100.")]
		[InlineData(10, 10, 100, true, "RangeValidationRule_Success", "Value 10 is within the range 10 to 100.")]
		[InlineData(100, 10, 100, true, "RangeValidationRule_Success", "Value 100 is within the range 10 to 100.")]
		public void Execute_ShouldValidateRangeCorrectly(
			int value, int min, int max, bool expectedIsValid, string expectedRuleName, string expectedMessage)
		{
			// Arrange
			var rule = new RangeValidationRule<int>(expectedRuleName, expectedMessage, value, min, max);

			// Act
			var result = rule.Execute();

			// Assert
			Assert.Equal(expectedIsValid, result.IsValid);
			Assert.Equal(expectedRuleName, result.Name);
			Assert.Equal(expectedMessage, result.Message);
		}

		/// <summary>
		/// Tests the asynchronous <see cref="RangeValidationRule{T}"/> to ensure it validates whether a value is within the specified range.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <param name="min">The minimum value of the range.</param>
		/// <param name="max">The maximum value of the range.</param>
		/// <param name="expectedIsValid">The expected result indicating if the value is valid.</param>
		/// <param name="expectedRuleName">The expected rule name.</param>
		/// <param name="expectedMessage">The expected validation message.</param>
		[Theory]
		[InlineData(50, 10, 100, true, "RangeValidationRule_Success", "Value 50 is outside of the range 10 to 100.")]
		[InlineData(5, 10, 100, false, "RangeValidationRule_Failure", "Value 5 is outside of the range 10 to 100.")]
		public async Task ExecuteAsync_ShouldValidateRangeCorrectlyAsync(
			int value, int min, int max, bool expectedIsValid, string expectedRuleName, string expectedMessage)
		{
			// Arrange
			var rule = new RangeValidationRule<int>(expectedRuleName, expectedMessage, value, min, max);

			// Act
			var result = await rule.ExecuteAsync();

			// Assert
			Assert.Equal(expectedIsValid, result.IsValid);
			Assert.Equal(expectedRuleName, result.Name);
			Assert.Equal(expectedMessage, result.Message);
		}
	}
}
