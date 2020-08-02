using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Plotting
{
    using Data;
    using RowAccess;

    public interface IFixedWidthBitmapFont
    {
        Size CharSize { get; }

        Range CharRange { get; }

        /// <summary>
        /// Copies the specified font character bitmap into the caller-specified bitmap rows.
        /// 
        /// <para>
        /// This method copies the pixel data available from the font character bitmap. Aside from the 
        /// fact that the pixel data is 32-bit, the interpretation of that pixel data may depend on
        /// the particular font implementation. It is not necessarily encoded in RGBA or PRGBA format.
        /// </para>
        /// </summary>
        /// 
        /// <param name="charValue">
        /// The character to be copied from the font bitmap.
        /// </param>
        /// 
        /// <param name="dest">
        /// An instance of <see cref="IBitmapRowAccess{T}"/> of <see langword="int"/> where the font 
        /// pixel data will be copied into.
        /// </param>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// The specified character is outside the character range.
        /// </exception>
        /// 
        void CopyTo(int charValue, IBitmapRowAccess<int> dest);

        /// <summary>
        /// Provides read-only access to the pixel data available from the font character bitmap.
        /// </summary>
        /// <param name="charValue"></param>
        /// 
        /// <returns>
        /// An instance of <see cref="IBitmapRowAccess{T}"/> of <see langword="int"/> from which
        /// the font pixel data for the specified character can be copied.
        /// </returns>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// The specified character is outside the character range.
        /// </exception>
        /// 
        IBitmapRowAccess<int> GetBitmapRowsForChar(int charValue);
    }
}
