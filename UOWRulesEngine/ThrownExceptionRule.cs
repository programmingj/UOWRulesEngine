using System;

namespace UOWRulesEngine
{
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
