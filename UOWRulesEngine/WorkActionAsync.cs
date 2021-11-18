using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using UOWRulesEngine.Rules;
using static UOWRulesEngine.Enums;

namespace UOWRulesEngine
{
	/// <summary>
	/// ============= TODO: Update the doc comments to describe the implementation details here =============
	/// The WorkActionAsync abstract class is the main component of the <see cref="UOWRulesEngine"/> Unit
	/// of Work pattern library. The child classes that implement the
	/// <see cref="WorkActionAsync"/> base class define a unit of work and the <see cref="WorkRule"/> objects
	/// that must be validated before the unit of work can be executed, and provides information about the
	/// success or failure of the action back to calling code.
	/// 
	/// By encapsulating units of work into a single class any business rules that have to pass for
	/// the described work to be completed can also be defined by any class implementing
	/// <see cref="WorkRule"/>, and the process of getting a single unit of work completed becomes
	/// standardized across all functionality in the codebase. Any class calling the
	/// <see cref="ExecuteAsync"/> method has a standard way of accessing information about the execution
	/// and it's success or failure as well as each individual business rule, whether it passed or
	/// failed and provides that data in a standardized way.
	/// 
	/// The business rules are added to the collection of business rules that must pass in order for
	/// the <see cref="ProcessActionAsync"/> method to execute via the <see cref="AddRulesAsync(IList{IWorkRule})"/>
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
	public abstract class WorkActionAsync : WorkActionBase, IWorkActionAsync
    {
		#region Constructors

		/// <summary>
		/// Default constructor. Instantializes a new instance of the <see cref="WorkActionAsync" /> class.
		/// Allows for object initializer usage.
		/// </summary>
		protected WorkActionAsync()
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
		protected WorkActionAsync(IWorkValidation workValidationContext)
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
		protected WorkActionAsync(WorkActionConfiguration config)
			: base (new WorkValidation(), config)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WorkAction" /> class, using the provided
		/// <see cref="WorkActionConfiguration" /> object to provide more fine tuned control over
		/// the <see cref="WorkAction" /> and to support performing all work inside of a
		/// <see cref="TransactionScope" /> when spanning multiple <see cref="WorkAction" />s when
		/// needed, and also the provided <see cref="IWorkValidation" /> to set the
		/// <see cref="WorkActionBase.WorkValidationContext"/> property for chaining actions.
		/// </summary>
		/// <param name="workValidationContext">A <see cref="WorkValidation"/> object.</param>
		/// <param name="config">
		/// A <see cref="WorkActionConfiguration" /> object containing the configuration settings.
		/// </param>
		protected WorkActionAsync(IWorkValidation workValidationContext, WorkActionConfiguration config)
			: base (workValidationContext, config)
		{
		}

		#endregion

		#region Properties

		#endregion

		#region Public Methods

		/// <inheritdoc cref="IWorkActionAsync.ExecuteAsync()" path="summary"/>
		public async Task ExecuteAsync()
		{
			try
			{
                ProcessingStage = WorkActionProcessingStage.PreAddRules;

				// Run any code needed before adding the rules to the collection.
				await PreAddRulesAsync();

				ProcessingStage = WorkActionProcessingStage.AddRules;

				// Add all of the business rules to the WorkValidation.Rules collection so that we can check
				// them before performing the action.
				await AddRulesAsync(WorkValidationContext.Rules);

				ProcessingStage = WorkActionProcessingStage.PreValidateRules;

				// Run any code that needs to run before the rules collection is validated.
				await PreValidateRulesAsync();

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
				await PreProcessActionAsync();

				ProcessingStage = WorkActionProcessingStage.ProcessAction;

				// If the rules checked out then go ahead and execute the action itself
				await ProcessActionAsync();

				ProcessingStage = WorkActionProcessingStage.PostProcessAction;

				// Run any post-processing code
				await PostProcessActionAsync();

				// Verify that the action succeeded by setting the Result property to a WorkActionResult value
				Result = await VerifyActionAsync();
			}
			catch (UnitOfWorkException exc)
			{
				ProcessingStage = WorkActionProcessingStage.ExceptionHandler;

				// Something went wrong somewhere between the business logic and the repository layer so report
				// it in the results.
				// If the current configuration says to just add the exception messages to the results then go
				// ahead.
				if (Configuration.UseExceptionMessageDuringExceptionHandling)
				{
					AddThrownExceptionRuleToResults(exc);
					return;
				}

				// The exception is marked as having a Message that is potentially not appropriate for display
				// so add a generic message.
				var exceptionRule = new ThrownExceptionRule(
					"HandledExceptionRule",
					Configuration.GenericExceptionMessageForThrownExceptionRule, exc);
				exceptionRule.Verify();

				WorkValidationContext.Results.Add(new WorkResult(exceptionRule));
			}
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Asynchronous version of the <see cref="WorkAction.PreAddRules"/> method.
		/// 
		/// This is the method to override if there is any code that needs to be executed before the rules are
		/// added to the collection. An example would be if data needed to be gathered or prepared in oroder to
		/// properly check the rules.
		/// </summary>
		/// <returns>A <see cref="Task"/> (void).</returns>
		/// <seealso cref="WorkAction.PreAddRules"/>
		protected virtual async Task PreAddRulesAsync()
		{
			await Task.CompletedTask;
		}

		/// <summary>
		/// Asynchronous version of the <see cref="WorkAction.AddRules(IList{IWorkRule})"/> method.
		/// 
		/// This is the method to override to add all of the business rules to the process. All business
		/// rules will be added to the <see cref="IWorkValidation.Rules" /> property so that they can be
		/// executed when the action is run to validate that the action can be performed.
		/// </summary>
		/// <param name="rules">An <see cref="IList{T}"/> of <see cref="IWorkRule"/> objects.</param>
		/// <returns>A <see cref="Task"/> (void).</returns>
		/// <seealso cref="WorkAction.AddRules(IList{IWorkRule})"/>
		protected virtual async Task AddRulesAsync(IList<IWorkRule> rules)
		{
			await Task.CompletedTask;
		}

		/// <summary>
		/// Asynchronous version of the <see cref="WorkAction.PreValidateRules"/> method.
		/// 
		/// This is the method to override if there is more work to be done to complete the rules check
		/// or to prepare data, etc. before the WorkAction's work can be completed.
		/// </summary>
		/// <returns>A <see cref="Task"/> (void).</returns>
		/// <seealso cref="WorkAction.PreValidateRules"/>
		protected virtual async Task PreValidateRulesAsync()
		{
			await Task.CompletedTask;
		}

		/// <summary>
		/// Asynchronous version of the <see cref="WorkAction.PreProcessAction"/> method.
		/// 
		/// Override this method to add any pre-processing actions you need to run before the actual
		/// action code is executed. Trace and logging code would be added here for example.
		/// </summary>
		/// <returns>A <see cref="Task"/> (void).</returns>
		/// <seealso cref="WorkAction.PreProcessAction"/>
		protected virtual async Task PreProcessActionAsync()
		{
			await Task.CompletedTask;
		}

		/// <summary>
		/// Asynchronous version of the <see cref="WorkAction.ProcessAction"/> method.
		/// 
		/// This is what needs to be overridden in derived classes in order to perform the action's
		/// tasks.  This is the actual work done.
		/// </summary>
		/// <returns>A <see cref="Task"/> (void).</returns>
		/// <seealso cref="WorkAction.ProcessAction"/>
		protected virtual async Task ProcessActionAsync()
		{
			await Task.CompletedTask;
		}

		/// <summary>
		/// Asynchronous version of the <see cref="WorkAction.PostProcessAction"/> method.
		/// 
		/// Override this method to add any post-processing actions you need to run after the actual
		/// action code is executed. Tracing and logging code would be added here for example. Also,
		/// if any UnitOfWorkException exceptions were handled you could access those here and do
		/// processing on them. The <see cref="WorkActionConfiguration" /> allows you to select
		/// automatic exception handling, which essentially adds the exception Message property to a
		/// failed <see cref="IWorkRule"/> object and adds it to the list of rules for the calling code
		/// to report to the end user.
		/// </summary>
		/// <returns>A <see cref="Task"/> (void).</returns>
		/// <seealso cref="WorkAction.PostProcessAction"/>
		protected virtual async Task PostProcessActionAsync()
		{
			await Task.CompletedTask;
		}

		/// <summary>
		/// Asynchronous version of the <see cref="WorkAction.VerifyAction"/> method.
		/// 
		/// This is the code to override when you need the action to return a success/fail value. It checks that the work of the action was
		/// actually completed successfully once the work is performed.
		/// </summary>
		/// <returns>A <see cref="Task{TResult}"/> of <see cref="WorkActionResult"/>.</returns>
		/// <seealso cref="WorkAction.PreValidateRules"/>
		protected virtual async Task<WorkActionResult> VerifyActionAsync()
		{
			return await Task.FromResult(WorkActionResult.Success);
		}

		#endregion
	}
}
