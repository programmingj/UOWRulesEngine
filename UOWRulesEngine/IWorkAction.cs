using System;
using System.Collections.Generic;
using UOWRulesEngine.Rules;
using static UOWRulesEngine.Enums;

namespace UOWRulesEngine
{
	/// <summary>
	/// Exposes the <see cref="Execute"/> method following the Command pattern to work with classes that inherit from the
	/// WorkAction abstract class and that contain the logic for the Unit of Work being represented.
	/// </summary>
	/// 
	/// <see cref="WorkAction"/>
	/// <seealso cref="IWorkActionAsync"/>
	/// <seealso cref="WorkActionAsync"/>
	/// <see cref="WorkRule"/>
	/// <seealso cref="IWorkRule"/>
	/// <seealso cref="RuleComponent"/>
	/// <see cref="WorkValidation"/>
	/// <seealso cref="IWorkValidation"/>
	/// <see cref="WorkResult"/>
	/// <seealso cref="IWorkResult"/>
	public interface IWorkAction
	{
		/// <inheritdoc cref="WorkActionBase.Result" path="*"/>
		WorkActionResult Result { get; }

		/// <inheritdoc cref="WorkActionProcessingStage" path="*"/>
		WorkActionProcessingStage ProcessingStage { get; }

		/// <summary>
		/// Executes the <see cref="WorkAction"/> event handlers in order. This is central to the Command
		/// pattern as this is the entry point of the command (the part that is publicly available to actually
		/// start the processing of the "Unit of Work" described by the child <see cref="WorkAction"/>).
		/// </summary>
		/// 
		/// <remarks>
		/// This is the method exposed by the <see cref="IWorkAction"/> interface (command pattern
		/// implementation). The processing of a class that implements the <see cref="IWorkAction"/>
		/// interface, and that inherits from the <see cref="WorkAction"/> abstract class, begins when
		/// the calling code executes this nethod. This causes the inherited <code>protected</code> event handlers in the
		/// class implementing the <see cref="WorkAction"/> class to be executed in the following order:
		/// 
		/// <see cref="WorkAction.PreAddRules"/> executes first before any rules are added or validated by the
		/// <see cref="WorkAction.AddRules(IList{IWorkRule})"/> method is executed, before any rules have been added or
		/// validated. Here implementers can add any pocessing code that needs to happen before the rules are added such
		/// as loading data required by the rules, logging, etc.
		/// 
		/// <see cref="WorkAction.AddRules(IList{IWorkRule})"/> executes next and allows the rules to be added and then validated.
		/// <see cref="WorkRule"/> objects are added to the list of <see cref="IWorkRule"/> objects and are then validated
		/// in the order they are added to the list. If the <see cref="WorkActionBase.Configuration"/> property's
		/// <see cref="WorkActionConfiguration.StopRuleProcessingOnFirstFailure"/> property is set to true the validation
		/// process will stop and return to the calling code immediately upon any rule failing. If it's false the
		/// <see cref="WorkAction"/> rule validation will continue until all rules have been processed. If any
		/// <see cref="WorkRule"/> fails validation the <see cref="WorkAction"/> processing stops and control is
		/// returned to the calling code.
		/// 
		/// <see cref="WorkAction.PreProcessAction"/> executes after the <see cref="WorkAction.AddRules(IList{IWorkRule})"/>
		/// method has added and validated all <see cref="WorkRule"/> objects. It allows implementors to check data, do any 
		/// additional logging for warnings that may have been generated during rule validation, etc.
		/// 
		/// <see cref="WorkAction.ProcessAction"/> executes next and is where the unit of work is actually processed.
		/// This is the event that actually performs the work represented by the <see cref="WorkAction"/>.
		/// 
		/// <see cref="WorkAction.PostProcessAction"/> executes last and allows implementors to perform additional
		/// logging, handle notifications, etc.
		/// 
		/// Note:
		/// 
		/// At any time during processing after the <see cref="WorkAction.AddRules(IList{IWorkRule})"/> event has validated
		/// the list of <see cref="WorkRule"/> business rules any event that finds a problem can either throw a
		/// <see cref="UnitOfWorkException"/> which will be trapped by the currently executing <see cref="Execute"/>
		/// method and a <see cref="ThrownExceptionRule"/> will be added to the rules as a failed rule so that it can be
		/// reported back the calling code as a failed rule.
		/// 
		/// When this happens the <see cref="UnitOfWorkException.UseExceptionMessageDuringExceptionHandling"/> property is
		/// examined and if it has a value it is used, otherwise the 
		/// <see cref="WorkActionConfiguration.UseExceptionMessageDuringExceptionHandling"/> property is examined and if set
		/// to true the error message for the failed rule will be set to the <see cref="UnitOfWorkException"/>'s
		/// <see cref="Exception.Message"/> property. Otherwise the error message will be set to a default, "user-friendly"
		/// value stored in the <see cref="WorkActionConfiguration.GenericExceptionMessageForThrownExceptionRule"/> property.
		/// 
		/// The initial value for the <see cref="WorkActionConfiguration.GenericExceptionMessageForThrownExceptionRule"/> property
		/// is set with the <see cref="Constants.GENERIC_EXCEPTION_MESSAGE"/> value.
		/// </remarks>
		void Execute();
	}
}
