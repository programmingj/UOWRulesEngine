using System.Transactions;

namespace UOWRulesEngine
{
	/// <summary>
	/// The results of an <see cref="IWorkAction" /> execution.
	/// </summary>
	public enum WorkActionResult
	{
		/// <summary>The WorkAction completed it's task successfully.</summary>
		Success,
		/// <summary>
		/// The WorkAction either has failing rule(s) or an Exception was thrown and a failed rule was added to the Rules
		/// collection with the exception information.
		/// </summary>
		Fail,
		/// <summary>The WorkAction either has not been executed yet or is currently running.</summary>
		Unknown
	}

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
		/// When this property is true the rule validation routine will stop on the first failed rule encountered and return execution to the
		/// WorkAction it belongs to. If false rule processing will continue until all rules have been validated so that all problems can be
		/// reported back to the calling code.
		/// </summary>
		public bool StopRuleProcessingOnFirstFailure { get; set; }

		#endregion
	}
}