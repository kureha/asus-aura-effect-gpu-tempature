using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsusAuraEffect.output
{
    interface ILedOutput : IDisposable
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
        bool SetColor(int r, int g, int b);
    }
}
