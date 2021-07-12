using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsusAuraEffect.monitor
{
    public class GpuMonitor : IHardwareMonitor
    {
        /// <summary>
        /// インナークラス
        /// </summary>
        public class UpdateVisitor : IVisitor
        {
            public void VisitComputer(IComputer computer)
            {
                computer.Traverse(this);
            }
            public void VisitHardware(IHardware hardware)
            {
                hardware.Update();
                foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
            }
            public void VisitSensor(ISensor sensor) { }
            public void VisitParameter(IParameter parameter) { }
        }

        /// <summary>
        /// Logger
        /// </summary>
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// リソース更新用Visitor。リソース終了時に必ず破棄すること。
        /// </summary>
        private UpdateVisitor MyUpdateVisitor { set; get; }

        /// <summary>
        /// OpenHardwareMonitorアクセス用オブジェクト。リソース終了時に必ず破棄すること。
        /// </summary>
        private Computer Computer { get; set; }

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public GpuMonitor()
        {
        }

        #region "Bootstrap and Closing"

        /// <summary>
        /// OpenHardwareMonitorを開く
        /// </summary>
        public void Open()
        {
            // Create instance
            MyUpdateVisitor = new UpdateVisitor();

            if (Computer != null)
            {
                // try close
                Close();
            }
            // Create instance
            Computer = new Computer();
            Computer.Open();
            Computer.MainboardEnabled = false;
            Computer.GPUEnabled = true;
            // Ready
            Computer.Accept(MyUpdateVisitor);
        }

        /// <summary>
        /// 強制的にリソースを閉じる
        /// </summary>
        public void Close()
        {
            _logger.Info(string.Format("Try to closing resource."));
            // リソースがある場合のみクローズ
            if (Computer != null)
            {
                Computer.Close();
                _logger.Debug(string.Format("_computer is not null, closed"));
            }
        }

        /// <summary>
        /// リソースの開放を実施する
        /// </summary>
        public void Dispose()
        {
            // クローズそのものはクローズ関数で実施する
            _logger.Debug(string.Format("Dispose called."));
            Close();
        }

        #endregion

        /// <summary>
        /// インデックスを指定してGPUから温度情報を取得
        /// </summary>
        /// <param name="hardwareIdx"></param>
        /// <returns></returns>
        public float? GetValue()
        {
            _logger.Debug("Get temperature start.");

            // 戻り値を定義
            float? tempure = null;

            // インデックス用の変数を定義
            foreach (var hardware in Computer.Hardware)
            {
                if (hardware.HardwareType == HardwareType.GpuNvidia || 
                    hardware.HardwareType == HardwareType.GpuAti)
                {
                    hardware.Update();
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature &&
                            sensor.Value.HasValue == true)
                        {
                            _logger.Debug($"Tempture:{sensor.Value.Value}");
                            // Get value (sensor.Value is flaot?)
                            tempure = sensor.Value;
                            break;
                        }
                    }
                }
            }

            return tempure;
        }
    }
}
