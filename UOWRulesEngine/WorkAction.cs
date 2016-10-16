using System;
using System.Collections.Generic;
using System.Transactions;

namespace UOWRulesEngine
{
	/// <summary>
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
		/// <param name="config">An <see cref="IWorkActionConfiguration" /> object containing the configuration settings desired.</param>
		protected WorkAction(WorkActionConfiguration config)
		{
			Result = WorkActionResult.Unknown;
			Configuration = config;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WorkAction" /> class, using the provided <see cref="IWorkActionConfiguration" /> object to
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

		#endregion

		#region Methods

		/// <summary>
		/// Executes the <see cref="WorkAction" />.  This is central to the Command pattern as this is the Execute part (the part that is publicly 
		/// available to actually start the processing of the "command").
		/// </summary>
		/// <remarks>
		/// This is the method exposed by the <see cref="IWorkAction" /> interface. Processing starts by calling the 
		/// <see cref="AddRules(IList&lt;IWorkRule&gt;)"/> method implemented in the classes that inherit from <see cref="WorkAction" /> to get all
		/// of the business rules (<see cref="WorkRule" /> objects), execute them, then execute the <see cref="PreExecuteAction()" />,
		/// <see cref="ProcessAction()" /> (which runs the logic to perform the action) and the <see cref="PostExecuteAction()" /> to perform all of
		/// the work of the business action.  Finally <see cref="VerifyAction()" /> is called to set the <see cref="Result" /> property value.
		/// </remarks>
		public void Execute()
		{
			try
			{
				//Add all of the business rules to the WorkValidation.Rules collection so that we can check them before performing the action
				AddRules(WorkValidationContext.Rules);

				//Execute all of the rules to make sure we can run the action
				((WorkValidation)WorkValidationContext).ValidateRules();

				//If validation failed return
				if (WorkValidationContext.IsValid == false)
				{
					Result = WorkActionResult.Fail;
					return;
				}

				//Run any pre-processing code
				PreExecuteAction();

				//If the rules checked out then go ahead and execute the action itself
				ProcessAction();

				//Run any post-processing code
				PostExecuteAction();

				//Verify that the action succeeded by setting the Result property to a WorkActionResult value
				Result = VerifyAction();
			}
			catch (UnitOfWorkException exc)
			{
				//Something went wrong somewhere between the business logic and the repository layer so report it in the results
				//If the current configuration says to just add the exception messages to the results then go ahead
				if(Configuration.UseExceptionMessageDuringExceptionHandling)
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

		/// <summary>
		/// This is the method to override to add all of the business rules to the process. All business rules will be added to the 
		/// <see cref="IWorkValidation.Rules" /> property so that they can be executed when the action is run to validate that the action
		/// can be performed.
		/// </summary>
		protected virtual void AddRules(IList<IWorkRule> rules)
		{
		}

		/// <summary>
		/// Override this method to add any pre-processing actions you need to run before the actual action code is executed. Trace and logging
		/// code would be added here for example.
		/// </summary>
		protected virtual void PreExecuteAction()
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
		protected virtual void PostExecuteAction()
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