using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsusAuraEffect.monitor
{
    interface IHardwareMonitor : IDisposable
    {
        /// <summary>
        /// リソースを開くメソッド
        /// </summary>
        void Open();

        /// <summary>
        /// リソースを閉じるメソッド
        /// </summary>
        void Close();

        /// <summary>
        /// データを取得するメソッド
        /// </summary>
        /// <returns></returns>
        float? GetValue();
    }
}
