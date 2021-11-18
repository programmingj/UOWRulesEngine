using System;
using System.Threading.Tasks;
using UOWRulesEngine.Rules;
using static UOWRulesEngine.Enums;

namespace UOWRulesEngine
{
	/// <summary>
	/// Exposes the <see cref="ExecuteAsync"/> method following the Command pattern to work with
	/// <see cref="WorkActionAsync"/> objects containing the logic for the action.
	/// to be performed.
	/// </summary>
	/// 
	/// <see cref="WorkActionAsync"/>
	/// <seealso cref="IWorkActionAsync"/>
	/// <seealso cref="WorkAction"/>
	/// <see cref="WorkRule"/>
	/// <seealso cref="IWorkRule"/>
	/// <seealso cref="RuleComponent"/>
	/// <see cref="WorkValidation"/>
	/// <seealso cref="IWorkValidation"/>
	/// <see cref="WorkResult"/>
	/// <seealso cref="IWorkResult"/>
	public interface IWorkActionAsync
    {
		/// <inheritdoc cref="WorkActionBase.Result" path="*"/>
		WorkActionResult Result { get; }

		/// <inheritdoc cref="WorkActionBase.ProcessingStage" path="*"/>
		WorkActionProcessingStage ProcessingStage { get; }

		/// <summary>
		/// Executes the <see cref="WorkActionAsync"/> event handlers in order. This is central to the Command
		/// pattern as this is the entry point of the command (the part that is publicly available to actually
		/// start the processing of the "Unit of Work" described by the child <see cref="WorkActionAsync"/>).
		/// </summary>
		/// 
		/// <remarks>
		/// This is the method exposed by the <see cref="IWorkActionAsync" /> interface (command pattern
		/// implementation). The processing of a class that implements the <see cref="IWorkActionAsync" />
		/// interface, and that inherits from the <see cref="WorkActionAsync"/> abstract class, begins when
		/// the calling code executes this nethod. This causes the inherited protected event handlers in the
		/// implementing <see cref="WorkActionAsync"/> class to be executed in the following order:
		/// 
		/// <see cref="WorkActionAsync.PreAddRulesAsync"/> executes first before the
		/// <see cref="WorkActionAsync.AddRulesAsync(System.Collections.Generic.IList{IWorkRule})"/> method is executed,
		/// before any rules have been added or validated. Here implementers can add any pocessing code that needs to
		/// happen before the rules are added such as loading data required by the rules, logging, etc.
		/// 
		/// <see cref="WorkActionAsync.AddRulesAsync(System.Collections.Generic.IList{IWorkRule})"/> executes next and allows
		/// the rules to be added and then validated. Rules are added to the list of <see cref="IWorkRule"/> objects and are
		/// then validated in the order they are added to the list. If the <see cref="WorkActionBase.Configuration"/> property's
		/// <see cref="WorkActionConfiguration.StopRuleProcessingOnFirstFailure"/> property is set to true the validation
		/// process will stop and return to the calling code immediately upon any rule failing. If it's false the
		/// <see cref="WorkActionAsync"/> rule validation will continue until all rules have been processed. If any
		/// <see cref="WorkRule"/> fails validation the <see cref="WorkActionAsync"/> processing stops and control is
		/// returned to the calling code.
		/// 
		/// <see cref="WorkActionAsync.PreProcessActionAsync"/> executes after the
		/// <see cref="WorkActionAsync.AddRulesAsync(System.Collections.Generic.IList{IWorkRule})"/> method has added and
		/// validated all <see cref="WorkRule"/> objects. It allows implementors to check data, do any additional logging
		/// for warnings that may have been generated during rule validation, etc. It runs before the
		/// <see cref="WorkActionAsync.ProcessActionAsync"/>.
		/// 
		/// <see cref="WorkActionAsync.ProcessActionAsync"/> executes next and is where the unit of work is actually processed.
		/// This is the event that actually performs the work represented by the <see cref="WorkActionAsync"/>.
		/// 
		/// <see cref="WorkActionAsync.PostProcessActionAsync"/> executes last and allows implementors to perform additional
		/// logging, handle notifications, etc. It runs after <see cref="WorkActionAsync.ProcessActionAsync"/>.
		/// 
		/// Note:
		/// 
		/// At any time during processing after the <see cref="WorkActionAsync.AddRulesAsync(System.Collections.Generic.IList{IWorkRule})"/>
		/// event has validated the <see cref="WorkRule"/> business rules any event that finds a problem can either throw a
		/// <see cref="UnitOfWorkException"/> which will be trapped by the currently executing
		/// <see cref="ExecuteAsync"/> method and a <see cref="ThrownExceptionRule"/> will be added to the rules as a
		/// failed rule so that it can be reported back the calling code as a failed rule. When this happens the
		/// <see cref="UnitOfWorkException.UseExceptionMessageDuringExceptionHandling"/> property is checked for a value
		/// and if one is present it is checked, otherwise the <see cref="WorkActionConfiguration.UseExceptionMessageDuringExceptionHandling"/>
		/// property is checked and if set to true the error message for the failed rule will be set to the
		/// <see cref="UnitOfWorkException"/>'s <see cref="Exception.Message"/> property. Otherwise the error
		/// message will be set to a default, "user-friendly" value stored in the
		/// <see cref="WorkActionConfiguration.GenericExceptionMessageForThrownExceptionRule"/> property.
		/// </remarks>
		Task ExecuteAsync();
	}
}