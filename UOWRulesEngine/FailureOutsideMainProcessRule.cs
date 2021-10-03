using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UOWRulesEngine
{
	/// <inheritdoc cref="WorkRule" path="*"/>
	public class FailureOutsideMainProcessRule : WorkRule
	{
		#region Fields
		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FailureOutsideMainProcessRule" />
		/// </summary>
		/// <param name="name">The name of the rule.</param>
		/// <param name="message">The error message to be displayed.</param>
		public FailureOutsideMainProcessRule(string name, string message)
			: base(name, message)
		{
		}

		#endregion

		#region Cores

		/// <summary>
		/// Verify the rule is satisfied.
		/// </summary>
		/// <remarks>
		/// This is a special implementation of a WorkRule. It's intended to always be a failed rule as it's used to
		/// report a failure in the <see cref="WorkAction"/> class that is utilizing it that has happened outside of
		/// the <see cref="WorkAction.AddRules(IList{IWorkRule})"/> processing pipeline but where a thrown
		/// <see cref="UnitOfWorkException"/> is not desireable.
		/// 
		/// For example, if a problem is detected in the results of a call to a method outside of the <see cref="WorkAction"/>,
		/// or other method within the <see cref="WorkAction"/> class, this rule can be added to the list of 
		/// <see cref="WorkRule"/> objects in the <see cref="IWorkValidation.Rules"/> property so that the failure can
		/// be gracefully reported to the calling process.
		/// 
		/// Do not add the <see cref="FailureOutsideMainProcessRule"/> object directly to the
		/// <see cref="IWorkValidation.Rules"/> property. Use the 
		/// </remarks>
		public override IWorkResult Verify()
		{
			IsValid = false;
			return new WorkResult(this);
		}

		#endregion
	}
}
