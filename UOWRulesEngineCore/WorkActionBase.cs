﻿using System;
using System.Transactions;
using UOWRulesEngineCore.Rules;
using static UOWRulesEngineCore.Enums;

namespace UOWRulesEngineCore
{
	/// <summary>
	/// The base class for the <see cref="WorkAction"/> and <see cref="WorkActionAsync"/> classes.
	/// </summary>
    public abstract class WorkActionBase
    {
        /// <summary>
        /// Default constructor. Instantiates a new <see cref="WorkActionBase"/> object.
        /// </summary>
        public WorkActionBase (IWorkValidation workValidationContext, WorkActionConfiguration configuration)
        {
            WorkValidationContext = workValidationContext;
            Configuration = configuration;
            Result = WorkActionResult.Unknown;
        }

        /// <summary>
        /// A <see cref="WorkValidation"/> object that is used to examine the results of a <see cref="WorkAction.Execute"/> or
        /// <see cref="WorkActionAsync.ExecuteAsync"/> execution.
        /// </summary>
        public IWorkValidation WorkValidationContext { get; private set; }

        /// <summary>
        /// A <see cref="WorkActionConfiguration" /> object containing various settings for the
        /// action's behavior.
        /// </summary>
        public WorkActionConfiguration Configuration { get; set; }

        /// <summary>
        /// A <see cref="WorkActionProcessingStage"/> enum representing the current stage of processing,
        /// used to limit some functionality to only certain stages. For example, the last chance to
        /// stop processing a <see cref="WorkAction"/> before work is done is the
        /// <see cref="WorkAction.ProcessAction"/> method.
        /// </summary>
        public WorkActionProcessingStage ProcessingStage { get; internal set; }

        /// <summary>
        /// A bool property that flags the implemented <see cref="WorkAction"/> or <see cref="WorkActionAsync"/> class as a
        /// child work action.
        /// </summary>
        /// <remarks>
        /// This property affects the way the <see cref="IWorkRule"/> objects are added to the <see cref="WorkValidation.Rules"/>
        /// collection and which collection can be used. For "parent" classes the rules are added to the inheriting class'
        /// <see cref="WorkValidationContext"/> property's <see cref="WorkValidation.Rules"/> collection.
        /// 
        /// For child classes, classes flagged with this property set to true, the rules can't be added to their internal
        /// validation context's rules list. They have to be added to the parent work action's rules collection.
        /// </remarks>
        public bool IsChildAction { get; set; }

        /// <summary>
        /// A <see cref="WorkActionResult"/> Enum value indicating success or failure of the action.
        /// </summary>
        public WorkActionResult Result { get; protected set; }

        /// <summary>
        /// Used to add a <see cref="UnitOfWorkException"/> exception to the failed rules and fail the work action.
        /// </summary>
        /// <remarks>
        /// When this method is called in one of the protected event handlers up to and including the <see cref="WorkAction.ProcessAction"/> method
        /// the exception information is added as a failed rule to the <see cref="WorkValidation.Rules"/> list, which in turn can then easily
        /// be accessed from the <see cref="WorkValidation.FailedResults"/> property, and the <see cref="WorkValidation.IsValid"/> property
        /// will correctly return a false value.
        /// 
        /// When this method is used to add a <see cref="UnitOfWorkException"/> exception to the failed rules and fail the work action once the
        /// <see cref="WorkAction.ProcessAction"/> method has successfully executed, or in scenarios where some of the <see cref="WorkAction.ProcessAction"/>
        /// code has already affected records such as those in a data store, it is the responsibility of the calling code to handle rolling back
        /// any data changes via use of a <see cref="TransactionScope"/> object or via some other mechanism to avoid data corruption.
        /// </remarks>
        /// <param name="exc"></param>
        protected virtual void AddThrownExceptionRuleToResults(UnitOfWorkException exc)
		{
			// Something went wrong somewhere between the business logic and the repository layer so report it in the results
			string message = string.Format("An Exception has been handled by the business rules. Message: {0}", exc.Message);
			Exception? innerExc = exc.InnerException;
			while (innerExc != null)
			{
				message += string.Format("\nInner Exception: {0}", innerExc);
				innerExc = innerExc.InnerException;
			}
			var exceptionRule = new ThrownExceptionRule("HandledExceptionRule", message, exc);
			exceptionRule.Verify();

			WorkValidationContext.Results.Add(new WorkResult(exceptionRule));
		}
	}
}
