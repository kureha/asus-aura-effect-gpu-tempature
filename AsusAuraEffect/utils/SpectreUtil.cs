using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsusAuraEffect.utils
{
    public class SpectreUtil
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 色情報を保存する構造体
        /// </summary>
        public class Color : IComparable
        {
            /// <summary>
            /// Red
            /// </summary>
            public int R { get; set; }

            /// <summary>
            /// Green
            /// </summary>
            public int G { get; set; }

            /// <summary>
            /// Blue
            /// </summary>
            public int B { get; set; }

            /// <summary>
            /// オブジェクトを比較し、等しいかを確認します
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int CompareTo(object obj)
            {
                if (!(obj is Color color))
                {
                    throw new ArgumentException("Compare object can't be null.");
                }

                if (
                    color.R == this.R && 
                    color.B == this.B && 
                    color.G == this.G)
                {
                    return 0;
                } else
                {
                    return 1;
                }
            }

            /// <summary>
            /// 色情報が等しいかを確認する
            /// </summary>
            /// <param name="color"></param>
            /// <returns></returns>
            public bool ColorEquals(Color color)
            {
                if (color == null)
                {
                    return false;
                }

                if(
                    color.R == this.R &&
                    color.B == this.B &&
                    color.G == this.G)
                {
                    return true;
                } else
                {
                    return false;
                }
            }

            /// <summary>
            /// Colorが正しい値かを返します
            /// </summary>
            /// <returns></returns>
            public bool IsValid()
            {
                if (R >= SpectreUtil.MIN_COLOR_VALUE && R <= SpectreUtil.MAX_COLOR_VALUE &&
                    G >= SpectreUtil.MIN_COLOR_VALUE && G <= SpectreUtil.MAX_COLOR_VALUE &&
                    B >= SpectreUtil.MIN_COLOR_VALUE && B <= SpectreUtil.MAX_COLOR_VALUE)
                {
                    return true;
                } else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// カラー一覧
        /// </summary>
        public List<Color> ColorList { get; private set; }

        /// <summary>
        /// 色の取りうる最低値
        /// </summary>
        public const int MIN_COLOR_VALUE = 0;

        /// <summary>
        /// 色の取りうる最大値
        /// </summary>
        public const int MAX_COLOR_VALUE = 255;

        /// <summary>
        /// スペクトル一覧を生成する。
        /// </summary>
        public SpectreUtil()
        {
            _logger.Info($"Initialize spectre util start.");

            // インスタンス生成
            ColorList = new List<Color>();

            // Phase.1 0x000000ff -> 0x0000ffff
            for(int i = MIN_COLOR_VALUE; i <= MAX_COLOR_VALUE; i++)
            {
                ColorList.Add(new Color() { R = MIN_COLOR_VALUE, G = i, B = MAX_COLOR_VALUE });
            }

            // Phase.2 0x0000ffff -> 0x0000ff00
            for (int i = MAX_COLOR_VALUE; i >= MIN_COLOR_VALUE ; i--)
            {
                ColorList.Add(new Color() { R = MIN_COLOR_VALUE, G = MAX_COLOR_VALUE, B = i });
            }

            // Phase.3 0x0000ff00 -> 0x00ffff00
            for (int i = MIN_COLOR_VALUE; i <= MAX_COLOR_VALUE; i++)
            {
                ColorList.Add(new Color() { R = i, G = MAX_COLOR_VALUE, B = MIN_COLOR_VALUE });
            }

            // Phase.4 0x00ffff00 -> 0x00ff0000
            for (int i = MAX_COLOR_VALUE; i >= MIN_COLOR_VALUE; i--)
            {
                ColorList.Add(new Color() { R = MAX_COLOR_VALUE, G = i, B = MIN_COLOR_VALUE });
            }

            // Phase.5 0x00ff0000 -> 0x00ff00ff
            for (int i = MIN_COLOR_VALUE; i <= MAX_COLOR_VALUE; i++)
            {
                ColorList.Add(new Color() { R = MAX_COLOR_VALUE, G = MIN_COLOR_VALUE, B = i });
            }

            _logger.Info($"Initialize spectre util end.");
        }

        /// <summary>
        /// 色を変化させるための配列レンジを取得する
        /// </summary>
        /// <param name="beforeColor"></param>
        /// <param name="afterColor"></param>
        /// <returns></returns>
        public List<Color> ChangeColor(Color beforeColor, Color afterColor)
        {
            // 返却用変数を取得
            var colorList = new List<Color>();

            // BeforeのIndexを取得
            var beforeIdx = this.ColorList.FindIndex(beforeColor.ColorEquals);
            var afterIdx = this.ColorList.FindIndex(afterColor.ColorEquals);

            // 取得できない場合は空リストの返却をする
            if (beforeIdx == -1 || afterIdx == -1)
            {
                return colorList;
            }

            if (beforeIdx < afterIdx)
            {
                // 昇順で入力された場合は、素直にレンジを切り出す
                colorList = this.ColorList.GetRange(beforeIdx, Math.Abs(afterIdx - beforeIdx) + 1);
            } else
            {
                // 降順の場合は切り出した後に逆順にする
                colorList = this.ColorList.GetRange(afterIdx, Math.Abs(afterIdx - beforeIdx) + 1);
                colorList.Reverse();
            }

            // 処理終了
            return colorList;
        }

        /// <summary>
        /// 色変化用のリストを取得します
        /// </summary>
        /// <param name="beforeColor">変更前の色</param>
        /// <param name="afterColor">変更後の色</param>
        /// <param name="count">リストの色の数を指定</param>
        /// <returns>色リスト一覧</returns>
        public List<Color> ChangeColor(Color beforeColor, Color afterColor, int count)
        {
            // 入力値チェック
            if (count < 2 || count > this.ColorList.Count)
            {
                throw new ArgumentOutOfRangeException($"Count param is out of range. (2 .. {this.ColorList.Count}).");
            }

            // 返却用変数を取得
            var colorList = new List<Color>();

            // BeforeのIndexを取得
            var beforeIdx = this.ColorList.FindIndex(beforeColor.ColorEquals);
            var afterIdx = this.ColorList.FindIndex(afterColor.ColorEquals);

            // 取得できない場合は空リストの返却をする
            if (beforeIdx == -1 || afterIdx == -1)
            {
                return colorList;
            }

            // 変化差分を計算
            Decimal step = (Decimal) Math.Abs(afterIdx - beforeIdx) / (Decimal) (count - 1);

            // 変化の開始点を求める
            int startIdx = 0;
            if (afterIdx > beforeIdx)
            {
                startIdx = beforeIdx;
            } else
            {
                startIdx = afterIdx;
            }

            // 詰め替えを実施する
            for(int i = 0; i < count; i++)
            {
                colorList.Add(this.ColorList[startIdx + (int)(i * step)]);
            }

            // Beforeの方が大きい場合は配列を逆順化
            if (afterIdx <= beforeIdx)
            {
                colorList.Reverse();
            }

            // 処理終了
            return colorList;
        }

        /// <summary>
        /// 温度のインデックスを求めて返却する
        /// </summary>
        /// <param name="minTemp"></param>
        /// <param name="maxTemp"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int GetTempatureIndex(int nowTemp, int minTemp, int maxTemp)
        {
            // 戻り値を定義する
            int index;

            // 最大個数を算出する
            int diffTemp = maxTemp - minTemp;
            if (diffTemp == 0) 
            {
                // 差分が0の時は強制的に　最初の要素を返す
                return 0;
            } 
            else if(diffTemp < 1)
            {
                //throw new ArgumentOutOfRangeException($"Tempature difference must be more than 2. difference = {diffTemp}");
                // 最大より最小が高い場合は逆転処理を行う
                int inputMinTemp = minTemp;
                int inputMaxTemp = maxTemp;

                minTemp = inputMaxTemp;
                maxTemp = inputMinTemp;
            }

            // 入力値に対する最大・最小の評価
            if (nowTemp < minTemp)
            {
                // 入力値が最低値未満ならインデックスの最初を返却する
                return 0;
            } else if (nowTemp > maxTemp)
            {
                // 入力値が最大値以上なら最大値のインデックスを返却する
                return ColorList.Count - 1;
            }

            // 変化差分を計算
            decimal step = (ColorList.Count - 1) / (decimal) diffTemp;

            // 上記をもとに計算実施
            index = Math.Abs(decimal.ToInt32((nowTemp - minTemp) * step));

            return index;
        }
    }
}
