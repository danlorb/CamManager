//
//  MainWindow.cs
//
//  Author:
//       Roland Breitschaft <roland.breitschaft@x-company.de>
//
//  Copyright (c) 2015 IT Solutions Roland Breitschaft
//
//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//

using System;
using Gtk;
using xCom.CamManager;

public partial class MainWindow: Gtk.Window
{
	private object _syncObject = new object();
	private CamSettings _settingsObj;

	private MPlayerHelper _mplayer;


	public MainWindow() : base(Gtk.WindowType.Toplevel)
	{
		this.Shown += (sender, e) =>
		{
			CultureManager.RegisterWidgets(this, lblConnection, lblNavigation, lblImageSettings, lblVideoSettings);
			CultureManager.RegisterActions(this, FileAction, ExitAction, ViewAction, ExtrasAction, SettingsAction, AboutAction, ShowVideoAction);

			lock(_syncObject)
			{
				CultureManager.ChangeCulture();
			}
		};

		this.KeyPressEvent += MainWindow_KeyPressEvent;
		this.KeyReleaseEvent += MainWindow_KeyReleaseEvent;

		Build();
	}

	[GLib.ConnectBefore]
	void MainWindow_KeyReleaseEvent(object o, KeyReleaseEventArgs args)
	{
		if(ctlNavigation != null)
			ctlNavigation.StopMove();

		var key = args.Event.Key;
		if(_mplayer != null)
		{
			if(key == Gdk.Key.Key_0)
				_mplayer.IncreaseVolume();
			if(key == Gdk.Key.Key_9)
				_mplayer.DecreaseVolume();
			if(key == Gdk.Key.f)
				_mplayer.FullScreen();
		}
	}

	[GLib.ConnectBefore]
	void MainWindow_KeyPressEvent(object o, KeyPressEventArgs args)
	{
		if(ctlNavigation == null)
			return;

		var key = args.Event.Key;

		if(key == Gdk.Key.Down)
		{
			ctlNavigation.OnBtnBottomPressed(this, EventArgs.Empty);
		}
		else if(key == Gdk.Key.Up)
		{
			ctlNavigation.OnBtnTopPressed(this, EventArgs.Empty);
		}
		else if(key == Gdk.Key.Left)
		{
			ctlNavigation.OnBtnLeftPressed(this, EventArgs.Empty);
		}
		else if(key == Gdk.Key.Right)
		{
			ctlNavigation.OnBtnRightPressed(this, EventArgs.Empty);
		}	
	}

	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		SettingsManager.SaveSettingsFile();

		if(_mplayer != null)
			_mplayer.Stop();

		Application.Quit();
		a.RetVal = true;
	}

	protected void OnExitActionActivated(object sender, EventArgs e)
	{
		OnDeleteEvent(sender, new DeleteEventArgs());
	}

	protected void OnSettingsActionActivated(object sender, EventArgs e)
	{
		var dlg = new SettingsDialog();
		dlg.Response += (o, args) =>
		{
			dlg.Destroy();
		};

		dlg.Show();
	}

	protected void OnCtlConnectionConnectionEstablished(object sender, ConnectionEventArgs e)
	{
		expConnection.Expanded = false;
		ctlConnection.Sensitive = false;
		_settingsObj = e.Settings;

		ShowVideoAction.Sensitive = true;

		// Load Navigation
		expNavigation.Expanded = true;
		expNavigation.Sensitive = true;
		ctlNavigation.SettingsObj = e.Settings;

		// Load Image Settings
		expImageSettings.Expanded = false;
		expImageSettings.Sensitive = true;
		ctlImageSettings.SettingsObj = e.Settings;

		// Load Video Settings
		expVideoSettings.Expanded = false;
		expVideoSettings.Sensitive = true;
		ctlVideoSettings.SettingsObj = e.Settings;
		ctlVideoSettings.StreamKindChanged += (sender1, e1) =>
		{
			_settingsObj.IsMainStream = e1.IsMainStream;
			if(_mplayer != null)
				_mplayer.ReStart(_settingsObj);
		};

		// Load Info
		expInfo.Expanded = false;
		expInfo.Sensitive = true;
		ctlInfo.SettingsObj = e.Settings;
	}

	protected void OnShowVideoActionActivated(object sender, EventArgs args)
	{
		if(_mplayer == null)
		{
			_mplayer = new MPlayerHelper(_settingsObj);
			_mplayer.Stopped += (s, e) =>
			{
				_mplayer = null;
				//ShowVideoAction.Active = false;
			};
		}

		if(ShowVideoAction.Active)
			_mplayer.Play();
		else
			_mplayer.Stop();
	}
}




