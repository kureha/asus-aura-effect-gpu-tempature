using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraEffect.monitor
{
    public class CpuMonitor : IHardwareMonitor 
	{
		public unsafe struct EffectiveFreqData
		{
			uint uLength;           ///< Length of the Array  
			double* dFreq;                  ///< Address of the Array of Effective Frequency in Mhz
			double* dState;                 ///< Address of the Array of C0 State Residency in %
		}

		public struct CPUParameters
		{
			uint eMode;                   ///< OCMode Structure
			EffectiveFreqData stFreqData;   ///< EffectiveFreqData Structure
			double dPeakCoreVoltage;        ///< Peak Core Voltage
			double dSocVoltage;             ///< Current SOC Voltage
			double dTemperature;            ///< Current Temperature(C)
			double dAvgCoreVoltage;         ///< Average Core Voltage of that Sample period
			double dPeakSpeed;              ///< Peak Speed of all the Active cores
			float fPPTLimit;                ///< PPT Current Limit(W)
			float fPPTValue;                ///< PPT Current value(W)
			float fTDCLimit_VDD;            ///< TDC(VDD) Current Limit(A)
			float fTDCValue_VDD;            ///< TDC(VDD) Current Value(A)
			float fEDCLimit_VDD;            ///< EDC(VDD) Current limit(A)
			float fEDCValue_VDD;            ///< EDC(VDD) Current Value(A)
			float fcHTCLimit;               ///< cHTC Current Limit(celsius)
			float fFCLKP0Freq;              ///< FCLK P0 Frequency
			float fCCLK_Fmax;               ///< CCLK Fmax frequency(MHz)
			float fTDCLimit_SOC;            ///< TDC(SOC) limit in ampere(A)
			float fTDCValue_SOC;            ///< TDC(SOC) current value  in ampere(A)
			float fEDCLimit_SOC;            ///< EDC(SOC) limit  in ampere(A)
			float fEDCValue_SOC;            ///< EDC(SOC) Avg value  in ampere(A)
			float fVDDCR_VDD_Power;         ///< VDDCR_VDD Power in watts(W)
			float fVDDCR_SOC_Power;         ///< VDDCR_SOC Power in watts(W)
		}

		public void Close()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public float? GetValue()
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
            throw new NotImplementedException();
        }
    }
}
