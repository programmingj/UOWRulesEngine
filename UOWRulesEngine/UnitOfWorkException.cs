using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UOWRulesEngine
{
	/// <summary>
	/// The UnitOfWorkException class provides a mechanism for implmenting a set of exceptions that will be caught by the business rule layer
	/// while other, unintended, exceptions pass through the <see cref="WorkAction" /> unhandled. This class is abstract and as such can be instantiated.
	/// You must create a custom exception and inherit from this class in order to have the WorkAction catch the exception during processing.
	/// </summary>
	/// <remarks>
	/// When implementing the <see cref="UOWRulesEngine" /> library this exception implementation can be utilized in areas "underneath" the business layer,
	/// such as the Repository layer, so that they will be caught and reported by the WorkAction implementations while allowing other unexpected
	/// exceptions to remain unhandled to be caught further up the command chain by default error handlers and the framework.
	/// 
	/// The WorkAction base class wraps all of it's calls in a try/catch block that only catches UnitOfWorkException exceptions and anything that inherits
	/// UnitOfWorkException. This allows flexibility when dealing with exception handling such that a group of exceptions inheriting from this type are
	/// automatically caught and reported by the WorkAction classes while other exception types are ignored.
	/// 
	/// NOTE: The exception handler in the <see cref="WorkAction" /> class display the UnitOfWorkException.Message in the Results
	/// </remarks>
	public class UnitOfWorkException : Exception
	{
		#region Constructors

		/// <inheritdoc cref="Exception(string)" path=""/>
		public UnitOfWorkException (string message)
			: base (message)
		{
		}

		/// <inheritdoc cref="Exception (string, Exception)" path="*"/>
		public UnitOfWorkException (string message, Exception innerException)
			: base (message, innerException)
		{
		}

		#endregion

		#region Properties

		/// <summary>
		/// Determines if an exception that inherits <see cref="UnitOfWorkException"/> has a Message property that is appropriate for display in
		/// the <see cref="WorkAction"/> and <see cref="WorkActionAsync"/> error handling. If this is set to true then the <see cref="WorkAction"/>
		/// and <see cref="WorkActionAsync"/> error handlers will use the Message property when building the error rule message.
		/// </summary>
		/// <remarks>
		/// This properties value, when not null, will override the <see cref="WorkActionConfiguration.UseExceptionMessageDuringExceptionHandling"/>
		/// property's value. If this property is null the system will default to using the value from the
		/// <see cref="WorkActionConfiguration.UseExceptionMessageDuringExceptionHandling"/> property.
		/// </remarks>
		public bool? UseExceptionMessageDuringExceptionHandling { get; set; }

		#endregion

		#region Fluid Method Definitions

		/// <summary>
		/// Fluid method used to set the <see cref="UseExceptionMessageDuringExceptionHandling"/> property value.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public UnitOfWorkException SetUseExceptionMessageDuringExceptionHandling(bool? value)
        {
			UseExceptionMessageDuringExceptionHandling = value;
			return this;
        }

		#endregion
	}
}
