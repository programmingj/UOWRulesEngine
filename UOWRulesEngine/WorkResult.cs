namespace UOWRulesEngine
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
		/// Initializes a new instance of the <see cref="WorkResult" /> object. Ensures object initializers can be used with this object.
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

		/// <summary>
		/// A string property containing the name of the business rule.
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// A string property containing the message that describes why the business rule failed, or what conditions needed to be met for the rule to pass.
		/// </summary>
		public string Message { get; set; }
		/// <summary>
		/// A bool field indicating whether the business rule passed or failed.
		/// </summary>
		public bool IsValid { get; set; }

		#endregion
	}
}