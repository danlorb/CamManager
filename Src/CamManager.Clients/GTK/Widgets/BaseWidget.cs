using System;

namespace xCom.CamManager
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class BaseWidget : Gtk.Bin
	{
		private ICam _cam;
		private CamSettings _settings;

		public BaseWidget()
		{
			// Insert initialization code here.
		}

		public CamSettings SettingsObj {
			get
			{ 
				return _settings;
			}
			set
			{
				_settings = value;
				if(value != null)
					OnSettingsObjChanged(value);
			}
		}

		protected ICam Cam {
			get
			{
				return _cam; 
			}
		}

		protected bool InitCam()
		{
			if(SettingsObj != null && _cam == null)
			{
				try
				{
					_cam = SettingsObj.Connect();
				}
				catch(Exception ex)
				{
					MessageBoxHelper.ShowError(ex);
				}
			}

			return _cam != null;
		}

		protected void DestroyCam()
		{
			_cam = null;
		}

		protected virtual void OnSettingsObjChanged(CamSettings settings)
		{
			
		}
	}
}

