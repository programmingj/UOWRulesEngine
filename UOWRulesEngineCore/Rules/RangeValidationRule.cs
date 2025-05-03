namespace UOWRulesEngineCore.Rules
{
	/// <summary>
	/// A business rule that validates whether a value is within a specified range.
	/// </summary>
	public class RangeValidationRule<T> : WorkRule
		where T : struct, IComparable
	{
		private readonly T _value;
		private readonly T _min;
		private readonly T _max;

		/// <summary>
		/// Initializes a new instance of the <see cref="RangeValidationRule{T}"/> class.
		/// </summary>
		/// <param name="ruleName">The name of the rule to display in the error dictionary.</param>
		/// <param name="message">The message that will be used if the rule fails.</param>
		/// <param name="value">The value to validate.</param>
		/// <param name="min">The minimum allowed value.</param>
		/// <param name="max">The maximum allowed value.</param>
		public RangeValidationRule(string ruleName, string message, T value, T min, T max)
			: base(ruleName, message)
		{
			_value = value;
			_min = min;
			_max = max;
		}

		/// <summary>
		/// Executes the rule to validate the value.
		/// </summary>
		/// <remarks>
		/// Following the Command Pattern, this method is called from the base class'
		/// <see cref="RuleComponent.Execute"/> method to execute the rule.
		/// </remarks>
		/// <returns>A <see cref="WorkResult"/> indicating whether the rule passed or failed.</returns>
		public override IWorkResult Verify()
		{
			if (_value.CompareTo(_min) >= 0 && _value.CompareTo(_max) <= 0)
			{
				return new WorkResult()
					.SetName("RangeValidationRule")
					.SetMessage($"Value {_value} is within the range {_min} to {_max}.")
					.SetIsValid(true);
			}
			else
			{
				return new WorkResult()
					.SetName("RangeValidationRule")
					.SetMessage($"Value {_value} is out of the range {_min} to {_max}.")
					.SetIsValid(false);
			}
		}

		/// <summary>
		/// Executes the rule asynchronously to validate the value.
		/// </summary>
		/// <remarks>
		/// Following the Command Pattern, this method is called from the base class'
		/// <see cref="RuleComponent.ExecuteAsync"/> method to execute the rule.
		/// </remarks>
		/// <returns></returns>
		public override Task<IWorkResult> VerifyAsync()
		{
			return Task.FromResult(Verify());
		}
	}
}