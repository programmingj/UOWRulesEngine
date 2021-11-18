namespace UOWRulesEngine
{
	/// <summary>
	/// This is the abstract class that the business rules are based on. The class follows the Command Pattern (via the <see cref="RuleComponent" />
	/// parent class) and as such there is really only one method that gets "exposed" for code that uses the business rule objects and that's the
	/// <see cref="RuleComponent.Execute()" /> method. The execute method executes the <see cref="RuleComponent.Verify()" /> method under the
	/// hood, which is the method that needs to be overridden to implement the business rule in the class that inherits from WorkRule.
	/// </summary>
	/// <seealso cref="IWorkRule">IWorkRule Interface</seealso>
	/// <seealso cref="RuleComponent">RuleComponent Class</seealso>
	/// <seealso cref="RuleComponent.Verify()">RuleComponent.Verify() Method</seealso>
	/// <seealso cref="RuleComponent.Execute()">RuleComponent.Execute() Method</seealso>
	/// <seealso cref="IWorkValidation">IWorkValidation Interface</seealso>
	/// <seealso cref="WorkValidation">WorkValidation Class</seealso>
	/// <seealso cref="IWorkAction">IWorkAction Interface</seealso>
	/// <seealso cref="WorkAction">WorkAction Class</seealso>
	/// <seealso cref="IWorkResult">IWorkResult Interface</seealso>
	/// <seealso cref="WorkResult">WorkResult Class</seealso>
	public abstract class WorkRule : RuleComponent, IWorkRule
	{
		#region Constructors

		/// <summary>
		/// Creates a new instance of the WorkRule class. WorkRule is the class that will be inherited and overridden to create a new business
		/// rule that must be satisfied in order for an <see cref="IWorkAction"/> to perform it's task.
		/// </summary>
		/// <param name="name">A string containing the name of the business rule.</param>
		/// <param name="message">A string containing the message that the calling code will display if the business rule fails.</param>
		protected WorkRule(string name, string message)
			: base(name, message)
		{
			Name = name;
			Message = message;
		}

		#endregion
	}
}