﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Text
{
    public interface ICharArray
    {
        int RowCount { get; }

        int ColumnCount { get; }

        char[,] CharArray { get; }
    }
}
