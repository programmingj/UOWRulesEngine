using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace UOWRulesEngine
{
	/// <summary>
	/// Used by code implementing the Unit Of Work library to allow <see cref="WorkAction" /> objects to store and check business rules 
	/// (<see cref="WorkRule" /> objects), execute those rules before performing the action code and store the results of those rule executions.
	/// </summary>
	/// <seealso cref="IWorkRule">IWorkRule Interface</seealso>
	/// <seealso cref="WorkRule">WorkRule Class</seealso>
	/// <seealso cref="RuleComponent">RuleComponent Class</seealso>
	/// <seealso cref="IWorkValidation">IWorkValidation Interface</seealso>
	/// <seealso cref="IWorkAction">IWorkAction Interface</seealso>
	/// <seealso cref="WorkAction">WorkAction Class</seealso>
	/// <seealso cref="IWorkResult">IWorkResult Interface</seealso>
	/// <seealso cref="WorkResult">WorkResult Class</seealso>
	/// <remarks>
	/// The WorkValidation object contains the properties used to determine if a <see cref="WorkAction" /> can be executed, allows for business rules 
	/// (<see cref="WorkRule" /> objects) to be added and executed and allows calling code to examine the results of executing each of the business rules.
	/// Unlike the <see cref="IWorkValidation" /> interface the class also exposes the <see cref="ValidateRules()" /> method that actually executes each
	/// of the business rules to safely ensure the data for an action satisfies all of the business rules before the action is executed.
	/// 
	/// The <see cref="WorkValidation" /> object is primarily used by the <see cref="WorkAction" /> class to process business rules and actions, and
	/// is then later queried by API endpoints and unit tests to report problems with the action execution and to verify that all business rules have
	/// been properly checked before an action is taken against data.
	/// 
	/// In order to chain together actions in a single unit of work such as a Service call, the <see cref="WorkAction" /> constructor overload accepting an 
	/// <see cref="IWorkValidation" /> object ( <see cref="WorkAction(IWorkValidation)" /> ) is used to instantiate the instance so that the business
	/// rules are shared and executed together in the appropriate order to ensure data integrity.
	/// </remarks>
	public class WorkValidation : IWorkValidation
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="WorkValidation" /> class. Allows for object initializer usage.
		/// </summary>
		/// <remarks>
		/// The WorkValidation class is the main component for validating a <see cref="WorkAction" /> call and reporting the results to calling code.
		/// The <see cref="WorkAction" /> either instantiates a new WorkValidation object in it's constructor or uses one provided to it's constructor
		/// so that multiple <see cref="WorkAction" /> tasks can be chained together.
		/// 
		/// The <see cref="WorkAction" /> then adds rules to the Rules collection via the <see cref="WorkAction.AddRules(IList&lt;IWorkRule&gt;)"/> method
		/// overridden in the child class for the action being executed. Each of those <see cref="WorkRule" /> objects is then executed via their
		/// <see cref="IWorkRule.Execute()" /> (command pattern) to ensure business rules are validated. This process adds a <see cref="WorkResult" />
		/// object to the Results collection for each business rule executed.
		/// 
		/// The collections <see cref="FailedResults" /> and <see cref="PassedResults" /> simply extract out the passed and failed <see cref="WorkResult" />
		/// objects from the <see cref="Results" /> collection via LINQ.
		/// </remarks>
		public WorkValidation()
		{
			Rules = new List<IWorkRule>();
			Results = new List<IWorkResult>();
		}

		/// <inheritdoc cref="WorkValidation()" select="summary|remarks"/>
		public WorkValidation(TransactionScope transactionContext)
		{
			Rules = new List<IWorkRule>();
			Results = new List<IWorkResult>();
			TransactionContext = transactionContext;
		}

		#endregion

		#region Properties

		/// <summary>
		/// A bool value that is used to determine if all of the business rules are valid and the action can proceed.
		/// </summary>
		public bool IsValid { get { return Results.All(x => x.IsValid); } }
		/// <summary>
		/// An IList&lt;<see cref="WorkRule" />&gt; collection containing all of the business rules that have to succeed before a <see cref="WorkAction" />
		/// can be executed.
		/// </summary>
		public IList<IWorkRule> Rules { get; set; }
		/// <summary>
		/// An IList&lt;<see cref="WorkResult" />&gt; collection containing the results of each of the <see cref="WorkRule" /> objects executed to validate
		/// the business rules for a <see cref="WorkAction" />.
		/// </summary>
		public IList<IWorkResult> Results { get; private set; }

		/// <summary>
		/// Returns only the <see cref="WorkResult" /> objects for rules that failed.
		/// </summary>
		public IList<IWorkResult> FailedResults
		{
			get { return Results.Where(x => x.IsValid == false).ToList(); }
		}
		
		/// <summary>
		/// Used to run the <see cref="WorkAction" /> in a transaction when running multiple actions together.
		/// </summary>
		/// <remarks>
		/// If only 1 action is being executed there is no reason to have a <see cref="TransactionScope" /> as no work is performed if any of the 
		/// business rules fail unless multiple changes are being made by a single action. For instance, if you are updating multiple rows of data 
		/// in a single action you could use the scope to handle rolling back changes if any of the steps fail. But this is handled by the method
		/// override and as such the developer already has full control over the rollback capabilities of the action.
		/// </remarks>
		public TransactionScope TransactionContext { get; set; }

		/// <summary>
		/// Returns only the <see cref="WorkResult" /> objects for rules that passed.
		/// </summary>
		public IList<IWorkResult> PassedResults
		{
			get { return Results.Where(x => x.IsValid).ToList(); }
		}

		//TODO: Add another collection for warning results if we need them later.

		#endregion

		#region Methods

		/// <summary>
		/// Executes all of the <see cref="WorkRule" /> commands (via the <see cref="IWorkRule" /> interface) so that we can determine if the 
		/// <see cref="WorkAction" /> can be executed.
		/// </summary>
		public void ValidateRules()
		{
			foreach (IWorkRule rule in Rules)
			{
				Results.Add(rule.Execute());
			}
		}

		#endregion
	}
}