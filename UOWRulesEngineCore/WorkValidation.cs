using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UOWRulesEngineCore
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
		/// Default constructor. Instantiates a new <see cref="WorkValidation"/> object.
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
			Configuration = new WorkActionConfiguration();
		}

		/// <summary>
		/// Constructor overload. Instantiates a new <see cref="WorkValidation"/> object.
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
		/// <param name="configuration">
		/// A <see cref="WorkActionConfiguration"/> object containing application configuration settings.
		/// </param>
		public WorkValidation(WorkActionConfiguration configuration)
		{
			Rules = new List<IWorkRule>();
			Results = new List<IWorkResult>();
			Configuration = configuration;
		}

		#endregion

		#region Properties

		/// <summary>
		/// A <see cref="WorkActionConfiguration"/> object containing application configuration settings.
		/// </summary>
		internal WorkActionConfiguration Configuration { get; set; }

		/// <inheritdoc cref="IWorkValidation.IsValid" path="*"/>
		public bool IsValid { get { return Results.All(x => x.IsValid); } }

		/// <inheritdoc cref="IWorkValidation.Rules" path="*"/>
		public IList<IWorkRule> Rules { get; set; }

		/// <inheritdoc cref="IWorkValidation.Results" path="*"/>
		public IList<IWorkResult> Results { get; private set; }

		/// <inheritdoc cref="IWorkValidation.FailedResults" path="*"/>
		public IList<IWorkResult> FailedResults
		{
			get { return Results.Where(x => x.IsValid == false).ToList(); }
		}

		/// <inheritdoc cref="IWorkValidation.PassedResults" path="*"/>
		public IList<IWorkResult> PassedResults
		{
			get { return Results.Where(x => x.IsValid).ToList(); }
		}

		/// <inheritdoc cref="IWorkValidation.Warnings" path="*"/>
		public IList<IWorkResult> Warnings
		{
			get { return Results.Where(x => x.IsWarning).ToList(); }
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Executes all of the <see cref="WorkRule" /> commands (via the <see cref="IWorkRule" /> interface) so that we can determine if the 
		/// unit of work defined by the <see cref="WorkAction" /> can be executed.
		/// </summary>
		/// <remarks>
		/// This method loops through all of the <see cref="IWorkRule"/> objects in the <see cref="Rules"/> collection and validates them
		/// to determine whether or not the unit of work defined by the <see cref="WorkAction"/> or <see cref="WorkActionAsync"/> can be executed.
		/// 
		/// The value of the <see cref="WorkActionConfiguration.StopRuleProcessingOnFirstFailure"/> property determines if the processing will
		/// stop or continue upon any of the <see cref="WorkRule"/> objects failing validation. If the property is true and a rule fails validation
		/// processing of the rules is immediately stopped and execution is returned to the calling code. If it's false processing of the rules
		/// continues until all rules have been processed before returning to the calling code.
		/// </remarks>
		public void ValidateRules ()
		{
			foreach (IWorkRule rule in Rules)
			{
				Results.Add(rule.Execute());
				if (rule.IsValid == false && Configuration.StopRuleProcessingOnFirstFailure)
				{
					return;
				}
			}
		}

		/// <summary>
		/// Executes all of the <see cref="WorkRule" /> commands (via the <see cref="IWorkRule"/> interface) so that we can determine if the 
		/// unit of work defined by the <see cref="WorkAction" /> or <see cref="WorkActionAsync"/> objects can be executed.
		/// </summary>
		/// <remarks>
		/// This method loops through all of the <see cref="IWorkRule"/> objects in the <see cref="Rules"/> collection and validates them
		/// to determine whether or not the unit of work defined by the <see cref="WorkAction"/> or <see cref="WorkActionAsync"/> can be executed.
		/// 
		/// The value of the <see cref="WorkActionConfiguration.StopRuleProcessingOnFirstFailure"/> property determines if the processing will
		/// stop or continue upon any of the <see cref="WorkRule"/> objects failing validation. If the property is true and a rule fails validation
		/// processing of the rules is immediately stopped and execution is returned to the calling code. If it's false processing of the rules
		/// continues until all rules have been processed before returning to the calling code.
		/// </remarks>
		public async Task ValidateRulesAsync()
		{
			foreach (IWorkRule rule in Rules)
			{
				Results.Add(await rule.ExecuteAsync());
				if (rule.IsValid == false && Configuration.StopRuleProcessingOnFirstFailure)
				{
					return;
				}
			}
		}

		#endregion

		#region Fluent API Methods

		/// <inheritdoc cref="IWorkValidation.SetRules(IList{IWorkRule})" path="*"/>
		public WorkValidation SetRules (IList<IWorkRule> value)
		{
			Rules = value;
			return this;
		}

		/// <inheritdoc cref="IWorkValidation.SetResults(IList{IWorkResult})" path="*"/>
		public WorkValidation SetResults (IList<IWorkResult> value)
		{
			Results = value;
			return this;
		}

		/// <summary>
		/// Fluid API method used to set the <see cref="Configuration"/> property.
		/// </summary>
		/// <param name="value">A <see cref="WorkActionConfiguration"/> object containing application configuration.</param>
		/// <returns>This instance of a <see cref="WorkValidation"/> object.</returns>
		public WorkValidation SetConfiguration (WorkActionConfiguration value)
		{
			Configuration = value;
			return this;
		}

		#endregion
	}
}