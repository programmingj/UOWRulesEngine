using System.Transactions;

namespace UOWRulesEngine
{
	/// <summary>
	/// The results of an <see cref="IWorkAction" /> execution.
	/// </summary>
	public enum WorkActionResult
	{
		Success,
		Fail,
		Unknown
	}

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

		#endregion
	}
}