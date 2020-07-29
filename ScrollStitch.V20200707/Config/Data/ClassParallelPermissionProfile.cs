using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ScrollStitch.V20200707.Config.Data
{
    public class ClassParallelPermissionProfile
    {
        public bool UseParallel { get; set; }

        public string WorkSizeUnit { get; set; }

        public long? MinWorkSizePerPartition { get; set; }

        public int? MaxWorkPartitionCount { get; set; }

        public static bool TryParseXml(XmlNode profileNode, out ClassParallelPermissionProfile result)
        {
            result = null;
            string strUseParallel = profileNode.SelectSingleNode("UseParallel")?.InnerText;
            string strWorkSizeUnit = profileNode.SelectSingleNode("WorkSizeUnit")?.InnerText;
            string strMinWorkSizePerPartition = profileNode.SelectSingleNode("MinWorkSizePerPartition")?.InnerText;
            string strMaxWorkPartitionCount = profileNode.SelectSingleNode("MaxWorkPartitionCount")?.InnerText;
            if (string.IsNullOrEmpty(strUseParallel) ||
                !bool.TryParse(strUseParallel, out bool useParallel))
            {
                return false;
            }
            result = new ClassParallelPermissionProfile()
            {
                UseParallel = useParallel,
                WorkSizeUnit = strWorkSizeUnit
            };
            if (!string.IsNullOrEmpty(strMinWorkSizePerPartition) &&
                long.TryParse(strMinWorkSizePerPartition, out long minWorkSize))
            {
                result.MinWorkSizePerPartition = minWorkSize;
            }
            if (!string.IsNullOrEmpty(strMaxWorkPartitionCount) &&
                int.TryParse(strMaxWorkPartitionCount, out int maxPartCount))
            {
                result.MaxWorkPartitionCount = maxPartCount;
            }
            return true;
        }
    }
}
