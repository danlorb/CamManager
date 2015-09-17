//
//  Program.cs
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

namespace xCom.CamManager
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Application.Init();
//			AppDomain.CurrentDomain.FirstChanceException += (sender, e) =>
//			{
//				MessageBoxHelper.ShowError(new CamException("This is a First Chance Exception", e.Exception));
//			};
			AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
			{
				var tmp = e.ExceptionObject as Exception;
				MessageBoxHelper.ShowError(new CamException("An Unhandled Exception is occured!", tmp));
			};
			var win = new MainWindow();
			win.Show();
			Application.Run();
		}
	}
}




