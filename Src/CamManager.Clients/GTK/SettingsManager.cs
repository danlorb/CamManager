using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Configuration;

namespace xCom.CamManager
{
	internal static class SettingsManager
	{
		private static Dictionary<string, string> _settings;

		internal static string LoadSetting(string key)
		{
			LoadSettingsFile();

			var result = string.Empty;
			var tmp = string.Empty;

			if(_settings != null && _settings.TryGetValue(key, out result))
				return result;			

			throw new CamException("Key could not found.", new KeyNotFoundException());
		}

		internal static void SaveSetting(string key, string value)
		{
			LoadSettingsFile();

			var tmp = string.Empty;
			if(!_settings.TryGetValue(key, out tmp))
				_settings.Add(key, value);
			else
				_settings[key] = value;
		}

		private static void LoadSettingsFile()
		{
			if(_settings != null)
				return;
			else {
				_settings = new Dictionary<string, string>();

				// Load Default Settings
				var lang = ConfigurationManager.AppSettings["lang"];
				var mplayer = ConfigurationManager.AppSettings["mplayer"];

				SaveSetting("lang", lang);
				SaveSetting("mplayer", mplayer);
			}
			
			var xDoc = XDocument.Load(CreateFile());
			xDoc.Element("settings").Elements("setting").ToList().ForEach(x => {
				var key = x.Attribute("key").Value;
				var value = x.Attribute("value").Value;
				var temp = string.Empty;

				if(!_settings.TryGetValue(key, out temp))
					_settings.Add(key, x.Value);
			});
				
		}

		internal static void SaveSettingsFile()
		{
			var file = CreateFile(false);
			if(File.Exists(file))
				File.Delete(file);

			XDocument xDoc = new XDocument();
			var xRoot = new XElement("settings");
			xDoc.Add(xRoot);

			foreach(var kvp in _settings) {
				xRoot.Add(new XElement("setting",
					new XAttribute("key", kvp.Key), 
					new XAttribute("value", kvp.Value)));
			}

			xDoc.Save(file);
		}

		private static string CreateFile(bool createFilePhysical = true)
		{
			var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			var appPath = System.IO.Path.Combine(path, "FOSCAM");
			if(!Directory.Exists(appPath))
				Directory.CreateDirectory(appPath);

			var file = Path.Combine(appPath, "settings.xml");
			if(createFilePhysical && !File.Exists(file)) {
				XDocument xdoc = new XDocument();
				xdoc.Add(new XElement("settings"));
				xdoc.Save(file);
			}
			return file;
		}
	}
}

