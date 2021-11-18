using System.Collections.Generic;
using UOWRulesEngine;
using UOWRulesEngine.Rules;

namespace UOWRulesEngineTests.Setup
{
	/// <summary>
	/// Sets up various data structures needed by the tests to perform their checks.
	/// </summary>
	internal class UOWTestDataSetup
	{
		/// <summary>
		/// Default constuctor. Instantiates a new <see cref="UOWTestDataSetup"/> object.
		/// </summary>
		internal UOWTestDataSetup()
		{
		}

		/// <summary>
		/// Returns an <see cref="IList{T}"/> of <see cref="IWorkRule"/> objects for testing code.
		/// </summary>
		/// <remarks>
		/// This method builds a list of test <see cref="IWorkRule"/> objects to test any code that needs to
		/// work with the list of rules in the application.
		/// 
		/// By passing in the bool value withFailures the returned list can have either all passing IWorkRule
		/// objects or 1 passing rule and the other 2 failed rules to test various features.
		/// </remarks>
		/// <param name="withFailures">
		/// A bool value that determines whether the <see cref="WorkRule"/> objects returned are all passing
		/// (false) rules, or if the last two will be failed rules (true).
		/// </param>
		/// <returns>An <see cref="IList{T}"/> of <see cref="IWorkRule"/> with the test data.</returns>
		internal static IList<IWorkRule> GetRulesList(bool withFailures)
		{
			IList<IWorkRule> rules = new List<IWorkRule>();
			rules.Add(new NullRule(
				"FirstPassingRule", "Target object must be null.", null));

			if (withFailures)
			{
				rules.Add(new NullRule(
					"FirstFailingRule", "Target object must be null.", string.Empty));
				rules.Add(new NullRule(
					"SecondFailingRule", "Target object must be null.", string.Empty));
			}
			else
			{
				rules.Add(new NullRule(
					"SecondPassingRule", "Target object must be null.", null));
				rules.Add(new NullRule(
					"ThirdPassingRule", "Target object must be null.", null));
			}

			return rules;
		}
	}
}
