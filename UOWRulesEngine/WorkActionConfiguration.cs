using System.Transactions;

namespace UOWRulesEngine
{
	/// <summary>
	/// Contains properties for the various options available. For example the StopRuleProcessingOnFirstFailure property determines whether all rules
	/// in the collection will be processed for a WorkAction, or if rule validation stops on the first failed rule encountered.
	/// </summary>
	public class WorkActionConfiguration
	{
		#region Constructors

		/// <summary>
		/// Constructor overload accepting a <see cref="TransactionScope" /> object to use for chaining multiple <see cref="IWorkAction" /> calls together 
		/// and there is a need for transactions spanning those actions.
		/// </summary>
		/// <param name="transaction">A <see cref="TransactionScope" /> object that is shared between IWorkActions.</param>
		public WorkActionConfiguration(TransactionScope transaction = null)
		{
			Transaction = transaction;
			UseExceptionMessageDuringExceptionHandling = true;

			// Set the default value for the generic error message to use when the UseExceptionMessageDuringExceptionHandling is false.
			GenericExceptionMessageForThrownExceptionRule = Constants.GENERIC_EXCEPTION_MESSAGE;
		}

		#endregion

		#region Properties

		/// <summary>
		/// A <see cref="TransactionScope" /> object that provides transaction support that can span multiple <see cref="IWorkAction" />.
		/// </summary>
		public TransactionScope Transaction { get; private set; }

		/// <summary>
		/// Determines if the exception handling during the action processing simply uses the Message property of the UnitOfWorkException or
		/// any other child exception or if custom exception handling is required. Not all error handling situations are appropriate for using
		/// the Message, this allows that to be taken into account.
		/// </summary>
		public bool UseExceptionMessageDuringExceptionHandling { get; set; }

		/// <summary>
		/// The generic error message to use for exceptions that happen after the <see cref="WorkRule"/> objects have all been validated.
		/// </summary>
		public string GenericExceptionMessageForThrownExceptionRule { get; set; }

		/// <summary>
		/// When this property is true the rule validation routine will stop on the first failed rule encountered and return execution to the
		/// WorkAction it belongs to. If false rule processing will continue until all rules have been validated so that all problems can be
		/// reported back to the calling code.
		/// </summary>
		public bool StopRuleProcessingOnFirstFailure { get; set; }             

		#endregion
	}
}