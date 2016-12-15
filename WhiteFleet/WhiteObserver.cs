using ElectronicObserver.Data;
using ElectronicObserver.Window;
using ElectronicObserver.Observer;
using ElectronicObserver.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace WhiteFleet
{
	class WhiteObserver
	{
		#region Singleton

		public static WhiteObserver Instance { get; } = new WhiteObserver();

		#endregion

		private WhiteObserver()
		{
		}

		private static readonly int[] TargetShips = { 24, 25, 114 }; // Ooi, Kitagami, Abukuma
		private HashSet<int> shipIds;
		private string fileName;

		private MediaPlayer mediaPlayer = new MediaPlayer();
		private bool _isPlaying = false;
		private bool isPlaying
		{
			get { return _isPlaying; }
			set
			{
				if (_isPlaying != value)
				{
					// Logger.Add(2, "WhiteFleet: Status change triggered: " + value.ToString());
					_isPlaying = value;
					if (value)
					{
						mediaPlayer.Play();
					}
					else
					{
						mediaPlayer.Stop();
					}
				}
			}
		}

		public void Initialize()
		{
			APIObserver o = APIObserver.Instance;
			foreach (string api in new[] { "api_req_hensei/change" })
				o[api].RequestReceived += FleetChange;
			foreach (string api in new[] { "api_req_hensei/preset_select", "api_port/port", "api_get_member/deck" })
				o[api].ResponseReceived += FleetChange;

			fileName = Path.Combine(Path.GetTempPath(), "Todokanai_Koi.mp3");
			File.WriteAllBytes(fileName, Resource.Todokanai_Koi);
			mediaPlayer.SetPlaylist(new string[] { fileName });
			mediaPlayer.SourcePath = fileName;
			mediaPlayer.IsLoop = true;
		}

		private void UpdateShipIds()
		{
			if (shipIds == null)
			{
				shipIds = new HashSet<int>();
				foreach (int startId in TargetShips)
				{
					shipIds.Add(startId);
					ShipDataMaster ship = KCDatabase.Instance.MasterShips[startId];
					while (ship.RemodelAfterShip != null && !shipIds.Contains(ship.RemodelAfterShipID))
					{
						ship = ship.RemodelAfterShip;
						shipIds.Add(ship.ShipID);
					}
				}
			}
		}

		private void FleetChange(string apiname, dynamic data)
		{
			UpdateShipIds();
			isPlaying = ShouldPlay();
		}

		private bool ShouldPlay()
		{
			var e = KCDatabase.Instance.Fleet.Fleets[1].MembersInstance.Where(i => i != null);
			// 改装された ship cannot appear together with the original one so we can compare Count().
			return e.Count() == TargetShips.Count() && e.All(i => shipIds.Contains(i.ShipID));
		}
	}
}
