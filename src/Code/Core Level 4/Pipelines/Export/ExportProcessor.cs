#region Usings

using SIM.Base;
using SIM.Pipelines.Processors;

#endregion

namespace SIM.Pipelines.Export
{
	/// <summary>
	///   The backup processor.
	/// </summary>
	public abstract class ExportProcessor : Processor
	{
		#region Methods

		/// <summary>
		/// The is require processing.
		/// </summary>
		/// <param name="args">
		/// The args. 
		/// </param>
		/// <returns>
		/// The is require processing. 
		/// </returns>
		public override bool IsRequireProcessing(ProcessorArgs args)
		{
			Assert.ArgumentNotNull(args, "args");

			return IsRequireProcessing((ExportArgs)args);
		}

		/// <summary>
		/// The is require processing.
		/// </summary>
		/// <param name="args">
		/// The args. 
		/// </param>
		/// <returns>
		/// The is require processing. 
		/// </returns>
		protected virtual bool IsRequireProcessing([NotNull] ExportArgs args)
		{
			Assert.ArgumentNotNull(args, "args");

			return true;
		}

		public sealed override long EvaluateStepsCount(ProcessorArgs args)
		{
			return EvaluateStepsCount((ExportArgs)args);
		}

		protected virtual long EvaluateStepsCount(ExportArgs args)
		{
			return 1;
		}

		/// <summary>
		/// The process.
		/// </summary>
		/// <param name="args">
		/// The args. 
		/// </param>
		protected override void Process(ProcessorArgs args)
		{
			Assert.ArgumentNotNull(args, "args");

			Process((ExportArgs)args);
		}

		/// <summary>
		/// The process.
		/// </summary>
		/// <param name="args">
		/// The args. 
		/// </param>
		protected abstract void Process([NotNull] ExportArgs args);

		#endregion
	}
}
