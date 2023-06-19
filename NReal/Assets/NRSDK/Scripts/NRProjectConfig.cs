/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*
* This file is part of NRSDK.
*
* https://www.nreal.ai/
*
*****************************************************************************/

namespace NRKernal
{
	using System.Collections.Generic;
    using UnityEngine;

	[System.Serializable]
	public class NRProjectConfig: ScriptableObject
    {
		public List<NRDeviceType> targetDeviceTypes = new List<NRDeviceType> {
				NRDeviceType.NrealLight,
				NRDeviceType.NrealAir,
			};

        public string GetTargetDeviceTypesDesc()
        {
            string devices = string.Empty;
            // NRDebugger.Info("Here ZERO: {0}", devices);
            // Uncomment line below for Unity emulator!! (Commented out for push)
            // targetDeviceTypes[0] = NRDeviceType.NrealLight; // NrealAir; //NrealLight; // Switch here //
			// targetDeviceTypes[0] = NRDeviceType.NrealAir; 
			foreach (var device in targetDeviceTypes)
            {
                // NRDebugger.Info("Here EACH: {0}", device);
                if (devices != string.Empty)
                    devices += "|";
                devices += device;
            }
            // devices = "NrealAir"; // NrealLight"; // Switching it here //

            // NRDebugger.Info("Here ONE: {0}", targetDeviceTypes);
            // NRDebugger.Info("Here TWO: {0}", devices);
            return devices;
        }
	}
}
