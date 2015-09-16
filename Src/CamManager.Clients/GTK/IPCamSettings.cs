using System;
using System.Net;

namespace xCom.CamManager
{
	public class IPCamSettings
	{
		private ICam _cam;

		public IPCamSettings()
		{
		}

		public string MPlayerLocation {
			get;
			set;
		}

		public IPAddress Address {
			get;
			set;
		}

		public int Port {
			get;
			set;
		}

		public string UserName {
			get;
			set;
		}

		public string Password {
			get;
			set;
		}

		public ICam Connect()
		{
			if(Address == null)
				throw new CamException("No Address given");

			if(UserName == null)
				throw new CamException("No UserName given");

			if(Password == null)
				throw new CamException("No Password given");

			if(_cam == null)
				_cam = CamFactory.Connect(Address, Port, UserName, Password);

			return _cam;
		}

		public void Disconnect()
		{
			_cam = null;
		}
	}
}

