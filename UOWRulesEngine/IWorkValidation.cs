using System.Collections.Generic;
using System.Transactions;

namespace UOWRulesEngine
{
	/// <summary>
	/// Used by code implementing the Unit Of Work library to present the essential properties of the WorkValidation class.
	/// </summary>
	/// <seealso cref="IWorkRule">IWorkRule Interface</seealso>
	/// <seealso cref="WorkRule">WorkRule Class</seealso>
	/// <seealso cref="RuleComponent">RuleComponent Class</seealso>
	/// <seealso cref="WorkValidation">WorkValidation Class</seealso>
	/// <seealso cref="IWorkAction">IWorkAction Interface</seealso>
	/// <seealso cref="WorkAction">WorkAction Class</seealso>
	/// <seealso cref="IWorkResult">IWorkResult Interface</seealso>
	/// <seealso cref="WorkResult">WorkResult Class</seealso>
	/// <remarks>
	/// The IWorkValidation interface exposes the properties of a <see cref="WorkValidation" /> object to determine if a <see cref="WorkAction" />
	/// can be executed, allow for business rules (<see cref="WorkRule" /> objects) to be added and executed and examine the results of executing each
	/// of the business rules.
	/// 
	/// The <see cref="WorkValidation" /> object is primarily used by the <see cref="WorkAction" /> class to process business rules and actions, and
	/// is then later queried by API endpoints and unit tests to report problems with the action execution and to verify that all business rules have
	/// been properly checked before an action is taken against data.
	/// </remarks>
	public interface IWorkValidation
	{
		/// <summary>
		/// A bool value that is used to determine if all of the business rules are valid and the action can proceed.
		/// </summary>
		bool IsValid { get; }
		/// <summary>
		/// An IList&lt;<see cref="WorkRule" />&gt; collection containing all of the business rules that have to succeed before a <see cref="WorkAction" />
		/// can be executed.
		/// </summary>
		IList<IWorkRule> Rules { get; set; }
		/// <summary>
		/// An IList&lt;<see cref="WorkResult" />&gt; collection containing the results of each of the <see cref="WorkRule" /> objects executed to validate
		/// the business rules for a <see cref="WorkAction" />.
		/// </summary>
		IList<IWorkResult> Results { get; }
		/// <summary>
		/// Returns only the <see cref="WorkResult" /> objects for rules that failed.
		/// </summary>
		IList<IWorkResult> FailedResults { get; }
		/// <summary>
		/// Returns only the <see cref="WorkResult" /> objects for rules that passed.
		/// </summary>
		IList<IWorkResult> PassedResults { get; }
		/// <summary>
		/// Used to run the <see cref="WorkAction" /> in a transaction when running multiple actions together.
		/// </summary>
		/// <remarks>
		/// If only 1 action is being executed there is no reason to have a <see cref="TransactionScope" /> as no work is performed if any of the 
		/// business rules fail unless multiple changes are being made by a single action. For instance, if you are updating multiple rows of data 
		/// in a single action you could use the scope to handle rolling back changes if any of the steps fail. But this is handled by the method
		/// override and as such the developer already has full control over the rollback capabilities of the action.
		/// </remarks>
		TransactionScope TransactionContext { get; set; }
	}
}
