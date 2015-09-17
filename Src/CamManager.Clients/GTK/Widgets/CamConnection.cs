//
//  CamConnection.cs
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
using System.Net;

namespace xCom.CamManager
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class CamConnection : Gtk.Bin
	{
		public event EventHandler<ConnectionEventArgs> ConnectionEstablished;

		public CamConnection()
		{
			this.Shown += (sender, e) =>
			{
				#if DEBUG

				txtIpDns.Text = "192.168.2.160";
				txtPort.Text = "88";
				txtUserName.Text = "root";
				txtPassword.Text = "TheaterDueWo";

				#else

				txtIpDns.Text = SettingsManager.LoadSetting("ip");
				txtPort.Text = SettingsManager.LoadSetting("port");
				txtUserName.Text = SettingsManager.LoadSetting("user");

				#endif

				CultureManager.RegisterWidgets(this, lblIpDns, lblPassword, lblPort, lblUserName, btnConnect);

			};
			this.Build();
		}

		protected void OnBtnConnectClicked(object sender, EventArgs e)
		{
			var tmp = ConnectionEstablished;
			if(tmp != null && ValidateMask())
			{
				var settings = new CamSettings { UserName = txtUserName.Text, Password = txtPassword.Text, Port = Convert.ToInt32(txtPort.Text), Address = IPAddress.Parse(txtIpDns.Text) };
				SettingsManager.SaveSetting("user", settings.UserName);
				SettingsManager.SaveSetting("ip", settings.Address.ToString());
				SettingsManager.SaveSetting("port", settings.Port.ToString());
				tmp(this, new ConnectionEventArgs(settings));
			}
			else
			{
				MessageBoxHelper.ShowInfo("Please fill out all fields.");
			}
		}

		private bool ValidateMask()
		{
			if(string.IsNullOrEmpty(txtIpDns.Text))
				return false;

			if(string.IsNullOrEmpty(txtPort.Text))
				return false;

			if(string.IsNullOrEmpty(txtUserName.Text))
				return false;

			if(string.IsNullOrEmpty(txtPassword.Text))
				return false;
			
			return true;
		}
	}
}

