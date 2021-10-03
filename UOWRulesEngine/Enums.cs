using System;

namespace UOWRulesEngine
{
	/// <summary>
	/// A static class containing any enum values needed by the systm. 
	/// </summary>
	public static class Enums
	{
		/// <summary>
		/// The results of an <see cref="IWorkAction" /> execution.
		/// </summary>
		public enum WorkActionResult
		{
			/// <summary>The <see cref="WorkAction"/> completed it's task successfully.</summary>
			Success,
			/// <summary>
			/// The <see cref="WorkAction"/> either has failing rule(s) or a <see cref="UnitOfWorkException"/> was thrown
			/// and a failed rule was added to the Rules collection with the exception information.
			/// </summary>
			Fail,
			/// <summary>The <see cref="WorkAction"/> either has not been executed yet or is currently running.</summary>
			Unknown
		}
	}
}
