using System.Collections.Generic;

namespace UOWRulesEngineCore
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
		/// Calculated property returning only the <see cref="WorkResult" /> objects for rules that failed.
		/// </summary>
		IList<IWorkResult> FailedResults { get; }

		/// <summary>
		/// Calculated property returning only the <see cref="WorkResult" /> objects for rules that passed.
		/// </summary>
		IList<IWorkResult> PassedResults { get; }

		/// <summary>
		/// returning only the <see cref="WorkResult"/> objects for rules that generated a warning.
		/// </summary>
		IList<IWorkResult> Warnings { get; }

		#region Fluent API Method Definitions.

		/// <summary>
		/// Fluid API method used to set the <see cref="Rules"/> property.
		/// </summary>
		/// <param name="value">An <see cref="IList{T}"/> of <see cref="IWorkRule"/> objects.</param>
		/// <returns>This instance of a <see cref="WorkValidation"/> object.</returns>
		WorkValidation SetRules(IList<IWorkRule> value);

		/// <summary>
		/// Fluid API method used to set the <see cref="Results"/> property.
		/// </summary>
		/// <param name="value">An <see cref="IList{T}"/> of <see cref="IWorkResult"/> objects.</param>
		/// <returns>This instance of a <see cref="WorkValidation"/> object.</returns>
		WorkValidation SetResults(IList<IWorkResult> value);

		#endregion Fluent API Method Definitions.
	}
}
