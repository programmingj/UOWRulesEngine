using System;
using System.Threading.Tasks;

namespace UOWRulesEngine
{
	/// <summary>
	/// The abstract base class for business rule classes.
	/// </summary>
	/// <seealso cref="IWorkRule"/>
	/// <seealso cref="WorkRule"/>
	/// <seealso cref="IWorkValidation"/>
	/// <seealso cref="WorkValidation"/>
	/// <seealso cref="IWorkAction"/>
	/// <seealso cref="WorkAction"/>
	/// <seealso cref="IWorkResult"/>
	/// <seealso cref="WorkResult"/>
	/// <remarks>
	/// The <see cref="IWorkRule"/> interface defines the <see cref="Execute()"/> and <see cref="ExecuteAsync"/> methods inplementing the Command pattern
	/// for the business rules. The rest of the functionality for business rules is implemented by inheriting from the <see cref="WorkRule"/> class, which
	/// in turn inherits from the <see cref="RuleComponent" /> class. This implements the <see cref="IWorkRule"/> interface and as such the implementation
	/// for the <see cref="Execute()"/> and <see cref="ExecuteAsync"/> methods are here. When implementing a new business rule by inheriting from the
	/// <see cref="WorkRule"/> class you have to override the <see cref="Verify()"/> method with the business rule logic to decide if the rule is satisfied.
	/// </remarks>
	public abstract class RuleComponent : IWorkRule
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RuleComponent"/> class. This is the base class for the <see cref="WorkRule" /> class and 
		/// allows custom rules to be created by inheriting from <see cref="WorkRule" /> then overriding the <see cref="Verify"/> method to implement
		/// the business rule logic. These rules are then executed by the <see cref="WorkValidation.ValidateRules()"/> method during the
		/// <see cref="WorkAction"/> and <see cref="WorkActionAsync"/> processing pipeline execution before any work defined representing the Unit of
		/// Work can be executed. If the logic in these rules does not pass, the action's processing code is never executed and the failed rules can be
		/// examined by the calling process to determine where the problem occurred.
		/// </summary>
		/// <param name="name">A string containing the name of the business rule. These values must be unique.</param>
		/// <param name="message">A string containing the message that the calling code will display if the business rule fails.</param>
		protected RuleComponent(string name, string message)
		{
			if (string.IsNullOrEmpty(name.Trim()))
			{
				throw (new ArgumentNullException("name"));
			}
			if (string.IsNullOrEmpty(message.Trim()))
			{
				throw (new ArgumentNullException("name"));
			}

			Name = name;
			Message = message;
		}

		#endregion

		#region Properties

		/// <summary>
		/// The Name of the rule. Used by the implementing code when a rule fails. Must be unique.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The rule failure Message that will be used by the implementing code if the rule fails.
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// The flag that determines if the rule has passed verification. Set when the rule is executed ( Verify is executed from Execute() ).
		/// </summary>
		public bool IsValid { get; set; }

		/// <summary>
		/// A flag that determines whether or not this rule has already been processed.
		/// </summary>
		public bool HasBeenProcessed { get; private set; }

		#endregion

		#region Cores

		/// <summary>
		/// Implementation of the Execute method from the interface for the Command pattern.
		/// </summary>
		/// <remarks>
		/// The <see cref="HasBeenProcessed"/> flag is set by this method so that implementers don't have to call
		/// <code><![CDATA[base.Verify()]]>.</code>
		/// </remarks>
		/// <returns>An <see cref="IWorkResult" /> object containing the results of the rule verification.</returns>
		/// <exception cref="UnitOfWorkException">
		/// A <see cref="UnitOfWorkException"/> is thrown if there is an attempt to execute a rule more than one time.
		/// </exception>
		public IWorkResult Execute()
		{
			// Make sure we haven't already processed this rule. If we have throw an Exception.
			if (HasBeenProcessed)
			{
				throw (new UnitOfWorkException(
					"The rule has already been processed but an attempt has been made to validate the rule again."));
			}

			// Just in case the implementer forgets to call the base Verify() method or otherwise set the HasBeenProcessed flag
			// set it for rule just processed.
			HasBeenProcessed = true;

			return Verify();
		}

		/// <summary>
		/// Implementation of the ExecuteAsync method from the interface for the Command pattern.
		/// </summary>
		/// <remarks>
		/// The <see cref="HasBeenProcessed"/> flag is set by this method so that implementers don't have to call
		/// <code><![CDATA[base.VerifyAsync()]]></code> to insure the flag gets set.
		/// </remarks>
		/// <returns>An <see cref="Task{IWorkResult}" /> object containing the results of the rule verification.</returns>
		/// <exception cref="UnitOfWorkException">
		/// A <see cref="UnitOfWorkException"/> is thrown if there is an attempt to execute a rule more than one time.
		/// </exception>
		public async Task<IWorkResult> ExecuteAsync()
		{
			// Make sure we haven't already processed this rule. If we have throw an Exception.
			if (HasBeenProcessed)
			{
				throw (new UnitOfWorkException(
					"The rule has already been processed but an attempt has been made to validate the rule again."));
			}

			// Just in case the implementer forgets to call the base VerifyAsync() method or otherwise set the HasBeenProcessed flag,
			// set it for rule just processed.
			HasBeenProcessed = true;

			return await VerifyAsync();
		}

		/// <summary>
		/// Runs the business rule. 
		/// </summary>
		/// <remarks>
		/// This is central to the Command pattern as this is the method the <see cref="Execute()" /> method, exposed by the
		/// <see cref="IWorkRule" /> interface, calls.
		/// 
		/// Implementer Note: This is the method you need to override to create your rule. It is the actual rule logic.
		/// 
		/// Implementer Note: In order for the <see cref="WorkActionConfiguration"/> class'
		/// <see cref="WorkActionConfiguration.StopRuleProcessingOnFirstFailure"/> property to function as intended implementers
		/// must implement the Circuit Breaker pattern in this method. If it is not implemented here all rules in the
		/// <see cref="WorkValidation.Rules"/> collection will be processed regardless of the
		/// <see cref="WorkActionConfiguration.StopRuleProcessingOnFirstFailure"/> property's value.
		/// </remarks>
		/// <example>
		/// The Verify() method returns an <see cref="IWorkResult" /> object with the results of the rule check. This will determine if the rule has
		/// passed or failed to the calling code when it's time to verify all rules before executing the <see cref="WorkAction" />. If even one of the
		/// results objects is marked as IsValid == false then the <see cref="WorkAction" /> code will not be executed.
		/// <code>
		/// <![CDATA[
		/// public override IWorkResult Verify()
		/// {
		/// 	IsValid = string.IsNullOrEmpty(target) == false && target.Length == 9;
		/// 	return new WorkResult(this);
		/// }
		/// ]]>
		/// </code>
		/// </example>
		public abstract IWorkResult Verify();

		/// <summary>
		/// Runs the business rule. 
		/// </summary>
		/// <remarks>
		/// This is central to the Command pattern as this is the method the <see cref="Execute()" /> method, exposed by the
		/// <see cref="IWorkRule" /> interface, calls.
		/// 
		/// Implementer Note: This is the method you need to override to create your rule. It is the actual rule logic.
		/// 
		/// Implementer Note: In order for the <see cref="WorkActionConfiguration"/> class'
		/// <see cref="WorkActionConfiguration.StopRuleProcessingOnFirstFailure"/> property to function as intended implementers
		/// must implement the Circuit Breaker pattern in this method. If it is not implemented here all rules in the
		/// <see cref="WorkValidation.Rules"/> collection will be processed regardless of the
		/// <see cref="WorkActionConfiguration.StopRuleProcessingOnFirstFailure"/> property's value.
		/// </remarks>
		/// <example>
		/// The Verify() method returns an <see cref="IWorkResult" /> object with the results of the rule check. This will determine if the rule has
		/// passed or failed to the calling code when it's time to verify all rules before executing the <see cref="WorkAction" />. If even one of the
		/// results objects is marked as IsValid == false then the <see cref="WorkAction" /> code will not be executed.
		/// <code>
		/// <![CDATA[
		/// public override async Task<IWorkResult> VerifyAsync()
		/// {
		/// 	IsValid = string.IsNullOrEmpty(target) == false && target.Length == 9;
		/// 	return new WorkResult(this);
		/// }
		/// ]]>
		/// </code>
		/// </example>
		public abstract Task<IWorkResult> VerifyAsync();

		#endregion
	}
}
