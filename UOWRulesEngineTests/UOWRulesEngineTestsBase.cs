using System;
using System.Collections.Generic;
using UOWRulesEngine;
using UOWRulesEngine.Rules;

namespace UOWRulesEngineTests
{
	/// <summary>
	/// Base class for the UOWRulesEngine test classes. Used to perform setup operations for the test classes
	/// in this test project.
	/// </summary>
	public class UOWRulesEngineTestsBase
	{
		public UOWRulesEngineTestsBase()
		{
		}

		internal IList<IWorkRule> GetRulesList(bool withFailures)
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
