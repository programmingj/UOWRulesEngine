using System;

namespace UOWRulesEngineCore
{
	/// <summary>
	/// Contains the properties needed to report failures in business rules for a <see cref="WorkAction" />.
	/// </summary>
	/// <seealso cref="IWorkRule">IWorkRule Interface</seealso>
	/// <seealso cref="WorkRule">WorkRule Class</seealso>
	/// <seealso cref="RuleComponent">RuleComponent Class</seealso>
	/// <seealso cref="IWorkValidation">IWorkValidation Interface</seealso>
	/// <seealso cref="WorkValidation">WorkValidation Class</seealso>
	/// <seealso cref="IWorkAction">IWorkAction Interface</seealso>
	/// <seealso cref="WorkAction">WorkAction Class</seealso>
	/// <seealso cref="IWorkResult">IWorkResult Interface</seealso>
	/// <remarks>
	/// Before a <see cref="WorkAction" /> can be executed all of the business rules stored in the <see cref="WorkValidation.Rules" /> property have
	/// to be executed to ensure that the <see cref="WorkAction" /> can be run successfully with all of the business rules followed properly. As each of
	/// the object's <see cref="IWorkRule.Execute()" /> methods are executed the results of each of the rules is stored in a WorkResult object for 
	/// inspection later, ensuring that the business rules have been satisfied. If any rule fails the returned WorkResult will have it's IsValid
	/// property set to false.
	/// </remarks>
	public class WorkResult : IWorkResult
	{
		#region Constructors

		/// <summary>
		/// Default constructor. Initializes a new instance of the <see cref="WorkResult"/> class.
		/// Empty constructor that ensures object initializers can be used with this object.
		/// </summary>
		public WorkResult()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WorkResult" /> class using the provided <see cref="WorkRule" /> properties to set it's Name, 
		/// Message and IsValid properties.
		/// </summary>
		public WorkResult(RuleComponent rule)
		{
			Name = rule.Name;
			Message = rule.Message;
			IsValid = rule.IsValid;
		}

		#endregion

		#region Properties

		/// <inheritdoc cref="IWorkResult.Name" path="*"/>
		public string? Name { get; set; }

		/// <inheritdoc cref="IWorkResult.Message" path="*"/>
		public string? Message { get; set; }

		/// <inheritdoc cref="IWorkResult.IsValid" path="*"/>
		public bool IsValid { get; set; }

		/// <inheritdoc cref="IWorkResult.IsWarning" path="*"/>
		public bool IsWarning { get; set; }

		/// <inheritdoc cref="IWorkResult.WarningMessage" path="*"/>
		public string? WarningMessage { get; set; }

		#endregion

		#region Fluent Methods.

		/// <inheritdoc cref="IWorkResult.SetName(string)" path="*"/>
		public WorkResult SetName (string value)
		{
			if (string.IsNullOrEmpty(value.Trim()))
			{
				throw new ArgumentNullException("value");
			}

			Name = value;
			return this;
		}

		/// <inheritdoc cref="IWorkResult.SetMessage(string)" path="*"/>
		public WorkResult SetMessage (string value)
		{
			if (string.IsNullOrEmpty(value.Trim()))
			{
				throw new ArgumentNullException("value");
			}

			Message = value;
			return this;
		}

		/// <inheritdoc cref="IWorkResult.SetIsValid(bool)" path="*"/>
		public WorkResult SetIsValid (bool value)
		{
			IsValid = value;
			return this;
		}

		#endregion
	}
}