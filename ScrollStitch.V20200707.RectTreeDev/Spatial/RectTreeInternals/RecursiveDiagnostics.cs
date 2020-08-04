using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial.RectTreeInternals
{
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Text;

    public class RecursiveDiagnostics
    {
        public Node StartNode;
        public int IndentSize;

        public RecursiveDiagnostics(Node startNode)
        {
            StartNode = startNode;
            IndentSize = 2;
        }

        public MultiLineTextOutput Generate()
        {
            MultiLineTextOutput output = new MultiLineTextOutput();
            Generate(output);
            return output;
        }

        public void Generate(IMultiLineTextOutput output)
        {
            _Generate(indentLevel: 0, path: "root", node: StartNode, output: output);
        }

        private void _Generate(int indentLevel, string path, Node node, IMultiLineTextOutput output)
        {
            StringBuilder sb = null;
            void WriteToOutput(int extraLevel, string s)
            {
                sb = sb ?? new StringBuilder();
                sb.Append(' ', IndentSize * (indentLevel + extraLevel));
                sb.Append(s);
                output.AppendLine(sb.ToString());
                sb.Clear();
            }
            string nodeName = $"Node (Path={path})";
            WriteToOutput(0, nodeName);
            node.Bounds.Deconstruct(out int left, out int center, out int right, out int top, out int middle, out int bottom);
            string boundsNameHorz = $".Bounds.Horz L={left}, C={center}, R={right}";
            string boundsNameVert = $".Bounds.Vert T={top}, M={middle}, B={bottom}";
            WriteToOutput(1, boundsNameHorz);
            WriteToOutput(1, boundsNameVert);
            for (int childIndex = 0; childIndex < 5; ++childIndex)
            {
                WhichChild whichChild = (WhichChild)childIndex;
                string whichChildName = whichChild.ToShortString();
                var list = node.RecordList[childIndex];
                if (!(list is null))
                {
                    string childListName = $".List[{childIndex}, {whichChildName}]";
                    WriteToOutput(1, childListName);
                    int itemCount = list.Count;
                    for (int itemIndex = 0; itemIndex < itemCount; ++itemIndex)
                    {
                        var record = list[itemIndex];
                        string rectName = _MyRectString(record.Rect);
                        string recordName = $".Item[{itemIndex}] = (Index={record.Index}, {rectName})";
                        WriteToOutput(2, recordName);
                    }
                }
                var childNode = node.ChildList[childIndex];
                if (!(childNode is null))
                {
                    string childNodeName = $".Child[{childIndex}, {whichChildName}]";
                    WriteToOutput(1, childNodeName);
                    string childPath = $"{path}.{whichChildName}";
                    _Generate(indentLevel + 2, childPath, childNode, output);
                }
            }
        }

        /// <summary>
        /// This method prints also the right and bottom, in addition to the XYWH that is 
        /// printed by the built-in <see cref="Rect.ToString"/> implementation.
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        private static string _MyRectString(Rect rect)
        {
            return $"(L={rect.Left}, T={rect.Top}, R={rect.Right}, B={rect.Bottom}, W={rect.Width}, H={rect.Height})";
        }
    }
}
