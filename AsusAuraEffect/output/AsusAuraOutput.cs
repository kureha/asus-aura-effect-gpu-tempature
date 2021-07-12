using AuraServiceLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsusAuraEffect.output
{
    public class AsusAuraOutput : ILedOutput
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// AuraSDK
        /// </summary>
        private IAuraSdk AuraSdk { get; set; }

        public AsusAuraOutput()
        {
            // Get Aura SDK Interface
            AuraSdk = new AuraSdk();
            AuraSdk.SwitchMode();

            _logger.Info("Created aura interface.");
        }

        public void Open()
        {
            // with no action
        }

        public void Close()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            // with no atcion
        }
        
        /// <summary>
        /// Aura対応デバイスのすべてのLEDに色を設定します
        /// </summary>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>
        /// <returns>成功/失敗</returns>
        public bool SetColor(int r, int g, int b)
        {
            // デバイス一覧を取得（全デバイス）
            IAuraSyncDeviceCollection auraDeviceList = AuraSdk.Enumerate(0);

            // 全デバイスに実施
            foreach (IAuraSyncDevice dev in auraDeviceList)
            {
                // デバイスの全LEDに実施
                foreach (IAuraRgbLight light in dev.Lights)
                {
                    // 色を設定
                    light.Color = ConvertColor(r, g, b);
                }
                // 適用実施
                dev.Apply();
            }

            return true;
        }

        /// <summary>
        /// 色をカラーコードに変換します
        /// </summary>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>
        /// <returns>カラーコード</returns>
        public static uint ConvertColor(int r, int g, int b)
        {
            // カラーコードの最大・最低を指定
            const int MIN_COLOR_VALUE = 0;
            const int MAX_COLOR_VALUE = 255;

            // 入力値チェック
            if (r < MIN_COLOR_VALUE || r > MAX_COLOR_VALUE ||
                g < MIN_COLOR_VALUE || g > MAX_COLOR_VALUE ||
                b < MIN_COLOR_VALUE || b > MAX_COLOR_VALUE)
            {
                throw new ArgumentOutOfRangeException($"Parameters are out of range(0~255): r = {r}, g = {g}, b = {b}");
            }

            return uint.Parse(
                $"{b:X2}{g:X2}{r:X2}",
                System.Globalization.NumberStyles.HexNumber);
        }
    }
}
