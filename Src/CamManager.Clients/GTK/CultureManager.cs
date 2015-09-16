using System;
using System.Globalization;
using System.Collections.Generic;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace xCom.CamManager
{
	internal static class CultureManager
	{
		private static Dictionary<ResourceManager, List<Gtk.Widget>> _widgets;

		internal static string CurrentCulture()
		{
			return CultureInfo.CurrentUICulture.Name;
		}

		internal static void ChangeCulture(string cultureName)
		{
			var currentCulture = CultureInfo.GetCultureInfo(cultureName);

			CultureInfo.DefaultThreadCurrentCulture = currentCulture;
			CultureInfo.DefaultThreadCurrentUICulture = currentCulture;

			Thread.CurrentThread.CurrentCulture = currentCulture;
			Thread.CurrentThread.CurrentUICulture = currentCulture;

			if(_widgets == null)
				_widgets = new Dictionary<ResourceManager, List<Gtk.Widget>>();

			foreach(var kvp in _widgets) {
				kvp.Value.ForEach(x => Task.Factory.StartNew(() => {
					x.Translate(kvp.Key);
				}));
			}
		}

		internal static void RegisterWidgets(Gtk.Widget source, params Gtk.Widget[] widgets)
		{			
			if(_widgets == null)
				_widgets = new Dictionary<ResourceManager, List<Gtk.Widget>>();

			var sourceType = source.GetType();
			var baseName = string.Format("FOSCAM.Viewer.Resources.{0}", sourceType.Name);
			var rm = new ResourceManager(baseName, sourceType.Assembly);

			_widgets.Add(rm, new List<Gtk.Widget>(widgets));
		}

		private static void Translate(this Gtk.Widget widget, ResourceManager rm)
		{
			var name = widget.Name;

			if(widget is Gtk.Label) {
				var label = widget as Gtk.Label;
				label.Text = ReadValue(rm, string.Format("{0}_Text", name), label.Text);
				label.TooltipText = ReadValue(rm, string.Format("{0}_Tooltip", name), false);
			}
			else if(widget is Gtk.Dialog) {
				var dialog = widget as Gtk.Dialog;
				dialog.Title = ReadValue(rm, string.Format("{0}_Text", name), dialog.Title);
			}
			else if(widget is Gtk.Button) {
				var button = widget as Gtk.Button;
				button.Label = ReadValue(rm, string.Format("{0}_Text", name), button.Label);
				button.TooltipText = ReadValue(rm, string.Format("{0}_Tooltip", name), false);
			}
		}

		private static string ReadValue(ResourceManager rm, string key, string defaultValue)
		{
			return ReadValue(rm, key, true, defaultValue);
		}

		private static string ReadValue(ResourceManager rm, string key, bool writeErrorString)
		{
			return ReadValue(rm, key, writeErrorString, null);
		}

		private static string ReadValue(ResourceManager rm, string key, bool writeErrorString, string defaultValue)
		{
			var result = GetString(rm, key);
			if(string.IsNullOrEmpty(result) && defaultValue != null)
				return defaultValue;
			else if(!string.IsNullOrEmpty(result))
				return result;
			else if(writeErrorString)
				return "Resource could not located";
			else
				return string.Empty;
		}

		private static string GetString(ResourceManager rm, string key)
		{
			if(rm == null)
				return string.Empty;

			var assm = typeof(MainWindow).Assembly;
			var names = assm.GetManifestResourceNames();
			var info = assm.GetManifestResourceInfo("SettingsDialogStrings.resx");

			// FOSCAM.Viewer.Resources.
			//	var rm1 = new ResourceManager("FOSCAM.Viewer.Resources.SettingsDialogStrings", assm);
			var rm1 = new global::System.Resources.ResourceManager("FOSCAM.Viewer.SettingsDialogStrings", assm);



			try {				
				//var ssss = SettingsDialogStrings.lblLanguage_Text;

				// var result = rm1.GetString(key, CultureInfo.CurrentUICulture);
				var rrrr = rm1.GetObject(key, CultureInfo.CurrentUICulture);
				return "";
			} catch(Exception ex) {
				return string.Empty;
			}
		}
	}
}

