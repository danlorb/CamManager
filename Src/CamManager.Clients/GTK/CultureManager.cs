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
		private static Dictionary<ResourceManager, List<Gtk.Action>> _actions;

		internal static string CurrentCulture()
		{
			return CultureInfo.CurrentUICulture.Name;
		}

		internal static void ChangeCulture()
		{
			var lang = SettingsManager.LoadSetting("lang");
			ChangeCulture(lang);
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

			Task.Factory.StartNew(() =>
			{
				foreach(var kvp in _widgets)
				{
					kvp.Value.ForEach(x => x.Translate(kvp.Key));
				}

				foreach(var kvp in _actions)
				{
					kvp.Value.ForEach(x => x.TranslateAction(kvp.Key));
				}
			});
		}

		internal static void UpdateCulture(Gtk.Widget source)
		{
			if(_widgets == null)
				_widgets = new Dictionary<ResourceManager, List<Gtk.Widget>>();	

			var sourceType = source.GetType();
			var baseName = string.Format("xCom.CamManager.Resources.{0}Strings", sourceType.Name);

			foreach(var kvp in _widgets)
			{
				if(string.Compare(kvp.Key.BaseName.ToUpper(), baseName.ToUpper()) == 0)
				{
					kvp.Value.ForEach(x => x.Translate(kvp.Key));
					break;
				}
			}
		}

		internal static void RegisterWidgets(Gtk.Widget source, params Gtk.Widget[] widgets)
		{			
			if(_widgets == null)
				_widgets = new Dictionary<ResourceManager, List<Gtk.Widget>>();

			var sourceType = source.GetType();
			var baseName = string.Format("xCom.CamManager.Resources.{0}Strings", sourceType.Name);
			var rm = new ResourceManager(baseName, sourceType.Assembly);

			if(source is Gtk.Dialog)
			{
				if(widgets == null)
					widgets = new Gtk.Widget[] { };
				
				var widgetsAsList = widgets.ToList();
				widgetsAsList.Add(source);
				widgets = widgetsAsList.ToArray();
			}

			_widgets.Add(rm, new List<Gtk.Widget>(widgets));
		}

		internal static void RegisterActions(Gtk.Widget source, params Gtk.Action[] actions)
		{
			if(_actions == null)
				_actions = new Dictionary<ResourceManager, List<Gtk.Action>>();

			var sourceType = source.GetType();
			var baseName = string.Format("xCom.CamManager.Resources.{0}Strings", sourceType.Name);
			var rm = new ResourceManager(baseName, sourceType.Assembly);	

			_actions.Add(rm, new List<Gtk.Action>(actions));
		}

		private static void Translate(this Gtk.Widget widget, ResourceManager rm)
		{
			var name = widget.Name;

			if(widget is Gtk.Label)
			{
				var label = widget as Gtk.Label;
				label.Text = ReadValue(rm, string.Format("{0}_Text", name), label.Text);
				label.TooltipText = ReadValue(rm, string.Format("{0}_Tooltip", name), false);
			}
			else if(widget is Gtk.Dialog)
			{
				var dialog = widget as Gtk.Dialog;
				dialog.Title = ReadValue(rm, string.Format("{0}_Text", name), dialog.Title);
			}
			else if(widget is Gtk.Button)
			{
				var button = widget as Gtk.Button;
				button.Label = ReadValue(rm, string.Format("{0}_Text", name), button.Label);
				button.TooltipText = ReadValue(rm, string.Format("{0}_Tooltip", name), false);
			}
		}

		private static void TranslateAction(this Gtk.Action action, ResourceManager rm)
		{
			action.Label = ReadValue(rm, string.Format("{0}_Text", action.Name), action.Label);
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
			if(!string.IsNullOrEmpty(result))
				return result;
			else if(string.IsNullOrEmpty(result) && defaultValue != null)
				return defaultValue;
			else if(writeErrorString)
				return "Resource could not located";
			else
				return string.Empty;
		}

		private static string GetString(ResourceManager rm, string key)
		{
			if(rm == null)
				return string.Empty;
		
			try
			{
				return rm.GetString(key, CultureInfo.CurrentUICulture);
			}
			catch
			{
				return string.Empty;
			}
		}
	}
}

