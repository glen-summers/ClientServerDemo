using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.CSharp;


namespace BuildTasks
{
	public class AssemblyBuilder : Task
	{
		[Required]
		public string OutputAssembly { get; set; }

		[Required]
		public string MainAssembly { get; set; }

		[Required]
		public string EntryPoint { get; set; }

		public string KeyFile { get; set; }

		public string Target { get; set; }

		public string Icon { get; set; }

		public bool Compress { get; set; }

		public bool Pdbs { get; set; }

		public bool XmlSerialisers { get; set; }

		public bool Trace { get; set; }

		public bool Args { get; set; }

		[Required]
		public string TempDir { get; set; }

		public bool KeepFiles { get; set; }

		[Required]
		public ITaskItem[] Assemblies { get; set; }

		public string CompilerVersion { get; set; }

		public string Platform { get; set; }

		private const string Usings =
			@"
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;";

		private const string AssemblyAttributes =
			@"
[assembly: AssemblyTitle(""{0}"")]
[assembly: AssemblyDescription(""{1}"")]
[assembly: AssemblyCompany(""{2}"")]
[assembly: AssemblyProduct(""{3}"")]
[assembly: AssemblyCopyright(""{4}"")]
[assembly: AssemblyVersion(""{5}"")]
[assembly: AssemblyFileVersion(""{6}"")]";

		private const string Program =
			@"
public class Program
{
	private static readonly Dictionary<string, Assembly> assemblies;

	static Program()
	{
		Trace.WriteLine(""Init"");
		assemblies = new Dictionary<string, Assembly>(StringComparer.OrdinalIgnoreCase);
		AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
		{
			try
			{
				string resourceName = new AssemblyName(args.Name).Name;
				Assembly assembly;
				if (assemblies.TryGetValue(resourceName, out assembly))
				{
					Trace.WriteLine(""Cache hit : "" + args.Name);
					return assembly;
				}

				Trace.WriteLine(""Loading : "" + args.Name);
				Assembly resourceAssembly = Assembly.GetExecutingAssembly();
				Stream stream = resourceAssembly.GetManifestResourceStream(resourceName + "".dll"");
				if (stream == null)
				{
					stream = resourceAssembly.GetManifestResourceStream(resourceName + "".exe"");
				}
				if (stream == null)
				{
					Trace.WriteLine(""Unresolved : "" + args.Name);
					return null;
				}
				using (stream)
				using (Stream pdbStream = resourceAssembly.GetManifestResourceStream(resourceName + "".pdb""))
				{
					assembly = Assembly.Load(ReadAll(stream), pdbStream == null ? null : ReadAll(pdbStream));
				}

				assemblies.Add(resourceName, assembly);
				return assembly;
			}
			catch (Exception e)
			{
				Trace.WriteLine(e.ToString());
				throw;
			}
		};
	}";

		private const string ReadAllCompressed =
			@"
	private static byte[] ReadAll(Stream stream)
	{
		var buffer = new byte[4096];
		using (var memStream = new MemoryStream())
		{
			using (Stream gzipStream = new GZipStream(stream, CompressionMode.Decompress))
			{
				int numRead;
				while ((numRead = gzipStream.Read(buffer, 0, buffer.Length)) != 0)
				{
					memStream.Write(buffer, 0, numRead);
				}
			}
			return memStream.ToArray();
		}
	}";

		private const string ReadAll =
			@"
	private static byte[] ReadAll(Stream stream)
	{
		byte[] data = new Byte[stream.Length];
		stream.Read(data, 0, data.Length);
		return data;
	}";

		private const string MainVoid =
			@"
	[STAThread]
	public static void Main()
	{{
		try
		{{
			Trace.WriteLine(""Main"");
			{0}();
		}}
		catch (Exception e)
		{{
			Trace.WriteLine(e.ToString());
			throw;
		}}
	}}
}}";

		private const string MainArgs = @"
	[STAThread]
	public static void Main(string[] args) 
	{{
		try
		{{
			Trace.WriteLine(""Main"");
			{0}(args);
		}}
		catch (Exception e)
		{{
			Trace.WriteLine(e.ToString());
			throw;
		}}
	}}
}}";

		public override bool Execute()
		{
			bool success = true;

			var options = new Dictionary<string, string>
			{
				{"CompilerVersion", string.IsNullOrEmpty(CompilerVersion) ? "v4.0" : CompilerVersion}
			};

			using (var provider = new CSharpCodeProvider(options))
			{
				var cp = new CompilerParameters
				{
					GenerateExecutable = true,
					OutputAssembly = OutputAssembly,
					IncludeDebugInformation = false,
					GenerateInMemory = false,
					WarningLevel = 3,
					TreatWarningsAsErrors = false,
					CompilerOptions = GetCompilerOptions(),
					TempFiles = new TempFileCollection(TempDir, KeepFiles)
				};

				cp.ReferencedAssemblies.Add("System.dll");
				cp.ReferencedAssemblies.Add(MainAssembly);

				try
				{
					foreach (ITaskItem assembly in Assemblies)
					{
						cp.EmbeddedResources.Add(AddFile(assembly.ItemSpec));

						string pdbFile = Path.ChangeExtension(assembly.ItemSpec, "pdb");
						if (Pdbs && File.Exists(pdbFile))
						{
							cp.EmbeddedResources.Add(AddFile(pdbFile));
						}
						string xmlSerialiserFile = Path.ChangeExtension(assembly.ItemSpec, "XmlSerializers.dll");
						if (XmlSerialisers && File.Exists(xmlSerialiserFile))
						{
							cp.EmbeddedResources.Add(AddFile(xmlSerialiserFile));
						}
					}

					Assembly mainAssembly = Assembly.LoadFrom(MainAssembly);
					var title = (AssemblyTitleAttribute)Attribute.GetCustomAttribute(mainAssembly, typeof(AssemblyTitleAttribute));
					var description =
						(AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(mainAssembly, typeof(AssemblyDescriptionAttribute));
					var company =
						(AssemblyCompanyAttribute)Attribute.GetCustomAttribute(mainAssembly, typeof(AssemblyCompanyAttribute));
					var product =
						(AssemblyProductAttribute)Attribute.GetCustomAttribute(mainAssembly, typeof(AssemblyProductAttribute));
					var copyright =
						(AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(mainAssembly, typeof(AssemblyCopyrightAttribute));
					var fileVersion =
						(AssemblyFileVersionAttribute)Attribute.GetCustomAttribute(mainAssembly, typeof(AssemblyFileVersionAttribute));
					AssemblyName assemblyName = mainAssembly.GetName();

					var builder = new StringBuilder();
					builder.Append(Usings);
					builder.AppendFormat(AssemblyAttributes, title.Title, description.Description, company.Company, product.Product,
						copyright.Copyright, assemblyName.Version,
						fileVersion.Version);
					builder.Append(Program);
					builder.Append(Compress ? ReadAllCompressed : ReadAll);
					builder.AppendFormat(Args ? MainArgs : MainVoid, assemblyName.Name + "." + EntryPoint);

					CompilerResults cr = provider.CompileAssemblyFromSource(cp, builder.ToString());

					if (cr.Errors.HasErrors)
					{
						Log.LogError("Errors building {0}", cr.PathToAssembly);
						foreach (CompilerError ce in cr.Errors)
						{
							Log.LogError("  {0}", ce.ToString());
						}
						success = false;
					}
					else
					{
						Log.LogMessage("Built {0} successfully.", cr.PathToAssembly);
						Log.LogMessage("{0} temporary files created during the compilation.", cp.TempFiles.Count);
					}
				}
				finally
				{
					if (Compress)
					{
						foreach (var file in cp.EmbeddedResources)
						{
							Log.LogMessage("Deleting temp file {0}", file);
							File.Delete(file);
						}
					}
				}
			}

			return success;
		}

		private string GetCompilerOptions()
		{
			var builder = new StringBuilder();

			builder.Append("/optimize");

			if (KeyFile != null)
			{
				builder.AppendFormat(" /KeyFile:{0}", KeyFile);
			}

			if (Target != null)
			{
				builder.AppendFormat(" /target:{0}", Target);
			}

			if (Icon != null)
			{
				builder.AppendFormat(" /win32icon:{0}", Icon);
			}

			if (Platform != null)
			{
				builder.AppendFormat(" /platform:{0}", Platform);
			}

			if (Trace)
			{
				builder.Append(" /d:TRACE");
			}

			return builder.ToString();
		}

		private string AddFile(string inFile)
		{
			if (!Compress)
			{
				return inFile;
			}
			var buffer = new byte[4096];
			string tempFile = Path.Combine(Path.GetTempPath(), Path.GetFileName(inFile));
			using (var readStream = File.Open(inFile, FileMode.Open))
			using (var writeStream = File.Create(tempFile))
			{
				using (var gZipStream = new GZipStream(writeStream, CompressionMode.Compress, true))
				{
					int numRead;
					while ((numRead = readStream.Read(buffer, 0, buffer.Length)) != 0)
					{
						gZipStream.Write(buffer, 0, numRead);
					}
				}
				Log.LogMessage("Added {0} [{1} -> {2}]", inFile, readStream.Length, writeStream.Length);
			}
			return tempFile;
		}
	}
}
