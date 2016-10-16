namespace UOWRulesEngine
{
	/// <summary>
	/// Exposes the Execute() method following the Command pattern to work with <see cref="WorkRule" /> objects containing business rule logic for
	/// an <see cref="IWorkAction" />.
	/// </summary>
	/// <seealso cref="WorkRule">WorkRule Class</seealso>
	/// <seealso cref="RuleComponent">RuleComponent Class</seealso>
	/// <seealso cref="IWorkValidation">IWorkValidation Interface</seealso>
	/// <seealso cref="WorkValidation">WorkValidation Class</seealso>
	/// <seealso cref="IWorkAction">IWorkAction Interface</seealso>
	/// <seealso cref="WorkAction">WorkAction Class</seealso>
	/// <seealso cref="IWorkResult">IWorkResult Interface</seealso>
	/// <seealso cref="WorkResult">WorkResult Class</seealso>
	public interface IWorkRule
	{
		/// <inheritdoc cref="RuleComponent.Execute()" select="summary"/>
		IWorkResult Execute();
	}
}
