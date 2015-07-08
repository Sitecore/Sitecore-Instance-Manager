#region Usings

using System;
using System.IO;
using SIM.Base;

#endregion

namespace SIM.Pipelines.Export
{
	class ExportPostActions : ExportProcessor
	{
		protected override void Process(ExportArgs args)
		{
			var zipName = args.ExportFile;
		  FileSystem.Local.Zip.CreateZip(args.Folder, zipName);
		}
	}
}
