using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ScrollStitch.V20200707.Config.Data
{
    public class ClassParallelPermission
    {
        public string ClassName { get; set; }

        public ClassParallelPermissionProfile NormalProfile { get; set; }

        public ClassParallelPermissionProfile ThreadPoolProfile { get; set; }

        public static bool TryParseXml(XmlNode permNode, out ClassParallelPermission result)
        {
            result = null;
            string permClassName = permNode.SelectSingleNode("ClassName")?.InnerText;
            if (string.IsNullOrEmpty(permClassName))
            {
                return false;
            }
            XmlNode normalNode = permNode.SelectSingleNode("NormalProfile");
            XmlNode threadPoolNode = permNode.SelectSingleNode("ThreadPoolProfile");
            if (normalNode is null &&
                threadPoolNode is null)
            {
                // nothing interesting, don't return an object.
                return false;
            }
            result = new ClassParallelPermission()
            {
                ClassName = permClassName
            };
            if (!(normalNode is null))
            {
                if (ClassParallelPermissionProfile.TryParseXml(normalNode, out var normalProf))
                {
                    result.NormalProfile = normalProf;
                }
                else
                {
                    string strProblem = Text.CharArrayFilterUtility.RemoveControlAndHighChars(normalNode.InnerText);
                    Logging.Sinks.LogMemorySink.DefaultInstance.Add(DateTime.Now,
                        "CONFIG WARNING: Cannot parse " + strProblem);

                }
            }
            if (!(threadPoolNode is null))
            {
                if (ClassParallelPermissionProfile.TryParseXml(threadPoolNode, out var threadProf))
                {
                    result.ThreadPoolProfile = threadProf;
                }
                else
                {
                    string strProblem = Text.CharArrayFilterUtility.RemoveControlAndHighChars(threadPoolNode.InnerText);
                    Logging.Sinks.LogMemorySink.DefaultInstance.Add(DateTime.Now,
                        "CONFIG WARNING: Cannot parse " + strProblem);
                }
            }
            return true;
        }
    }
}
