namespace UOWRulesEngine
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
		/// <inheritdoc cref="WorkResult.Name" select="summary"/>
		string Name { get; set; }
		/// <inheritdoc cref="WorkResult.Message" select="summary"/>
		string Message { get; set; }
		/// <inheritdoc cref="WorkResult.IsValid" select="summary"/>
		bool IsValid { get; set; }
	}
}
