using System;

namespace UOWRulesEngine
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
		/// Initializes a new instance of the <see cref="ThrownExceptionRule" />
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

		#region Cores

		/// <summary>
		/// Verify the rule is satisfied
		/// </summary>
		public override IWorkResult Verify()
		{
			IsValid = target == null;
			return new WorkResult(this);
		}

		#endregion
	}
}
