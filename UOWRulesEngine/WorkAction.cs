using System;
using System.Collections.Generic;
using System.Transactions;

namespace UOWRulesEngine
{
	/// <summary>
	/// ==================================== TODO: Update the doc comments to describe the implementation details here ===========================================
	/// The WorkAction abstract class is the main component of the <see cref="UOWRulesEngine"/> unit of work pattern library. The child classes that implement
	/// the WorkAction base class define a unit of work and the rules that must be validated before the unit of work can be executed, and provides information
	/// about the success or failure of the action back to calling code.
	/// 
	/// By encapsulating units of work into a single class any business rules that have to pass for the described work to be completed can also be defined by any
	/// class implementing <see cref="WorkRule"/>, and the process of getting a single unit of work completed becomes standardized across all functionality in
	/// the codebase. Any class calling a WorkAction's <see cref="Execute"/> method has a standard way of accessing information about the execution and it's
	/// success or failure as well as each individual business rule, whether it passed or failed and provides that data in a standardized way.
	/// 
	/// The business rules are added to the collection of business rules that must pass in order for the <see cref="ProcessAction"/> method to execute via the
	/// ==========================================================================================================================================================
	/// <see cref="AddRules(IList{IWorkRule})"/> method.
	/// </summary>
	/// <seealso cref="IWorkRule">IWorkRule Interface</seealso>
	/// <seealso cref="WorkRule">WorkRule Class</seealso>
	/// <seealso cref="RuleComponent">RuleComponent Class</seealso>
	/// <seealso cref="IWorkValidation">IWorkValidation Interface</seealso>
	/// <seealso cref="WorkValidation">WorkValidation Class</seealso>
	/// <seealso cref="IWorkAction">IWorkAction Interface</seealso>
	/// <seealso cref="IWorkResult">IWorkResult Interface</seealso>
	/// <seealso cref="WorkResult">WorkResult Class</seealso>
	public abstract class WorkAction
	{
		/// <summary>
		/// Describes the stages of the <see cref="WorkAction"/> processing. These are used to enforce certain constraints in the processing pipeline and allow
		/// implementors to know if they can do something safely such as adding a rule to the rules collection.
		/// </summary>
		public enum WorkActionProcessingStage
		{
			/// <summary>Runs just before the <see cref="AddRules"/> method is invoked, essentially the beginning of the WorkAction processing.</summary>
			/// <remarks>This method should have any logic that needs to run in order to add/process the business rules.</remarks>
			PreAddRules,
			/// <summary>The Event Hndler that where rules are added to the Rrules collection.</summary>
			AddRules,
			/// <summary>The Event Hndler that runs after <see cref="PreAddRules"/> just before the <see cref="AddRules"/> is invoked.</summary>
			PreValidateRules,
			/// <summary>
			/// The event handler that actually validates all of the rules is ready to execute. Each rule will have it's Execute method called to validate
			/// whether or not the rule is satisfied. If it's not a failure will occur and processing will either continue through the other rules or it will
			/// halt at the first failed rule depending on the 
			/// </summary>
			/// <remarks>
			/// There isn't a protected method named "ValidateRules". The code is actually called from the Execute method when the <see cref="WorkValidation"/>
			/// object's <see cref="WorkValidation.ValidateRules"/> method is called.
			/// </remarks>
			ValidateRules,
			/// <summary>The Event Hndler that runs after <see cref="ValidateRules"/> just before the <see cref="PreProcessAction"/> is invoked.</summary>
			/// <remarks>This is the first method that is run after the rules have all been validated and passed. If any of the rules fail this method is never invoked.</remarks>
			PreProcessAction,
			/// <summary>The Event Hndler that runs after <see cref="PreProcessAction"/> just before the <see cref="ProcessAction"/> is invoked.</summary>
			ProcessAction,
			/// <summary>The Event Hndler that runs after <see cref="ProcessAction"/> just before the <see cref="ProcessAction"/> is invoked.</summary>
			PostProcessAction,
			/// <summary>This handler is only executed if the code has trapped a <see cref="UnitOfWorkException"/>.</summary>
			ExceptionHandler
		}

		#region Constructors

		/// <summary>
		/// Instantializes a new instance of the <see cref="WorkAction" /> class. Allows for object initializer usage.
		/// </summary>
		protected WorkAction()
		{
			Result = WorkActionResult.Unknown;
			WorkValidationContext = new WorkValidation();
			Configuration = new WorkActionConfiguration();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WorkAction" /> class, using the provided <see cref="IWorkValidation" /> to set the
		/// WorkValidationContext property.
		/// </summary>
		/// <remarks>
		/// Use this constructor to initialize a new instance of the <see cref="WorkAction" /> class when chaining multiple actions in a single
		/// service call. This allows all business rules to be executed in the correct order before any work is done in an action. It also allows
		/// a transaction to be utilized in the logic flow as the <see cref="TransactionScope" /> object is part of the <see cref="WorkValidation" />
		/// class.
		/// </remarks>
		/// <param name="workValidationContext"></param>
		protected WorkAction(IWorkValidation workValidationContext)
		{
			Result = WorkActionResult.Unknown;
			WorkValidationContext = workValidationContext;
			Configuration = new WorkActionConfiguration();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WorkAction" /> class, using the provided <see cref="WorkActionConfiguration" /> object to
		/// provide more fine tuned control over the <see cref="WorkAction" /> and to support performing all work inside of a
		/// <see cref="TransactionScope" /> when spanning multiple <see cref="WorkAction" />s when needed.
		/// </summary>
		/// <param name="config">An <see cref="WorkActionConfiguration" /> object containing the configuration settings desired.</param>
		protected WorkAction(WorkActionConfiguration config)
		{
			Result = WorkActionResult.Unknown;
			Configuration = config;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WorkAction" /> class, using the provided <see cref="WorkActionConfiguration" /> object to
		/// provide more fine tuned control over the <see cref="WorkAction" /> and to support performing all work inside of a
		/// <see cref="TransactionScope" /> when spanning multiple <see cref="WorkAction" />s when needed, and also the provided
		/// <see cref="IWorkValidation" /> to set the WorkValidationContext property for chaining actions.
		/// </summary>
		/// <param name="workValidationContext"></param>
		/// <param name="config"></param>
		protected WorkAction(IWorkValidation workValidationContext, WorkActionConfiguration config)
		{
			Result = WorkActionResult.Unknown;
			WorkValidationContext = workValidationContext;
			Configuration = config;
		}

		#endregion

		#region Properties

		/// <summary>
		/// An <see cref="IWorkValidation" /> used to validate the business rules before the action code is executed and report the results.
		/// </summary>
		public IWorkValidation WorkValidationContext { get; private set; }
		/// <summary>
		/// A <see cref="WorkActionResult"/> Enum value indicating success or failure of the action.
		/// </summary>
		public WorkActionResult Result { get; protected set; }
		/// <summary>
		/// A <see cref="WorkActionConfiguration" /> object containing various settings for the action's behavior.
		/// </summary>
		public WorkActionConfiguration Configuration { get; set; }
		/// <summary>
		/// A <see cref="WorkActionProcessingStage"/> enum representing the current stage of process used to limit some functionality to only
		/// certain stages. For example, the last chance to stop processing a WorkAction before work is done is the <see cref="ProcessAction"/> method.
		/// </summary>
		public WorkActionProcessingStage ProcessingStage { get; private set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Executes the <see cref="WorkAction" />.  This is central to the Command pattern as this is the Execute part (the part that is publicly 
		/// available to actually start the processing of the "command").
		/// </summary>
		/// <remarks>
		/// This is the method exposed by the <see cref="IWorkAction" /> interface (command pattern implementation). The processing of a class that
		/// implements the <see cref="IWorkAction" /> interface, and that inherits from the <see cref="WorkAction"/> abstract class, begins when the
		/// calling code executes this nethod. This causes the inherited protected methods in the implementing WorkAction class to be executed in the
		/// following sequence: 
		/// The code that gets called behind the scenes is the ProcessAction protected method.
		/// This is the method exposed by the <see cref="IWorkAction" /> interface. Processing starts by calling the <see cref="AddRules(IList&lt;IWorkRule&gt;)"/>
		/// method implemented in the classes that inherit from <see cref="WorkAction" /> to get all
		/// of the business rules (<see cref="WorkRule" /> objects), execute (Validate) them, then execute all of the protected event methods <see cref="PreProcessAction()" />,
		/// <see cref="ProcessAction()" /> (which runs the logic to perform the action) and the <see cref="PostProcessAction()" /> to perform all of
		/// the work of the business action.  Finally <see cref="VerifyAction()" /> is called to set the <see cref="Result" /> property value.
		/// </remarks>
		public void Execute()
		{
			try
			{
				ProcessingStage = WorkActionProcessingStage.PreAddRules;

				// Run any code needed before adding the rules to the collection.
				PreAddRules();

				ProcessingStage = WorkActionProcessingStage.AddRules;

				// Add all of the business rules to the WorkValidation.Rules collection so that we can check them before performing the action
				AddRules(WorkValidationContext.Rules);

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

				// Verify that the action succeeded by setting the Result property to a WorkActionResult value
				Result = VerifyAction();
			}
			catch (UnitOfWorkException exc)
			{
				ProcessingStage = WorkActionProcessingStage.ExceptionHandler;

				//Something went wrong somewhere between the business logic and the repository layer so report it in the results
				//If the current configuration says to just add the exception messages to the results then go ahead
				if (Configuration.UseExceptionMessageDuringExceptionHandling)
				{
					AddThrownExceptionRuleToResults(exc);
					return;
				}

				//The exception is marked as having a Message that is potentially not appropriate for display so add a generic message
				var exceptionRule = new ThrownExceptionRule("HandledExceptionRule", "An exception occurred while processing request.", exc);
				exceptionRule.Verify();

				WorkValidationContext.Results.Add(new WorkResult(exceptionRule));
			}
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// This is the method to override if there is any code that needs to be executed before the rules are added to the collection. An example would be if data
		/// needed to be gathered or prepared in oroder to properly check the rules.
		/// </summary>
		protected virtual void PreAddRules()
		{
		}

		/// <summary>
		/// This is the method to override to add all of the business rules to the process. All business rules will be added to the 
		/// <see cref="IWorkValidation.Rules" /> property so that they can be executed when the action is run to validate that the action
		/// can be performed.
		/// </summary>
		protected virtual void AddRules(IList<IWorkRule> rules)
		{
		}

		/// <summary>
		/// This is the method to override if there is more work to be done to complete the rules check or to prepare data, etc. before the WorkAction's work
		/// can be completed.
		/// </summary>
		protected virtual void PreValidateRules()
		{

		}

		/// <summary>
		/// Override this method to add any pre-processing actions you need to run before the actual action code is executed. Trace and logging
		/// code would be added here for example.
		/// </summary>
		protected virtual void PreProcessAction()
		{
		}

		/// <summary>
		/// This is what needs to be overridden in derived classes in order to perform the action's tasks.  This is the actual work done.
		/// </summary>
		protected virtual void ProcessAction()
		{
		}

		/// <summary>
		/// Override this method to add any post-processing actions you need to run after the actual action code is executed. Trace and logging
		/// code would be added here for example. Also, if any UnitOfWorkException exceptions were handled you could access those here and do processing
		/// on them. The <see cref="WorkActionConfiguration" /> allows you to select automatic exception handling, which essentially adds the exception
		/// Message property to a 
		/// </summary>
		protected virtual void PostProcessAction()
		{
		}

		/// <summary>
		/// This is the code to override when you need the action to return a success/fail value. It checks that the work of the action was
		/// actually completed successfully once the work is performed.
		/// </summary>
		/// <returns></returns>
		protected virtual WorkActionResult VerifyAction()
		{
			return Result;
		}

		/// <summary>
		/// Used to add a <see cref="UnitOfWorkException"/> exception to the failed rules and fail the work action.
		/// </summary>
		/// <remarks>
		/// When this method is called in one of the protected event handlers up to and including the <see cref="ProcessAction"/> method
		/// the exception information is added as a failed rule to the <see cref="WorkValidation.Rules"/> list, which in turn can then easily
		/// be accessed from the <see cref="WorkValidation.FailedResults"/> property, and the <see cref="WorkValidation.IsValid"/> property
		/// will correctly return a false value.
		/// 
		/// When this method is used to add a <see cref="UnitOfWorkException"/> exception to the failed rules and fail the work action once the
		/// <see cref="ProcessAction"/> method has successfully executed, or in scenarios where some of the <see cref="ProcessAction"/> code
		/// has already affected records such as those in a data store, it is the responsibility of the calling code to handle rolling back
		/// any data changes via use of a <see cref="TransactionScope"/> object or via some other mechanism to avoid data corruption.
		/// </remarks>
		/// <param name="exc"></param>
		protected virtual void AddThrownExceptionRuleToResults(UnitOfWorkException exc)
		{
			//Something went wrong somewhere between the business logic and the repository layer so report it in the results
			string message = string.Format("An Exception has been handled by the business rules. Message: {0}", exc.Message);
			Exception innerExc = exc.InnerException;
			while (innerExc != null)
			{
				message += string.Format("\nInner Exception: {0}", innerExc);
				innerExc = innerExc.InnerException;
			}
			var exceptionRule = new ThrownExceptionRule("HandledExceptionRule", message, exc);
			exceptionRule.Verify();

			WorkValidationContext.Results.Add(new WorkResult(exceptionRule));
		}

		#endregion
	}
}