using AsusAuraEffect.monitor;
using AsusAuraEffect.output;
using AsusAuraEffect.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AsusAuraEffect
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region "CONST DEFINES"
        /// <summary>
        /// 処理インターバル（自動的に計算）
        /// </summary>
        public const int NUM_INTERVAL_MSEC = CHANGE_STEP_COUNT * CHANGE_SLEEP_INTERVAL_MSEC;

        /// <summary>
        /// モニタリング非稼働時のボタン文字列
        /// </summary>
        public const string STR_START_MONITOR = "START MONITOR";

        /// <summary>
        /// モニタリング稼働時のボタン文字列
        /// </summary>
        public const string STR_END_MONITOR = "END MONITOR";

        /// <summary>
        /// モニタリング失敗時のボタン文字列
        /// </summary>
        public const string STR_FAILED_MONITOR = "DETECTION FAILED";

        /// <summary>
        /// 温度変化時の段階を示す
        /// </summary>
        public const int CHANGE_STEP_COUNT = 10;

        /// <summary>
        /// 温度変化時、一段階の色変化を何msecで処理するかを示す
        /// </summary>
        public const int CHANGE_SLEEP_INTERVAL_MSEC = 30;

        /// <summary>
        /// 色の最大値
        /// </summary>
        public const int COLOR_MAX = 255;
        #endregion

        #region "DEFINE INSTANCE VARIABLES"
        /// <summary>
        /// モニタリング動作状態（有効・無効）を示す
        /// </summary>
        private bool isActive = false;

        /// <summary>
        /// 色変化の温度最低値
        /// </summary>
        private int minTempature = 0;

        /// <summary>
        /// 色変化の温度最高値
        /// </summary>
        private int maxTempature = 0;
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            // ラベルを待機中向けに変更
            btnGetGpuTempature.Content = STR_START_MONITOR;
        }

        private async void BtnGetGpuTempature_Click(object sender, RoutedEventArgs e)
        {
            ILedOutput auraManager = new AsusAuraOutput();
            SpectreUtil spectreUtil = new SpectreUtil();

            if (isActive == true)
            {
                // 起動中　→　停止処理を実施
                isActive = false;
            }
            else
            {
                // 停止中　→　起動して待機

                // ラベルを起動中向けに変更
                btnGetGpuTempature.Content = STR_END_MONITOR;

                // モニタリング処理開始
                using (IHardwareMonitor statusManager = new GpuMonitor())
                {
                    // 監視開始
                    statusManager.Open();

                    // 前回温度、色変化開始カラー、現状温度
                    int beforeTemp = 0;
                    int startColorIdx = 0;
                    int nowTemp = 0;

                    // 温度を取得し状態を確認
                    float? rawTempValue = statusManager.GetValue();
                    if (rawTempValue.HasValue == false)
                    {
                        // 失敗時は即死させる
                        isActive = false;

                        // 内容をフォームに反映
                        WriteInfoToForm(STR_FAILED_MONITOR, COLOR_MAX, COLOR_MAX, COLOR_MAX);
                    }
                    else
                    {
                        // その他の時は処理続行
                        isActive = true;

                        // 前回温度と、前回の色インデックスを取得
                        beforeTemp = (int)rawTempValue.Value;
                        startColorIdx = spectreUtil.GetTempatureIndex(beforeTemp, minTempature, maxTempature);

                        // 最初の1回目は強制的に色を変える
                        auraManager.SetColor(spectreUtil.ColorList[startColorIdx].R, spectreUtil.ColorList[startColorIdx].G, spectreUtil.ColorList[startColorIdx].B);

                        // 内容をフォームに反映
                        WriteInfoToForm(rawTempValue.Value, spectreUtil.ColorList[startColorIdx].R, spectreUtil.ColorList[startColorIdx].G, spectreUtil.ColorList[startColorIdx].B);

                        // 1回目の変更が終わったので待機
                        Thread.Sleep(NUM_INTERVAL_MSEC);
                    }

                    // メイン処理ループ開始
                    await Task.Run(() =>
                    {
                        while (isActive)
                        {
                            // 温度を取得し状態を確認
                            rawTempValue = statusManager.GetValue();
                            if (rawTempValue.HasValue == false)
                            {
                                // 失敗時は即死させる
                                isActive = false;
                                continue;
                            }

                            // 現在の温度を取得
                            nowTemp = decimal.ToInt32(new decimal(rawTempValue.Value));

                            // 前回温度と今回温度の色情報を取得
                            int beforeIdx = spectreUtil.GetTempatureIndex(beforeTemp, minTempature, maxTempature);
                            int nowIdx = spectreUtil.GetTempatureIndex(nowTemp, minTempature, maxTempature);

                            _logger.Debug($"温度変化を検知：{beforeTemp} -> {nowTemp}, [{beforeIdx} -> {nowIdx}]");

                            // スペクトル変化用の色リストを計算
                            List<SpectreUtil.Color> colorList = spectreUtil.ChangeColor(
                                spectreUtil.ColorList[beforeIdx],
                                spectreUtil.ColorList[nowIdx],
                                CHANGE_STEP_COUNT);

                            // 色リストに対し描画を実施
                            _logger.Debug($"色変化開始：({colorList[0].R}, {colorList[0].G}, {colorList[0].B}) -> ({colorList[colorList.Count - 1].R}, {colorList[colorList.Count - 1].G}, {colorList[colorList.Count - 1].B})");
                            foreach (SpectreUtil.Color color in colorList)
                            {
                                // 色状態をASUS AURAに書き込み
                                auraManager.SetColor(color.R, color.G, color.B);

                                // 色状態をフォームに通知
                                Dispatcher.Invoke(new Action<float, int, int, int>(this.WriteInfoToForm), nowTemp, color.R, color.G, color.B);

                                // 指定時間待機
                                Thread.Sleep(NUM_INTERVAL_MSEC);
                            }

                            // 前回温度を更新
                            beforeTemp = nowTemp;
                        }

                    });
                }

                // 終了後はラベルを待機中向けに変更
                btnGetGpuTempature.Content = STR_START_MONITOR;

                // 色だけクリア
                WriteInfoToForm(lblGpuTempature.Content.ToString(), COLOR_MAX, COLOR_MAX, COLOR_MAX);
            }
        }

        /// <summary>
        /// 温度と色状態をフォームに書き込む
        /// </summary>
        /// <param name="tempature"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        private void WriteInfoToForm(string msg, int r, int g, int b)
        {
            lblGpuTempature.Content = msg;
            Background = new SolidColorBrush(Color.FromRgb((byte)r, (byte)g, (byte)b));
        }

        /// <summary>
        /// 温度と色状態をフォームに書き込む
        /// </summary>
        /// <param name="tempature"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        private void WriteInfoToForm(float tempature, int r, int g, int b)
        {
            lblGpuTempature.Content = $"GPU TEMP : {tempature.ToString()}";
            Background = new SolidColorBrush(Color.FromRgb((byte)r, (byte)g, (byte)b));
        }

        /// <summary>
        /// 最小温度のスライダを変更したときのイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SldMinTempature_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = (Slider)sender;
            minTempature = decimal.ToInt32(new decimal(slider.Value));

            RenderLabelBySliderValue(slider, lblMinTempature);
        }

        /// <summary>
        /// 最大温度のスライダを変更したときのイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SldMaxTempature_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = (Slider)sender;
            maxTempature = decimal.ToInt32(new decimal(slider.Value));

            RenderLabelBySliderValue(slider, lblMaxTempature);
        }

        /// <summary>
        /// 温度の内容をラベルに変更する内部処理
        /// </summary>
        /// <param name="slider"></param>
        /// <param name="label"></param>
        private void RenderLabelBySliderValue(Slider slider, Label label)
        {
            // 値を取得しDecimal型にキャスト
            decimal value = decimal.Round(new decimal(slider.Value), 0);
            // 値の反映
            label.Content = $"{value} ℃";
        }

    }
}
