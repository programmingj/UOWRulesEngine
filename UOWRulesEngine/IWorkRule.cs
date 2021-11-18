namespace UOWRulesEngine
{
	/// <summary>
	/// Exposes the Execute() method following the Command pattern to work with <see cref="WorkRule" /> objects containing business rule logic for
	/// an <see cref="IWorkAction" />.
	/// </summary>
	/// <seealso cref="WorkRule"/>
	/// <seealso cref="RuleComponent"/>
	/// <seealso cref="IWorkAction"/>
	/// <seealso cref="WorkAction"/>
	/// <seealso cref="IWorkActionAsync"/>
	/// <seealso cref="WorkActionAsync"/>
	/// <seealso cref="IWorkResult"/>
	/// <seealso cref="WorkResult"/>
	public interface IWorkRule
	{
		/// <inheritdoc cref="RuleComponent.IsValid" path="*"/>
		bool IsValid { get; set; }

		/// <inheritdoc cref="RuleComponent.HasBeenProcessed" path="*"/>
		bool HasBeenProcessed { get; }

		/// <inheritdoc cref="RuleComponent.Execute()" path="*"/>
		IWorkResult Execute();
	}
}
