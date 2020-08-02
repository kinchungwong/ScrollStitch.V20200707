using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Plotting.TextInternals
{
    using Data;
    using Imaging.IO;
    using Imaging.RowAccess;

    public class AsciiConsoleFont_10x14
        : IFixedWidthBitmapFont
    {
        public static Lazy<IntBitmap> LazySingleColumnImage = new Lazy<IntBitmap>(() =>
        {
            return StaticResource.LoadImageFromResource();
        });

        public static AsciiConsoleFont_10x14 DefaultInstance { get; } = new AsciiConsoleFont_10x14();

        public Size CharSize { get; } = new Size(10, 14);

        public Range CharRange { get; } = new Range(32, 128);

        public IntBitmap SingleColumnImage => LazySingleColumnImage.Value;

        public AsciiConsoleFont_10x14()
        {
        }

        public Rect GetRectForChar(int charValue)
        {
            if (!CharRange.Contains(charValue))
            {
                throw new ArgumentOutOfRangeException(nameof(charValue));
            }
            int charRowStart = (charValue - CharRange.Start) * CharSize.Height;
            return new Rect(0, charRowStart, CharSize.Width, CharSize.Height);
        }

        public List<KeyValuePair<int, Rect>> GetAllCharRects()
        {
            var list = new List<KeyValuePair<int, Rect>>(capacity: CharRange.Count);
            CharRange.ForEach((int charValue) =>
            {
                list.Add(new KeyValuePair<int, Rect>(charValue, GetRectForChar(charValue)));
            });
            return list;
        }

        public IntBitmap GetImageForChar(int charValue)
        {
            var rect = GetRectForChar(charValue);
            return BitmapCopyUtility.CropRect(SingleColumnImage, rect);
        }

        public void CopyTo(int charValue, IBitmapRowAccess<int> dest)
        {
            BitmapRowAccessUtility.Copy(GetBitmapRowsForChar(charValue), dest);
        }

        public IBitmapRowAccess<int> GetBitmapRowsForChar(int charValue)
        {
            var rect = GetRectForChar(charValue);
            return BitmapRowAccessUtility.Wrap(SingleColumnImage, rect, false, false);
        }

        #region static resource
        public static class StaticResource
        {
            public static IntBitmap LoadImageFromResource()
            {
                using (var strm = new MemoryStream(GetResourceFileBytes(), writable: false))
                {
                    return BitmapIoUtility.LoadAsIntBitmap(strm);
                }
            }

            public static byte[] GetResourceFileBytes()
            {
                return Convert.FromBase64String(GetResourceDataAsBase64String());
            }

            public static string GetResourceDataAsBase64String()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("iVBORw0KGgoAAAANSUhEUgAAAAoAAAVACAIAAAD+q15HAAAAAXNSR0IArs4c6QAA");
                sb.Append("AARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAABA0SURBVHhe7ZyL");
                sb.Append("eRs5DISvgEs37sWduBFV4j6uqJvBACD4WsmxrEei+b7ES/4kCICPXa2V/PPvoV54");
                sb.Append("qafAv94+Pv/7/Hj75WXTg+Bf76f/Tu+/fg8vVXqj83/s7mXTxXipq2AbmDmx5DQP");
                sb.Append("LsSsZU4sOS21l+GdXnipi7FNlk0asn56V2XFVh2tVFmwT5XPnSqJrd8sWvLeWiWs");
                sb.Append("cBvDclgNDBGz4yQ18t7FNH+oEnLcmR5w+iMbApL33umKmE7MrklvH6cIf5oS+k7n");
                sb.Append("1fsNUr1jhkSwMW4hI2YMsBub9jHC6b0cjA1DtG1NYEE1PcYQpw/30oaY8XvMQMFv");
                sb.Append("7x8YUpi9h5XKgDMpp+b8wrgXTB2e9X2sjFrC3CPmx5z4p20A/LRwbdZ9vQIjThby");
                sb.Append("QtCsTL3f8VeJrR/7w+ByQrO7zZrbb1ie+fgxoGP1tH0i7/3CxqazLOB60VuQRszM");
                sb.Append("euylboRjj3EteVVimw8LJX6qPjAD7SJWffY2/kn7ilhyzIVsAyt3KWI3rD1ok50e");
                sb.Append("+HJQUq3WqnFBQ4HVCVIDjKQOaRwLwQxYQSYcQ/XcOelaC1l4p2thpTudkir2CHLv");
                sb.Append("Q51x5coLpg6ju7KRuti1pV5YwsTodNFq73BuhryddJgztpvQWO+NQQ0H3eB5LUCO");
                sb.Append("h72Suqw3dGbsM55DR3FDZ7IGHeV8FrFtLt6ZB7ch7w2TtHgU2Cpx3dhz6D0O7uUB");
                sb.Append("Q0PmV7g4+JXeZ8Y+8vwo7qOsnc/5Tl/BGKRGBTnm02HcIlRUvbmmkLTGcPDr4LXo");
                sb.Append("m3E0oom391yHUMPKduTNWzim/ZM9p8GBg4U86IWXuhhzVpdLUfot7LflUf2zA/QT");
                sb.Append("Y/vPjV54qWvhsw9khxjys2ff2z8SpC52banHx1jjzFgIuVH9A7i20wsvdS3MY08r");
                sb.Append("Yb1LjvFSP4GxV7dPmvXkXuDsCo14OPRHLOqFAdv5sT96hq5Qw3NXqOG5K+TYHpvo");
                sb.Append("sYqpwGZY11XEu66Q4U1XqLm21MXY3zzYQ59XJYZvWGV8UMUHi/KYbK7RtzaJUWLR");
                sb.Append("sPpGYLVomM0H7MYuwWeMH7sGqcM6MOkoLTvdDtvnvJYAiJjRnFD/aW9FJ5wasgs9");
                sb.Append("N9Z8j2KzeGm50y0wPwrGWsKaUyWU2D9IaZHDa9WPxu2k4DpWcY33vZkh3wNQh4fd");
                sb.Append("BDU8M8ixhsxtlnIcPVfYg6naBTboFpjOhXd8FTxkLT/6crudnbEtjpOnNw5FavgB");
                sb.Append("2as2vffGodgqtL/AdXoc64O9XayMYw1lVnR4qn5hvOqm2A6C3Tp3/1fYpjx+xzRg");
                sb.Append("m0d+KFlvIkFcLLAl0qsW2NI8Ck0cV63HTn0PD7oF1hJLTWuNWd0cHtD3sQ/L3a1K");
                sb.Append("yLGEWbct5vMBdRjyQ+z4RB4x95j5ZWfHZBwTFWk53P6DboQRUXMvbmWOFa3OBYQO");
                sb.Append("qd4xo91NiTZWLoGqhrk5NXaZNMOaTaaLA0da2WJh3FrTEVwLe4VRtS6YVd6fVdG5");
                sb.Append("YCizwmMxImx4qR/HirtXH1gVA9svZGR+f3iga65iqMNMbuRL6rDsesHUMLuGR6mG");
                sb.Append("EdzQFXJsU9ocTgW2vrqu6lybdQts6zd32PSCnHFZOtdni5aurpUCXZfeMmqPpFoL");
                sb.Append("UHOt7bB5Qn1A97C1II4d6lXZFNcLHDuuYXnmgbGtXTu2FjiK5RpGdgY53ukWuM6Y");
                sb.Append("5bZ3zQK3wCyubkrGtHippWXG+6zxOlqna20++U2R2nvQaHxQ0Gr8+D5WksJbgiqh");
                sb.Append("hfGqPwH7XES+pIvxUn+Ba0vdCP/JM8YG/c2u9sYu7Z6AoYtdW+rWWMcvnrpw3WEC");
                sb.Append("Ppbx6FNyHCeopzVE7Ie05VO1qQt6p7ZjV209n/VHY7v1jOruYzvd2/iBnhvzaatf");
                sb.Append("3inD0/JONeNa3kOjcWwcX7DDGRmePLa9z4x9xvMD/Tiuz7CQDpAO56+fECSP9TjV");
                sb.Append("F8aNe+sRM3XzA5lkhjmylysenjelgnUnmXN+oNLbNuLe+DFe6haYM2ErFMKSG5ci");
                sb.Append("k2nr0NYrJ6xhpQsXiM0t5MO/dbXpVStcoHvBVopWqDqDUR4wjQvDMe6kxPjjHO20");
                sb.Append("ytjd3NPvS8wplDyYt3d/0ociLdyd1odpafvN8U6PgZV0ZcCrOmxU3KsqVlKVQa9K");
                sb.Append("nJnnT0un6gOj1qYhueodG/UqcV0Tq0cvt284LEvVvn8HPC1Lad/H3uneOCbDNW1B");
                sb.Append("w4pNx17OyoSjYHTCtqDblE9jY9KHQ7PvzWvsBKMThqLE4sI1m90JN1ngqHd8oAfA");
                sb.Append("Fk+71XG/tMC0kO3UVFP7ZVpgn8KY4yqda22CBznOhA861xt/zDGfg0HV83bb5jvu");
                sb.Append("9Bya4vamjne6Nx7W2pk9Zm2nlZppr8UVtrJmYYtRxvVv9C7FEdvU2PwPnoe6D3TE");
                sb.Append("B7o37j03zYFlWqquhYs48ohvNTbkwxMf6N74jOc3zlo2vdHYW3ygx8DzjQyVji00");
                sb.Append("nhpejG+FECtutDfUybDfqjZpiaRdgO2eRCnHbezMeb0vEkPhHKsWGPK4P11PtMda");
                sb.Append("3Ixrv8d+Ave67dhbfKB7Y3+k6oQ4es/rTT/13PiiuJd6ANz77m5DY29r9zQYc6Vv");
                sb.Append("rOBC1BAlrAdfqj4gQ6PxQffGtgYZrKpQ2n5MXaUlcCzlPu7A3lFAahj3j2pWqr3V");
                sb.Append("akpqG9scw5UY1GG7ZovcaSMeisQHujcePK/6C3AvNGZr4gPdFfvqjPU7LNbAEQlK");
                sb.Append("DLLHp1O8+kMKVOgw9xf7k+q6x/rbqC4HbD/0Fa4l5s+8mLHK0Fewfux0b5whq6rG");
                sb.Append("0nCbtA32E2eJ23eJN9iWu62aNdaRi4sNxjWuYu2ssF23KEYMcYSKD/QAGAmJ3zrw");
                sb.Append("HqxKiFiHhz6Msd1wN7BM+dKEuriVpmwO5dTg2jEai0Ex1iW98edobPwZPMd15zmE");
                sb.Append("2m3cB7o3VjBNwzvsiJtVWuuWtDEt3iMmYYOjtML+cr4mVTi0XUxuvOr7+EAPgOH8");
                sb.Append("377HuoEcpzQwUvNunxZGDPEd+ckfshe46vtYwaRsi3kSG1YWF3F3WJHtcDxgT8ZD");
                sb.Append("jUFd72pGmoxHS6Mb11AwOuGhSHygB8D2WxT46+p+TzRMA4p8rVjwuFFSrbetg/bL");
                sb.Append("DCnG1vLUwBi3TmgVvzbAVj4rI4bC0eLaRxi0UTQjDXOHZdQZtOMD3RVHRicx8v5f");
                sb.Append("lw5nEPR34Fn3xpqz/nSoS/GBcb+/RwzVyRDVtWPfZtJwNzjQvfG8xJgIqyGO+78H");
                sb.Append("Axnts1YOTdEha7Zjcb3KWvLIuOoDJwcNM5BjSByPROkE1LBZ9UPDqzosXixDDS91");
                sb.Append("VzykScrksDcSUlMBaVJwQaymuMjEZw1xTKClxa5UUTA74RbF77bpCn8HlnOk/BD+");
                sb.Append("yX/7QhuBrXt5H4JLepBYo1pdOIB6xxD6yCmtixHvdG9sznqaWPQ0RdxQNGCVoOod");
                sb.Append("5zbwN+TzNlA+6ihQwcpnDCE1TGj/QSoaeFUb2yA79lEQ10igGgVxjQSqk9bGXure");
                sb.Append("2LLV8gxlLMSezogtshBxsypzEflTfRubDfAkJriYUBuhJhxqGJIBL5huh2fdG9Ph");
                sb.Append("QeH/2Dsmb8o5lKvcywN2uNxEw/6QHA9DpoiHxVRFPA+Z8o/nk/qluNO1cKxUyRMw");
                sb.Append("4G4rQS98BZyacr4UMdfLaqFBhml3XMKSG98ZCLwx0FwT90KoYVvR+7ih2UCHZwMd");
                sb.Append("hgYDI44I3MCIIQ2g6wWuBhYYSgNrnCIetngODHW91e6Fr4FVm9r2nvUVjHUybJQO");
                sb.Append("yzEvmBo2Bz+HA6JhLcD1OldXLk/wYiAwaIxqlwUPm3tc57XMYmndXFvqhZd6Yjzs");
                sb.Append("oCKuuGuNzVHKs7XU9Z7vR9cae6kXXuoBcPfiP95JQIm/8h/L5QnpmJ3KrxU6rJ2Q");
                sb.Append("/1Zx7C2s3/eBctMMxvPXY3ypyPWu572f//8Vw9m2d1I3wxG8vVAdsX6TgaT6lHV3");
                sb.Append("omLcZyDvRF/DUeoxfbOhRa1lw/bii37LcbdEfKA/AedipWx6UOnY8tb+MV/3b7hi");
                sb.Append("l3QnuWS4n5KqC3pDGhv+qGhfOS2uQfby3h3HymrL4UBXxzoM2lobdHXMAyk3GA+f");
                sb.Append("HntW8+Cq2A+ubhNVrG1gWWRxhQ96B7c9JhcLZot6Mgy9D/Qn4LbHYoNBjmvamBWb");
                sb.Append("PcfDLllPSa+yDc7vMXIbW8Vuf0P0KD2HpeF02OmKONcMheHrPdTDs9SxOHzHxJLT");
                sb.Append("lrN0AV4llUlyLP2ucemeeKmnx3nkcEHwyhPg2PdBOxULXp2KFffpXOKD3sF9ew9j");
                sb.Append("s0VxPL8F23BVOFrwm/1/E7hYPNZAPqIpNwpkrvWeV90M19j9lGpY5w2KYYvGGkYa");
                sb.Append("cc3ikJZh7JviA10RR/zuBPQVPOsCrF0S8oilrndNiPQseNYVsZ8DnBQf/it41mWY");
                sb.Append("AafiaIDG3kqMF54dI5fxvTrSPCYgw+VMrEciJNzt76pbY76gCAc7rDvJHJgwfrTT");
                sb.Append("VGr4Ex+YyiqTOuOes11a1is1Ma9horQgPtD3cX8ycVgxqOttPr4w9BU867mxorXH");
                sb.Append("DgTNUq6ZwDwWuKawjdQCvOuNha1zR5vhMfCBnhtbnAj49ayYenzM1dnfZlVPrJWr");
                sb.Append("tYxrZsWuA5Oygu37JBqO9kb99C/4TO/jsb3FzvMDPTeONNkPizsz2LCysZsxTzKL");
                sb.Append("ZYa+jqPUY/pmQ4tay9r7wxq5425pYbzq+/hAt8Bc+hm1vk3dshaut6xNM+ZzAEUp");
                sb.Append("cZki6Ks4OJneH1YMFcdtyQ246kdx1Y/jnJEIvi2shpUSFJHCBUYsqqqqxr1H1Y/j");
                sb.Append("Az09bpMFTfcxrnNNF5QvzyFhnzL948eqNjYPUw7QPulBo2u5f1U046jjmz/2GKZH");
                sb.Append("GNsuHeehKAaNxgc9ALZ4uGtVTJ3Dmo9BXdag3zWuH9/E5WCscgyVRw/OqCobXuq5");
                sb.Append("saWsLV7tKF3fFOcN06hwTBfvoJqXiiHrAnHzjsYP9NyYWbGkqEqJUxINBzeqHIMG");
                sb.Append("hlST17kfHGePwVJzzavxI8xADasbHohy4qGCNdMlBKjhpZ4Y1/lJsc4SwN5Kpy6U");
                sb.Append("FmRQNcQxHajjv5LEFWpkj9gnyepsRtQ5sXXH47mb+PDLwOxuG4vPqXAK8okhhppT");
                sb.Append("ZgotVe94p+fGnpaI1XNc0xIN9rvEsoV0EnpVxTYbdnQttwF7f56qE1AdO4a2K9UT");
                sb.Append("qy6r5ERbDgd6YjwsTckyyElj70yZWM2SYTOQE5Vdce1jt5xrR9WcQ2mA7aIr1DyX");
                sb.Append("AUx5doUKNl67QiOuXaE2Nh9Q+64QsRm1VRYLNNWML/Xj2CPuxBgcV3nT5dHjc7/O");
                sb.Append("uZjNm1dVnBn3ssmxbc4x4RCxW+0FO44PdEWcz0QZesOxGrv9MOL1YpKQGcWT6rC6");
                sb.Append("e8HUG9/11oTbYtyMfdRbOhobOvL8TNxnsgYd5XypDsP6mcBe2HQ7PItYC8GmeTw3");
                sb.Append("u97zqdthHloHmGP85kKGrPfB2LuPTNKZ3t/Y/tCZuNdZuzTnsx4cR1ztJZDdzXhL");
                sb.Append("0nOL5SJeMfFkjDq9UGn3rng5QYjivQM70C0w/wMnfK5x4bJf58BKE6+R0j8JI9YQ");
                sb.Append("r1XveKfv4H///R8kQ/OKXICLXQAAAABJRU5ErkJggg==");
                return sb.ToString();
            }
        }
        #endregion
    }
}
