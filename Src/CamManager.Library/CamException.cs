//
//  CamException.cs
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
	using System.Runtime.Serialization;

	/// <summary>
	/// Cam exception.
	/// </summary>
	[Serializable]
	public class CamException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="xCom.CamManager.CamException"/> class.
		/// </summary>
		public CamException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="xCom.CamManager.CamException"/> class.
		/// </summary>
		/// <param name="message">The Error Message</param>
		public CamException(string message) : base(message)
		{	
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="xCom.CamManager.CamException"/> class.
		/// </summary>
		/// <param name="message">The Error Message.</param>
		/// <param name="innerException">Inner exception.</param>
		public CamException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="xCom.CamManager.CamException"/> class.
		/// </summary>
		/// <param name="info">The SerializationInfo.</param>
		/// <param name="context">The Context.</param>
		protected CamException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}




























