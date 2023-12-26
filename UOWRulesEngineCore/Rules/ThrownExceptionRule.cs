namespace UOWRulesEngineCore.Rules
{
	/// <summary>
	/// Used to add a failed rule to the rules collection when an exception has been trapped
	/// and needs to be reported back to the calling code.
	/// </summary>
	public class ThrownExceptionRule : WorkRule
	{
		#region Fields

		private readonly Exception target;

		#endregion

		#region Constructors

		/// <summary>
		/// Default constructor. Instantiates a new <see cref="ThrownExceptionRule"/> object.
		/// </summary>
		/// <param name="name">The name of the rule.</param>
		/// <param name="message">The error message to be displayed.</param>
		/// <param name="target">The Exception being handled.</param>
		public ThrownExceptionRule(string name, string message, Exception target)
			: base(name, message)
		{
			this.target = target;
		}

		#endregion

		#region Public methods.

		/// <summary>
		/// Verify the rule is satisfied.
		/// </summary>
		/// <remarks>
		/// This is a special type of rule that is used to report back a trapped exception to the calling code.
		/// The logic here reflects that situation by simply setting IsValid to false.
		/// </remarks>
		public override IWorkResult Verify()
		{
			IsValid = false;
			return new WorkResult(this);
		}

		/// <summary>
		/// Override of the base class abstract method <see cref="RuleComponent.VerifyAsync"/>.
		/// </summary>
		/// <returns>A <see cref="Task{TResult}">Task{IWorkResult}</see> object.</returns>
		/// <exception cref="NotImplementedException"></exception>
		public override Task<IWorkResult> VerifyAsync()
		{
			throw new NotImplementedException();
		}

		#endregion // End Public Methods.
	}
}
