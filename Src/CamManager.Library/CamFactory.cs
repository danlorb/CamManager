//
//  CamFactory.cs
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

namespace xCom.CamManager
{
	using System;
	using System.Collections.Generic;
	using System.Net;

	public static class CamFactory
	{
		private static List<ICam> _registeredCams;

		public static ICam Connect(IPAddress address, int port, string username, string password)
		{
			return Connect(address, port, username, password, null);
		}

		/// <summary>
		/// Creates an specified IPCam with the given address, port, username and password.
		/// </summary>
		/// <param name="address">The IPAddress of the Cam</param>
		/// <param name="port">The Port of the Cam.</param>
		/// <param name="username">Username for auth.</param>
		/// <param name="password">Password for auth</param>
		/// <param name="errorHandler">The central ErrorHandler</param>
		public static ICam Connect(IPAddress address, int port, string username, string password, Action<Exception> errorHandler)
		{
			ICam cam = null;
			try
			{
				Init(address, port, username, password);
				cam = DetectIPCam();
				if(cam != null && errorHandler != null)
					((AbstractCam)cam).SetErrorHandler(errorHandler);
			}
			catch(Exception ex)
			{
				throw new CamException("An Connection Error occured", ex);
			}

			return cam;
		}

		private static void Init(IPAddress address, int port, string username, string password)
		{
			if(_registeredCams == null)
				_registeredCams = new List<ICam>();

			_registeredCams.Add(new Foscam.FI9821W(address, port, username, password));
		}

		private static ICam DetectIPCam()
		{
			ICam detectedCam = null;

			foreach(var cam in _registeredCams)
			{
				var info = cam.GetDevInfo();
				if(info != null)
				{
					detectedCam = cam;
					break;
				}
			}

			if(detectedCam != null)
				return detectedCam;
			else
				throw new CamException("Cam could not detected.");
		}
	}
}






























