namespace UOWRulesEngineCore
{
	/// <summary>
	/// Exposes the properties needed to validate a business rule.
	/// </summary>
	/// <seealso cref="IWorkRule">IWorkRule Interface</seealso>
	/// <seealso cref="WorkRule">WorkRule Class</seealso>
	/// <seealso cref="RuleComponent">RuleComponent Class</seealso>
	/// <seealso cref="IWorkValidation">IWorkValidation Interface</seealso>
	/// <seealso cref="WorkValidation">WorkValidation Class</seealso>
	/// <seealso cref="IWorkAction">IWorkAction Interface</seealso>
	/// <seealso cref="WorkAction">WorkAction Class</seealso>
	/// <seealso cref="WorkResult">WorkResult Class</seealso>
	/// <remarks>
	/// The properties exposed by the IWorkResult interface allows code utilizing the business rule engine to report on failed errors in unit tests, API
	/// responses and Views to name a few. The Name of the rule that passed/failed, the message that describes the failure and the IsValid bool flag field
	/// that determines if the rule failed or not are all exposed to code that implements the IWorkResult.
	/// </remarks>
	public interface IWorkResult
	{
		/// <summary>
		/// A string property containing the name of the business rule.
		/// </summary>
		string? Name { get; set; }

		/// <summary>
		/// A string property containing the message that describes why the business rule failed, or what conditions needed
		/// to be met for the rule to pass.
		/// </summary>
		string? Message { get; set; }

		/// <summary>
		/// A bool field indicating whether the business rule passed or failed.
		/// </summary>
		bool IsValid { get; set; }

		/// <summary>
		/// A bool value that allows the Unit of Work workflow to return a <see cref="WorkResult"/> as generating a warning.
		/// </summary>
		bool IsWarning { get; set; }

		/// <summary>
		/// A string containing a warning message that is set by the <see cref="WorkAction"/> or <see cref="WorkActionAsync"/>
		/// object that generated the warning.
		/// </summary>
		string? WarningMessage { get; set; }

		/// <summary>
		/// Fluent API method definition to set the <see cref="Name"/> property.
		/// </summary>
		/// <param name="value"></param>
		/// <returns>
		/// The current <see cref="WorkResult"/> object reference for this object (this) so that other Fluent methods can be chained.
		/// </returns>
		WorkResult SetName(string value);

		/// <summary>
		/// Fluent API method definition to set the <see cref="Message"/> property.
		/// </summary>
		/// <param name="value">A string value used to set the <see cref="Message"/> property.</param>
		/// <returns>
		/// The current <see cref="WorkResult"/> object reference for this object (this) so that other Fluent methods can be chained.
		/// </returns>
		WorkResult SetMessage(string value);

		/// <summary>
		/// Fluent API method definition to set the <see cref="IsValid"/> property.
		/// </summary>
		/// <param name="value">The bool value to set the property value.</param>
		/// <returns>
		/// The current <see cref="WorkResult"/> object reference for this object (this) so that other Fluent methods can be chained.
		/// </returns>
		WorkResult SetIsValid(bool value);
	}
}
