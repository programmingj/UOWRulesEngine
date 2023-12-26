namespace UOWRulesEngineCore.Rules
{
    /// <summary>
    /// A generic child class of the <see cref="WorkRule"/> class
    /// that validates an object is not null. The rule passes if the target object is not null.
    /// 
    /// <seealso cref="NullRule"/>
    /// </summary>
    public class NotNullRule : WorkRule
    {
        private readonly object obj;

        /// <summary>
        /// Default constructor. Instantiates a new <see cref="NotNullRule"/> object.
        /// </summary>
        /// <param name="ruleName">
        /// The name of the rule. Stored in the <see cref="RuleName"/> property.
        /// </param>
        /// <param name="message">The message that will be used if the rule fails.</param>
        /// <param name="obj">The target object that will be examined.</param>
        public NotNullRule(string ruleName, string message, object obj)
            : base(ruleName, message)
        {
            RuleName = ruleName;
            this.obj = obj;
        }

        /// <summary>
        /// A string value representing the name of the rule passed into the constructor's
        /// <paramref>rulename</paramref> parameter.
        /// </summary>
        /// <remarks>
        /// The RuleName is mostly used in unit testing to simplify the tests for failed
        /// rules. Without this property testing to ensure a rule has failed during the test
        /// when expected is less reliable as the other methods for checking involve testing
        /// the message for values or the index of the rule in the Rules collection which is
        /// inherently unreliable.
        /// </remarks>
        public string RuleName { get; private set; }

        /// <summary>
        /// Tests that the target object's value is not null during the
        /// <see cref="Enums.WorkActionProcessingStage.ValidateRules"/> processing stage.
        /// </summary>
        /// <returns>An <see cref="IWorkResult"/> value representing pass or fail or the rule.</returns>
        public override IWorkResult Verify()
        {
            IsValid = obj != null;
            return new WorkResult(this);
        }

		/// <summary>
		/// Override of the base class abstract method <see cref="RuleComponent.VerifyAsync"/>.
		/// </summary>
		/// <returns>A <see cref="Task{TResult}">Task{IWorkResult}</see> object.</returns>
		/// <exception cref="NotImplementedException"></exception>
		public override Task<IWorkResult> VerifyAsync()
		{
			throw new NotImplementedException();
		}
	}
}
