using UOWRulesEngineCore.Rules;

namespace UOWRulesEngineCore
{
	/// <summary>
	/// Contains global enum values needed by the system.
	/// </summary>
	public static class Enums
	{
		/// <summary>
		/// The results of an <see cref="IWorkAction" /> execution.
		/// </summary>
		public enum WorkActionResult
		{
			/// <summary>The <see cref="WorkAction"/> completed it's task successfully.</summary>
			Success,
			/// <summary>
			/// The <see cref="WorkAction"/> either has failing rule(s) or a <see cref="UnitOfWorkException"/> was thrown
			/// and a failed rule was added to the Rules collection with the exception information.
			/// </summary>
			Fail,
			/// <summary>The <see cref="WorkAction"/> either has not been executed yet or is currently running.</summary>
			Unknown
		}

		/// <summary>
		/// Describes the stages of the <see cref="WorkAction"/> and <see cref="WorkActionAsync"/> processing. These are
		/// used to enforce certain constraints in the processing pipeline and allow implementors to know if they can do
		/// something safely such as adding a rule to the rules collection and whether or not a <see cref="UnitOfWorkException"/>.
		/// needs to be thrown in order for an out of process failed rule (<see cref="ThrownExceptionRule"/>) to be added
		/// to the rules list so that error can be reported back to the calling code.
		/// </summary>
		public enum WorkActionProcessingStage
		{
			/// <summary>Runs just before the <see cref="AddRules"/> method is invoked, essentially the
			/// beginning of the WorkAction processing.
			/// </summary>
			/// <remarks>
			/// This method should have any logic that needs to run in order to add/process the business rules.
			/// </remarks>
			PreAddRules,
			/// <summary>The Event Hndler that where rules are added to the Rrules collection.</summary>
			AddRules,
			/// <summary>The Event Hndler that runs after <see cref="PreAddRules"/> just before the
			/// <see cref="AddRules"/> is invoked.
			/// </summary>
			PreValidateRules,
			/// <summary>
			/// The event handler that actually validates all of the rules is ready to execute. Each rule
			/// will have it's Execute method called to validate whether or not the rule is satisfied. If
			/// it's not a failure will occur and processing will either continue through the other rules
			/// or it will halt at the first failed rule depending on the value of the
			/// <see cref="WorkActionConfiguration.StopRuleProcessingOnFirstFailure"/> property.
			/// 
			/// ============================================================================================
			/// TODO: Implement the logic to halt processing after the first failed rule!
			/// ============================================================================================
			/// </summary>
			/// <remarks>
			/// There isn't a protected method named "ValidateRules". The code is actually called from
			/// the Execute method when the <see cref="WorkValidation"/> object's
			/// <see cref="WorkValidation.ValidateRules"/> method is called.
			/// </remarks>
			ValidateRules,
			/// <summary>The event handler that runs after <see cref="ValidateRules"/> just before the
			/// <see cref="PreProcessAction"/> is invoked.
			/// </summary>
			/// <remarks>
			/// This is the first method that is run after the rules have all been validated and passed.
			/// If any of the rules fail this method is never invoked and handles any tasks that need to
			/// be performed before the <see cref="ProcessAction"/> code can be executed.
			/// 
			/// This is the first place in the processing where tasks that would need to be rolled back in
			/// the case of a failure should be placed. The rules have passed at this point making it a
			/// logical starting point for performing updates/inserts to any data.
			/// </remarks>
			PreProcessAction,
			/// <summary>
			/// The event handler that runs after <see cref="PreProcessAction"/>. This is where the actual
			/// work is performed, the whole point (subject) of the Unit of Work.
			/// </summary>
			ProcessAction,
			/// <summary>
			/// The Event Handler that runs after the <see cref="ProcessAction"/> method for any
			/// post-processing clean up would need to execute. An example would be committing any
			/// active transaction.
			/// </summary>
			PostProcessAction,
			/// <summary>
			/// This handler is only executed if the code has trapped a <see cref="UnitOfWorkException"/>.
			/// Any other type of exceptions are NOT handled by the underlying <see cref="WorkAction"/> and
			/// <see cref="WorkActionAsync"/> base classes.
			/// </summary>
			ExceptionHandler
		}
	}
}
