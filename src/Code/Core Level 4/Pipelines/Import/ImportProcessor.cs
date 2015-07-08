using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SIM.Base;
using SIM.Pipelines.Processors;

namespace SIM.Pipelines.Import
{
    public abstract class ImportProcessor : Processor
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

            return IsRequireProcessing((ImportArgs)args);
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
        protected virtual bool IsRequireProcessing([NotNull] ImportArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            return true;
        }

        public sealed override long EvaluateStepsCount(ProcessorArgs args)
        {
            return EvaluateStepsCount((ImportArgs)args);
        }

        protected virtual long EvaluateStepsCount(ImportArgs args)
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

            Process((ImportArgs)args);
        }

        /// <summary>
        /// The process.
        /// </summary>
        /// <param name="args">
        /// The args. 
        /// </param>
        protected abstract void Process([NotNull] ImportArgs args);

        #endregion
    }
}
