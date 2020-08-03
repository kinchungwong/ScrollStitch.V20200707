using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Plotting.TextInternals
{
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Imaging.IO;

    public sealed class FixedWidthFont_10x13_1f4d3fb5
        : FixedWidthBitmapFontBase
    {
        public static FixedWidthFont_10x13_1f4d3fb5 DefaultInstance { get; } = new FixedWidthFont_10x13_1f4d3fb5();

        public FixedWidthFont_10x13_1f4d3fb5()
        {
            CharSize = new Size(10, 13);
            CharRange = new Range(33, 127);
            _SingleColumnImageFunc = () => StaticResource.LoadImageFromResource();
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
                sb.Append("iVBORw0KGgoAAAANSUhEUgAAAAoAAATGCAIAAACQNaI2AAAAAXNSR0IArs4c6QAA");
                sb.Append("AARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAA8KSURBVHhe7Z3t");
                sb.Append("jx1VHcf9S0oMJqIvwETjC2IkUWPKGxM1ElOJpmowsYkkjdFINBBRw0pJI4GQxvBQ");
                sb.Append("vBRrq5Rgy8CS8lApSy8rD60Fyv4x/p4fzjlzdy57vTu77jeBO+d8zu9hZu+dc87M");
                sb.Append("mekn9s1UgW88fPrfV878SkqjwvtufuilC8e+I4UaFyrwzQ+9vPFBMJ8Pl1oOvvWB");
                sb.Append("1f9sXHzsW/rJtW59x9Prit/95z1f4sqEqVo/SQtKra09fN1Kd2lycN++g5MpfZIq");
                sb.Append("fN3KCxvdfVLHGKtKcROzVqvkO2LzXeB+32bd9j0U92iZuMjMMQDZZZDstWI6MFBX");
                sb.Append("GtfYDEnqXA9sNs6pWfwiNksOLH6ok4glMfSRsXklhfjBWr+LUQGL76QYu6Fh+MCj");
                sb.Append("F69hVi8dvQmLdz/3/vrJQzcohiKC/IGq8YpDcw4Afa+fnJwTt6ycGrSKMGOCYM5+");
                sb.Append("qJVjSH568sjxi9didooJHrpBPgocYsJm03lTS8PfO/4mZrVxubvva1LlGCDX2wZo");
                sb.Append("MP7KH56/DNUH+UO8x9jYIEUOGPO6/Fr36mXk6OLSX75vGA2pTM08fbUO+VAMKXhs");
                sb.Append("NQNd6jrYxhYxtYaWhm+59+x7G2899SMpopL1bY+/sQVcaw83VeJPHzq5/v5z90ip");
                sb.Append("wDcdfWkjwIwBXrv42AEpkRwXblmOwbaijsF4WngGDcWbON8stU12DDXzsKCKDEtc");
                sb.Append("SHEz72jdSCw73yzzetdzahXPuMpwLlz92eZIbeaOzTwsZU6inFqlHHu19M4YXMPX");
                sb.Append("YOVoB/+lL0RKDU7GRQIt67ADg2L3ag/jka2+D1vG+P3krtVkf9b/dexZ2sNNLQ7z");
                sb.Append("9CNNiiLmeVGaXSTnOrFyKZYZVZ5uzZVaQzsBHz79Lu446u1nfvl5qR1Fam3t4aYi");
                sb.Append("/uzP/ganx6vP/lrKoDlwQ0vH4dJhAwNtXJUU4VVJoxUGOrVrkhWOjlEJZ8eoiAvH");
                sb.Append("qIBLxyjDtWOU4nQV1ZVSq7UU7N1w6igZh+4zDycRxxr2kvpvs4UNGpd5T2z4+JGT");
                sb.Append("69Yo4ewwBafUYg0HEMoYhLUsZyDFPRoLlqtAfgnLMV4iWz956HfQooVF6GB0GEHQ");
                sb.Append("yK5xNeT4C386L1mHk65igFKLzaxBwznwD//15+/Sdo0jLTGe8R1mXDKQYcyoYCDB");
                sb.Append("3I0ESe51akmjwJ/88VN2HfujN5/8Adc2rLHhR8//gbYb+Pr7XzTzgM29++51Li0a");
                sb.Append("mN1z8IHWab8kbVDTuWs8uBrfB4xD9GnXTZtYbmSgfY0FsvsSI5S6BuaEsmR+EVIj");
                sb.Append("tWObNsGFxoF/8Y/Lss+oVx7+MlYm/Pbff/45KYnmweIYph7arpXatx959QMJ3sLE");
                sb.Append("+zGGaWUukZWBms5dY8F4UibVp3s6JaeBPUkw0Gl1MgcxRturZyd6Tncvji0itZUG");
                sb.Append("wdpMQl/ksY03+zHyX4Z23NYo8L1nP5TERasPfKbPGtq+9fRPPtXGtz+x9hHZNrGZ");
                sb.Append("giocTEEFRmimoIwxfTcFRVyaghqpRY0Cf/OhV2yw6Fc1BAOcWt3h0+++9shXaVOt");
                sb.Append("8YoIG6EbMw+x1b1aoipralRaA/V8QsGxOY3BLbZGRnn0kFpLo8A+pbK5GogxTsi0");
                sb.Append("tp6uoalPw4DbjQu3jtM5m7NpbGzAod8/Nymtk2KkEsckQYKDb0sR1XAetVswdc2x");
                sb.Append("k58LV9oxsSuNB8fVCKS5cKX5MC164GUTpALzagznpXM0b1uzZcx73tRKLRzvP3bh");
                sb.Append("6vkHv0HbBf7iXWfe2Xjn1J1STBjMNsyOpRhRNBMNtBbNii2akXmpXY7DKJSVR7m9");
                sb.Append("WoR1r3YFbk0GkzWdr30yBqqc46zNnbRih5P+PNYzY2+aea0lYRkN+ogSVVpTo1lj");
                sb.Append("RWxgvMZxTJqwjkXdd4l9mM2qnSdFnKKy5sANLQv7BSO/2KMYmVw8sktSIMbhOhQK");
                sb.Append("2sbrTFzENqRLkxVdG4yYV9x6m24Fth1jAWvQIX9EzMYcXZJoYv5/9G3O7+PELk0m");
                sb.Append("/MH7Jaml3YpCTFwbQKjQlDAIAtA+g6IjxT0ajmnOECclIMe46Of4kb7FUrwkKM5J");
                sb.Append("UIp1wVDBGePMQeYMmRP2eQXL80McbFHRHnBBeQ+lQlPr0QgwLU01+SrTgLXytsff");
                sb.Append("uPLiH79O221s5i3nYQVr29paNHAMPtxaIrdi92kkGM+4Kj3zCqaOIHdCJMape4li");
                sb.Append("DH4bpiDCaDwDt/oBVkyt0UAwiFqoih3r0xhwWF4e50UBWyU2lUILh1Ib49wrLo2f");
                sb.Append("D89yTrM6mfAFLHuV5nuKezQGjL95l/3CA7azVdBwLI5Ri3e+hdg9GgOOZ60wUg24");
                sb.Append("dXYZjsUxyBsuzPkWcI/GgePYxsY1gnGoUozFSITRsAUFgy1TC6CjoIxJcZBkzn3U");
                sb.Append("VGF2qlU1BlELVbljfdp2nA9LLC0Gy/EQLdj5FnC/xoBxTaAcMlR7MWS1cHAHYASy");
                sb.Append("U6gZqzyjRoHjCjR94rJlrY9pksaADzw6OUIr6oDS86Ust7Y9m7Ewr9T243gShp93");
                sb.Append("ecp1nM8EGUcnrIDj7UKVYzxg0S8pOcesep1jPW70pkbb6CL3Y/0aC45LFUS7BtPP");
                sb.Append("hEUtI25oBBh/3Hq6whNAsXT27ufWVrvuBFYdePRc163J+YMxnU+OHD8HHOiJI8f1");
                sb.Append("9BLwoYNEsA3E4lNIxP7AeoGtKNoy5j1j3KsRYPoz2MkWS3HHqGJ9OpUj3cT4koI1");
                sb.Append("4j34Jjk8fRg/6U/Xg/F4rtkLCRqYjvd02otpW3dScJ9GgcPdu7DmWTAuugvr1/Kd");
                sb.Append("vbi4DRSKhON6N5SXh+BNnFOF2sdtwSDMVxQcGW5rDPiOp30+ZSPFhLWSRpUy3mxg");
                sb.Append("4lLosdbtVmxz03YetBjco7HgxuJZw3gnVe+W+g1XxVgTbrPqnVXB0RaFZWw9DG/i");
                sb.Append("PKYT2xpmh7JfFidglMZUFTj6QBcVzloEpvGhKo41Dev4Mg9GK5yHoiUuBqpV7MBA");
                sb.Append("hTW1C7m1nM9KLUdn3KuxYHyzm0reLme4fOMcyzCazsDmO744rk4NgzRiJ4EjdtLE");
                sb.Append("6CFj8Smy+G3npm3H4RaySO9RR2u8FW33rlm7Azc0AhyfnYqlUeL0FFZ88qr9DBeY");
                sb.Append("yBctPIbluKltxniSjv1HPOeideZIrUTOI09UYjvPVFNTXlDFUv/7ghomfm06zdQx");
                sb.Append("8XKVdsBNbT/G1ZtpBej+YxeoPAhbUbX/2Hla6OkYF36qF/icRizl/Q+e6s6/furO");
                sb.Append("Nr7zWHfqN3edeZ3/h7EEU/Cu69jx610nqQSsq1uBv6PLWBX3aLsxfY3sW5RKYm1f");
                sb.Append("dPpG6qglOMcG19a61QhTbGxQXll0zF/k4pusmGPis/GpAWNPKOXNmLKymNRAiyG1");
                sb.Append("lrYf4zqCsFbA1xWodWrgCxDcuTXADV2CHmNT/TMTh2VqtG4hJpFxXPZAWiguNQJM");
                sb.Append("v+koORE1rONZqsL5FFbgDAtcwoTxlJRhwHxm4wXxLsHVjo3rxGWLNcKKjoyrpR57");
                sb.Append("eACedVBbCrhxVyFZ3/7E2nTGXQdykB7RLGOXvEytCFDi2Y+PomKABo4BWjgEaGI/");
                sb.Append("RD1YtSjsd6HDTerSGhvt4eEYwaYHtdBwXAxZSIYBTvuXac9ah41uzSuMgXwINDy1");
                sb.Append("lvZwU//P2DqRKO5QBjunK3/puh8oW2OT2VcGs/ZwUyPB2HPKV4UXHIDa1tZHRwyV");
                sb.Append("Ygxie8cItVvXDtgxRpaIlERpHXyvnphI/99OzbQkbDsTtUC8+sCK7rscv4TtL2We");
                sb.Append("epyTK7D/OLg1pALMWYHMjeOmdgemi0ksnfMZRlY8Iw1SDLSGgzE5L2aZoJBanC5W");
                sb.Append("qbW1bByHPYvCeDlJVWJkOuaqrWPN/BjHc1xFA8YqtlSD6GJyfG5ulnYBth1v3IvF");
                sb.Append("AxlHwbbNGGrEklX/SYLHIMYUuNXAUov+/Y9ruK2lYT9w1ZeJYbVvjmHPfIdUiMtD");
                sb.Append("Gg7QAGvWduKmdgWmi8eu9GgbXeWVK8DhkmwPLh9t40qRX85VDIGn8RqvqGkNEgfB");
                sb.Append("OlybtuiKQzrhanefc23puCnGzW8KamF4coJ/LLGhYa3Ov8TaeQq0BNyrpWFIqu8+");
                sb.Append("EWoT3JBgOyH6LpOSdToipGVg2ieXNUrWtZaLaW2hXSObF2cZJiuRLy6srW+NLyof");
                sb.Append("F07LIiuMO+gXFhX7bqcVlbXzJMb1t0i0WMzfSfuhJYys9RNkm/IHBorWd1TGRWz8");
                sb.Append("oSYXZWocx5ow7tVyMK8bB/kzVqDC2hb7i3YBBkC7LZJGhXWpHY9vPHx6emHyV7qq");
                sb.Append("+fhRXF2YVhzSgkOo+Sle97xy5re6cDJgLMs6Ri0uBfdp52P4BU71H6oMzwQGbA92");
                sb.Append("hG9zxPEZENleCu7RbsDh1e6+NlFwXIRIaw/zekVfwIjyxY3D8K5c7ThLwzD0nPQv");
                sb.Append("k/B+ezfs2DrW0N8G7B0zuWL7uXGa9cTYInfTdh60ENynpeE0fed5rGGf98b5suI0");
                sb.Append("g54bhxk2T6ULHGbYYR1ZwK6G86itYddS8C33np2+eOxhefWVLZ8J+D2tDe/Piji8");
                sb.Append("Kku1FNyjXYHD7RK7u2F45uo3EBsXd0XK2HLbY9YdjxDHcEgsRGhZB40Gy0hRSqiB");
                sb.Append("GIjstIgGmgtyTtoE2+DWlHDMsJFarR2P42O3cXtZmB/jxS1/aldweKL36rMr9s9H");
                sb.Append("OW5rp+M0yG2MU0NN2jbn6IDrkOpA1WMrjzSldv39L7z55A/xf5pEwmiY/tmlAtfa");
                sb.Append("4figvaOWhY+h8StaCWtRm3lzds7l61YmXdfJe2v5/a6M5cWzHb5wtsP33WosSQ3M");
                sb.Append("wRBMoCH+u1D6cljHMNJCE/Tsj8cJ7tNOxzT21wkBFnSwbtZYiQ3yQD44l8v7NqtA");
                sb.Append("ZTydTnseXlGnLecxGypoiOC8pR2PsXP1ztdLZh0659ARu3OonBK3DVCILUaR5tQg");
                sb.Append("wJWuezn08Alz7x77/xpHmjAGbw8tZMyQLFHZeaWl4OIpjv5xqj4yRyowOEkPkSQM");
                sb.Append("Q9liYVDAyauoJzUdBtepJS0VU0/n16EyDn0vq8axj5wLex+uUowk/nuLooHWrC2l");
                sb.Append("NgDPHhxgg96Diq+siG+e2MOs7cSlFFPflq68kZJ17iBRCWMn+/Exek/dd4FBsf+d");
                sb.Append("z3prqW2C+w8LJbzJQa01CiwvhZGvkn6pFEsZPuitMwUG2ym/jCZ7WVBqbW0F79v3");
                sb.Append("X+HESmc6TYmjAAAAAElFTkSuQmCC");
                return sb.ToString();
            }
        }
        #endregion
    }
}
