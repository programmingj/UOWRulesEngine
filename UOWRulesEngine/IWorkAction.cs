namespace UOWRulesEngine
{
	/// <summary>
	/// Exposes the Execute() method following the Command pattern to work with <see cref="WorkAction" /> objects containing the logic for the action
	/// to be performed.
	/// </summary>
	/// <seealso cref="IWorkRule">IWorkRule Interface</seealso>
	/// <seealso cref="WorkRule">WorkRule Class</seealso>
	/// <seealso cref="RuleComponent">RuleComponent Class</seealso>
	/// <seealso cref="IWorkValidation">IWorkValidation Interface</seealso>
	/// <seealso cref="WorkValidation">WorkValidation Class</seealso>
	/// <seealso cref="WorkAction">WorkAction Class</seealso>
	/// <seealso cref="IWorkResult">IWorkResult Interface</seealso>
	/// <seealso cref="WorkResult">WorkResult Class</seealso>
	public interface IWorkAction
	{
		/// <inheritdoc cref="WorkAction.Result" select="summary"/>
		WorkActionResult Result { get; }

		/// <inheritdoc cref="WorkAction.Execute()" select="summary"/>
		void Execute();
	}
}
