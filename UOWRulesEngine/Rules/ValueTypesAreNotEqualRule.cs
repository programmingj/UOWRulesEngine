using System;
using System.Collections.Generic;

namespace UOWRulesEngine.Rules
{
    /// <summary>
    /// A generic child class of the <see cref="UOWRulesEngine"/>'s <see cref="WorkRule"/> class
    /// that validates 2 value types are not equal. The rule passes if the two value types are not equal.
    /// 
    /// <seealso cref="ValueTypesAreEqualRule{T}"/>
    /// </summary>
    public class ValueTypesAreNotEqualRule<T> : WorkRule
        where T : struct
    {
        private T value1;
        private T value2;

        /// <summary>
        /// Default constructor. Instantiates a new <see cref="ValueTypesAreNotEqualRule{T}"/> object.
        /// </summary>
        /// <param name="ruleName">
        /// The name of the rule. Stored in the <see cref="RuleName"/> property.
        /// </param>
        /// <param name="message">The message that will be used if the rule fails.</param>
        /// <param name="value1">First valuetype {T} to compare.</param>
        /// <param name="value2">Second valuetype {T} to compare.</param>
        public ValueTypesAreNotEqualRule(string ruleName, string message, T value1, T value2)
            : base(ruleName, message)
        {
            RuleName = ruleName;
            this.value1 = value1;
            this.value2 = value2;
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
        /// Tests that value1's value is not equal to value2's value during the 
        /// <see cref="Enums.WorkActionProcessingStage.ValidateRules"/> processing stage.
        /// </summary>
        /// <returns>An <see cref="IWorkResult"/> value representing pass or fail or the rule.</returns>
        public override IWorkResult Verify()
        {
            IsValid = EqualityComparer<T>.Default.Equals(value1, value2) == false;
            return new WorkResult(this);
        }
    }
}
