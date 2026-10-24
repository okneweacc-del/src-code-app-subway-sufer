using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Security;
using System.Text;

namespace System.Diagnostics;

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class ProcessStartInfo
{
	private string arguments = string.Empty;

	private IntPtr error_dialog_parent_handle = (IntPtr)0;

	private string filename = string.Empty;

	private string verb = string.Empty;

	private string working_directory = string.Empty;

	private System.Collections.Specialized.ProcessStringDictionary envVars;

	private bool create_no_window;

	private bool error_dialog;

	private bool redirect_standard_error;

	private bool redirect_standard_input;

	private bool redirect_standard_output;

	private bool use_shell_execute = true;

	private ProcessWindowStyle window_style;

	private Encoding encoding_stderr;

	private Encoding encoding_stdout;

	private string username;

	private string domain;

	private SecureString password;

	private bool load_user_profile;

	private static readonly string[] empty = new string[0];

	[DefaultValue("")]
	[RecommendedAsConfigurable(true)]
	[NotifyParentProperty(true)]
	[MonitoringDescription("Command line agruments for this process.")]
	[TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public string Arguments => arguments;

	[Editor("System.Diagnostics.Design.StringDictionaryEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[MonitoringDescription("Environment variables used for this process.")]
	[DefaultValue(null)]
	[NotifyParentProperty(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public StringDictionary EnvironmentVariables
	{
		get
		{
			if (envVars == null)
			{
				envVars = new System.Collections.Specialized.ProcessStringDictionary();
				foreach (DictionaryEntry environmentVariable in Environment.GetEnvironmentVariables())
				{
					envVars.Add((string)environmentVariable.Key, (string)environmentVariable.Value);
				}
			}
			return envVars;
		}
	}

	internal bool HaveEnvVars => envVars != null;

	[TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[Editor("System.Diagnostics.Design.StartFileNameEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[DefaultValue("")]
	[MonitoringDescription("The name of the resource to start this process.")]
	[RecommendedAsConfigurable(true)]
	[NotifyParentProperty(true)]
	public string FileName => filename;

	[MonitoringDescription("Errors of this process are redirected.")]
	[DefaultValue(false)]
	[NotifyParentProperty(true)]
	public bool RedirectStandardError => redirect_standard_error;

	[NotifyParentProperty(true)]
	[DefaultValue(false)]
	[MonitoringDescription("Standard input of this process is redirected.")]
	public bool RedirectStandardInput => redirect_standard_input;

	[DefaultValue(false)]
	[MonitoringDescription("Standart output of this process is redirected.")]
	[NotifyParentProperty(true)]
	public bool RedirectStandardOutput => redirect_standard_output;

	public Encoding StandardErrorEncoding => encoding_stderr;

	public Encoding StandardOutputEncoding => encoding_stdout;

	[DefaultValue(true)]
	[NotifyParentProperty(true)]
	[MonitoringDescription("Use the shell to start this process.")]
	public bool UseShellExecute => use_shell_execute;

	[RecommendedAsConfigurable(true)]
	[Editor("System.Diagnostics.Design.WorkingDirectoryEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[NotifyParentProperty(true)]
	[TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[DefaultValue("")]
	[MonitoringDescription("The initial directory for this process.")]
	public string WorkingDirectory => working_directory;

	[NotifyParentProperty(true)]
	public bool LoadUserProfile => load_user_profile;

	[NotifyParentProperty(true)]
	public string UserName => username;

	[NotifyParentProperty(true)]
	public string Domain => domain;

	public SecureString Password => password;

	public ProcessStartInfo(string filename)
	{
		this.filename = filename;
	}

	public ProcessStartInfo(string filename, string arguments)
	{
		this.filename = filename;
		this.arguments = arguments;
	}
}
