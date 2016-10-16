using System.Collections.Generic;

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
		/// <inheritdoc cref="WorkValidation.IsValid" select="summary"/>
		bool IsValid { get; }
		/// <inheritdoc cref="WorkValidation.Rules" select="summary"/>
		IList<IWorkRule> Rules { get; set; }
		/// <inheritdoc cref="WorkValidation.Results" select="summary"/>
		IList<IWorkResult> Results { get; }
		/// <inheritdoc cref="WorkValidation.FailedResults" select="summary"/>
		IList<IWorkResult> FailedResults { get; }
		/// <inheritdoc cref="WorkValidation.PassedResults" select="summary"/>
		IList<IWorkResult> PassedResults { get; }
	}
}
