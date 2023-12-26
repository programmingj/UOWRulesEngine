using System.Transactions;
using UOWRulesEngineCore.Rules;
using static UOWRulesEngineCore.Enums;

namespace UOWRulesEngineCore
{
	/// <summary>
	/// ============= TODO: Update the doc comments to describe the implementation details here =============
	/// The WorkAction abstract class is the main component of the <see cref="UOWRulesEngineCore"/> Unit
	/// of Work pattern library. The child classes that implement the WorkAction base class define a
	/// unit of work and the rules that must be validated before the unit of work can be executed, and
	/// provides information about the success or failure of the action back to calling code.
	/// 
	/// By encapsulating units of work into a single class any business rules that have to pass for
	/// the described work to be completed can also be defined by any class implementing
	/// <see cref="WorkRule"/>, and the process of getting a single unit of work completed becomes
	/// standardized across all functionality in the codebase. Any class calling a WorkAction's
	/// <see cref="Execute"/> method has a standard way of accessing information about the execution
	/// and it's success or failure as well as each individual business rule, whether it passed or
	/// failed and provides that data in a standardized way.
	/// 
	/// The business rules are added to the collection of business rules that must pass in order for
	/// the <see cref="ProcessAction"/> method to execute via the <see cref="AddRules(IList{IWorkRule})"/>
	/// method.
	/// =====================================================================================================
	/// 
	/// </summary>
	/// <seealso cref="IWorkRule">IWorkRule Interface</seealso>
	/// <seealso cref="WorkRule">WorkRule Class</seealso>
	/// <seealso cref="RuleComponent">RuleComponent Class</seealso>
	/// <seealso cref="IWorkValidation">IWorkValidation Interface</seealso>
	/// <seealso cref="WorkValidation">WorkValidation Class</seealso>
	/// <seealso cref="IWorkAction">IWorkAction Interface</seealso>
	/// <seealso cref="IWorkResult">IWorkResult Interface</seealso>
	/// <seealso cref="WorkResult">WorkResult Class</seealso>
	public abstract class WorkAction : WorkActionBase, IWorkAction
	{
		#region Constructors

		/// <summary>
		/// Default constructor. Instantializes a new instance of the <see cref="WorkAction" /> class.
		/// Allows for object initializer usage.
		/// </summary>
		protected WorkAction()
			: base (new WorkValidation(), new WorkActionConfiguration())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WorkAction" /> class, using the provided
		/// <see cref="IWorkValidation" />.
		/// </summary>
		/// <remarks>
		/// Use this constructor to initialize a new instance of the <see cref="WorkAction" />
		/// class when chaining multiple actions in a single service call. This allows all business
		/// rules to be executed in the correct order before any work is done in an action.
		/// </remarks>
		/// <param name="workValidationContext">A <see cref="WorkValidation"/> object.</param>
		protected WorkAction(IWorkValidation workValidationContext)
			: base (workValidationContext, new WorkActionConfiguration())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WorkAction" /> class, using the provided
		/// <see cref="WorkActionConfiguration" /> object to provide more fine tuned control over
		/// the <see cref="WorkAction" />.
		/// </summary>
		/// <param name="config">
		/// A <see cref="WorkActionConfiguration" /> object containing the configuration settings.
		/// </param>
		protected WorkAction(WorkActionConfiguration config)
			: base (new WorkValidation(), config)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WorkAction" /> class, using the provided
		/// <see cref="WorkActionConfiguration" /> object to provide more fine tuned control over
		/// the <see cref="WorkAction" /> and to support performing all work inside of a
		/// <see cref="TransactionScope" /> when chaining multiple <see cref="WorkAction"/> objects when
		/// needed, and also the provided <see cref="IWorkValidation" /> to set the
		/// <see cref="WorkActionBase.WorkValidationContext"/> property for chaining actions.
		/// </summary>
		/// <param name="workValidationContext">A <see cref="WorkValidation"/> object.</param>
		/// <param name="config">
		/// A <see cref="WorkActionConfiguration" /> object containing the configuration settings.
		/// </param>
		protected WorkAction(IWorkValidation workValidationContext, WorkActionConfiguration config)
			: base (workValidationContext, config)
		{
		}

		#endregion

		#region Properties

		/// <summary>
		/// Stores a reference to the parent <see cref="WorkAction"/> class so that the parent's properties can be examined
		/// and the <see cref="IWorkRule"/> objects can be added to it's <see cref="WorkValidation.Rules"/> collection.
		/// </summary>
		public WorkAction? ParentWorkAction { get; set; }

		#endregion

		#region Public Methods

		/// <inheritdoc cref="IWorkAction.Execute()" path="*"/>
		public void Execute()
		{
			try
			{
				ProcessingStage = WorkActionProcessingStage.PreAddRules;

				// Run any code needed before adding the rules to the collection.
				PreAddRules();

				ProcessingStage = WorkActionProcessingStage.AddRules;

				// Add all of the business rules to the WorkValidation.Rules collection so that we can
				// check them before performing the action.
				// NOTE: If this is a Child work action the rules can't be added to the internal WorkValidationContext.Rules
				// collection. They have to be added to the parent work action's rules collection.
				if (IsChildAction && ParentWorkAction != null)
				{
					AddRules(ParentWorkAction.WorkValidationContext.Rules);
				}
				else
				{
					AddRules(WorkValidationContext.Rules);
				}

				ProcessingStage = WorkActionProcessingStage.PreValidateRules;

				// Run any code that needs to run before the rules collection is validated.
				PreValidateRules();

				ProcessingStage = WorkActionProcessingStage.ValidateRules;

				// Execute all of the rules to make sure we can run the action
				((WorkValidation)WorkValidationContext).ValidateRules();

				// If validation failed return
				if (WorkValidationContext.IsValid == false)
				{
					Result = WorkActionResult.Fail;
					return;
				}

				ProcessingStage = WorkActionProcessingStage.PreProcessAction;

				// Run any pre-processing code
				PreProcessAction();

				ProcessingStage = WorkActionProcessingStage.ProcessAction;

				// If the rules checked out then go ahead and execute the action itself
				ProcessAction();

				ProcessingStage = WorkActionProcessingStage.PostProcessAction;

				// Run any post-processing code
				PostProcessAction();

				// Verify that the action succeeded by setting the Result property to a WorkActionResult
				// value.
				Result = VerifyAction();
			}
			catch (UnitOfWorkException exc)
			{
				ProcessingStage = WorkActionProcessingStage.ExceptionHandler;

				// Something went wrong somewhere between the business logic and the repository layer
				// so report it in the results.
				// If the current configuration says to just add the exception messages to the results
				// then go ahead.
				if (Configuration.UseExceptionMessageDuringExceptionHandling)
				{
					AddThrownExceptionRuleToResults(exc);
					return;
				}

				// The exception is marked as having a Message that is potentially not appropriate
				// for display so add a generic message.
				var exceptionRule = new ThrownExceptionRule(
					"HandledExceptionRule",
					Configuration.GenericExceptionMessageForThrownExceptionRule, exc);
				exceptionRule.Verify();

				WorkValidationContext.Results.Add(new WorkResult((RuleComponent)exceptionRule));
			}
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// This is the method to override if there is any code that needs to be executed before the rules are
		/// added to the collection. An example would be if data needed to be gathered or prepared in oroder to
		/// properly check the rules.
		/// </summary>
		protected virtual void PreAddRules()
		{
		}

		/// <summary>
		/// This is the method to override if there is more work to be done to complete the rules check or
		/// to prepare data, etc. before the WorkAction's work can be completed.
		/// </summary>
		protected virtual void PreValidateRules()
		{
		}

		/// <summary>
		/// This is the method to override to add all of the business rules to the process. All business
		/// rules will be added to the <see cref="IWorkValidation.Rules" /> property so that they can be
		/// executed when the action is run to validate that the action can be performed.
		/// </summary>
		protected virtual void AddRules(IList<IWorkRule> rules)
		{
		}

		/// <summary>
		/// Override this method to add any pre-processing actions you need to run before the actual
		/// action code is executed. Trace and logging code would be added here for example.
		/// </summary>
		protected virtual void PreProcessAction()
		{
		}

		/// <summary>
		/// This is what needs to be overridden in derived classes in order to perform the action's
		/// tasks.  This is the actual work done.
		/// </summary>
		protected virtual void ProcessAction()
		{
		}

		/// <summary>
		/// Override this method to add any post-processing actions you need to run after the actual
		/// action code is executed. Tracing and logging code would be added here for example. Also,
		/// if any UnitOfWorkException exceptions were handled you could access those here and do
		/// processing on them. The <see cref="WorkActionConfiguration" /> allows you to select
		/// automatic exception handling, which essentially adds the exception Message property to a
		/// failed <see cref="IWorkRule"/> object and adds it to the list of rules for the calling code
		/// to report to the end user.
		/// </summary>
		protected virtual void PostProcessAction()
		{
		}

		/// <summary>
		/// This is the code to override when you need the action to return a success/fail value. It
		/// checks that the work of the action was actually completed successfully once the work is
		/// performed.
		/// </summary>
		/// <returns>A <see cref="WorkActionResult"/> with the results of the operation.</returns>
		protected virtual WorkActionResult VerifyAction()
		{
			return Result;
		}

		#endregion
	}
}