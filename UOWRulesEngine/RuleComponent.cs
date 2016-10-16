namespace UOWRulesEngine
{
	/// <summary>
	/// The abstract base class for the <see cref="WorkRule" /> abstract class.
	/// </summary>
	/// <seealso cref="IWorkRule">IWorkRule Interface</seealso>
	/// <seealso cref="WorkRule">WorkRule Class</seealso>
	/// <seealso cref="IWorkValidation">IWorkValidation Interface</seealso>
	/// <seealso cref="WorkValidation">WorkValidation Class</seealso>
	/// <seealso cref="IWorkAction">IWorkAction Interface</seealso>
	/// <seealso cref="WorkAction">WorkAction Class</seealso>
	/// <seealso cref="IWorkResult">IWorkResult Interface</seealso>
	/// <seealso cref="WorkResult">WorkResult Class</seealso>
	/// <remarks>
	/// The <see cref="IWorkRule" /> interface defines the Execute() method inplementing the Command pattern for the business rules. The rest of the
	/// functionality for business rules is implemented by inheriting from the <see cref="WorkRule" /> class, which in turn inherits from the
	/// <see cref="RuleComponent" /> class. This implements the <see cref="IWorkRule"/> interface and as such the implementation for Execute() is
	/// here. When implementing a new business rule by inheriting from the <see cref="WorkRule" /> class you have to override the <see cref="Verify()" />
	/// method with the business rule logic to decide if the rule is satisfied.
	/// </remarks>
	public abstract class RuleComponent : IWorkRule
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the RuleComponent class. This is the base class for the <see cref="WorkRule" /> class and allows custom rules to be
		/// created by inheriting from <see cref="WorkRule" /> then overriding the <see cref="RuleComponent.Verify"/> method to implement the business
		/// rule logic. These rules are then executed by the <see cref="IWorkAction"/> during it's processing pipeline before any work is actually
		/// started.  If the logic in these rules does not pass, the action's processing code is never executed.
		/// </summary>
		/// <param name="name">A string containing the name of the business rule.</param>
		/// <param name="message">A string containing the message that the calling code will display if the business rule fails.</param>
		protected RuleComponent(string name, string message)
		{
			Name = name;
			Message = message;
		}

		#endregion

		#region Properties

		/// <summary>
		/// The Name of the rule. Used by implementing code when a rule fails.
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// The rule failure Message that will be used by the implementing code if the rule fails.
		/// </summary>
		public string Message { get; set; }
		/// <summary>
		/// The flag that determines if the rule has passed verification. Set when the rule is executed ( Verify is executed from Execute() ).
		/// </summary>
		public bool IsValid { get; set; }

		#endregion

		#region Cores

		/// <summary>
		/// Implementation of the Execute method from the interface for the Command pattern.
		/// </summary>
		/// <returns>An <see cref="IWorkResult" /> object containing the results of the rule verification.</returns>
		public IWorkResult Execute()
		{
			return Verify();
		}

		/// <summary>
		/// Runs the business rule. 
		/// </summary>
		/// <remarks>
		/// This is central to the Command pattern as this is the method the <see cref="Execute()" /> method, exposed by the
		/// <see cref="IWorkRule" /> interface, calls.
		/// 
		/// Implementers: This is the method you need to override to create your rule. It is the actual rule logic.
		/// </remarks>
		/// <example>
		/// The Verify() method returns an <see cref="IWorkResult" /> object with the results of the rule check. This will determine if the rule has
		/// passed or failed to the calling code when it's time to verify all rules before executing the <see cref="WorkAction" />. If even one of the
		/// results objects is marked as IsValid == false then the <see cref="WorkAction" /> code will not be executed.
		/// <code>
		/// public override IWorkResult Verify()
		/// {
		/// 	IsValid = target.IsNullEmptyWhiteSpace() == false && target.Length == 9;
		/// 	return new WorkResult(this);
		/// }
		/// </code>
		/// </example>
		public abstract IWorkResult Verify();

		#endregion
	}
}
