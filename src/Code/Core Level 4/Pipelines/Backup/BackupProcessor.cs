#region Usings

using SIM.Base;
using SIM.Pipelines.Processors;

#endregion

namespace SIM.Pipelines.Backup
{
	public abstract class BackupProcessor : Processor
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

			return IsRequireProcessing((BackupArgs) args);
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
		protected virtual bool IsRequireProcessing([NotNull] BackupArgs args)
		{
			Assert.ArgumentNotNull(args, "args");

			return true;
		}

		public override sealed long EvaluateStepsCount(ProcessorArgs args)
		{
			return EvaluateStepsCount((BackupArgs) args);
		}

		protected virtual long EvaluateStepsCount(BackupArgs args)
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

			Process((BackupArgs) args);
		}

		/// <summary>
		/// The process.
		/// </summary>
		/// <param name="args">
		/// The args. 
		/// </param>
		protected abstract void Process([NotNull] BackupArgs args);

		#endregion
	}
}