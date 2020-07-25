using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ScrollStitch.V20200707.Config
{
    using Data;

    public class TestClassConfig
    {
        private static Lazy<TestClassConfig> _StaticLazy = 
            new Lazy<TestClassConfig>(
                () => new TestClassConfig());

        public static TestClassConfig DefaultInstance => _StaticLazy.Value;

        public static ConfigVariableSubstitutions VarSub => ConfigVariableSubstitutions.DefaultInstance;

        private MemoryStream _strm;
        private XmlDocument _xmlDoc;

        public List<TestSet> TestSets { get; set; }
        public CurrentTestSet CurrentTestSet { get; set; }

        public TestClassConfig()
        {
            _LoadXml();
            _ParseTestSets();
            _ParseCurrentTestSet();
            _Cleanup();
        }

        public void _LoadXml()
        {
            _strm = new MemoryStream(File.ReadAllBytes("TestClassConfig.xml"), writable: false);
            _xmlDoc = new XmlDocument();
            _xmlDoc.Load(_strm);
        }

        public void _ParseTestSets()
        {
            TestSets = new List<TestSet>();
            XmlNode testSetsNode = _xmlDoc.SelectSingleNode("//TestSets");
            XmlNodeList testSetNodes = testSetsNode.SelectNodes("TestSet");
            foreach (XmlNode testSetNode in testSetNodes)
            {
                string name = testSetNode.SelectSingleNode("Name").InnerText;
                string localPath = testSetNode.SelectSingleNode("LocalPath").InnerText;
                XmlNode filterListMaybe = testSetNode.SelectSingleNode("FilterList");
                if (!(filterListMaybe is null))
                {
                    // Currently cannot handle filtering
                    continue;
                }
                localPath = VarSub.Process(localPath);
                TestSet ts = new TestSet()
                {
                    Name = name,
                    LocalPath = localPath
                };
                TestSets.Add(ts);
            }
        }

        public void _ParseCurrentTestSet()
        {
            XmlNode currentTestSetNode = _xmlDoc.SelectSingleNode("//CurrentTestSet");
            string currentTestSetName = currentTestSetNode.SelectSingleNode("TestSetName").Attributes["name"].Value;
            string strTakeMaybe = currentTestSetNode.SelectSingleNode("Take")?.Attributes["value"]?.Value;
            CurrentTestSet = new CurrentTestSet()
            { 
                TestSetName = currentTestSetName,
                Take = (string.IsNullOrEmpty(strTakeMaybe) ? int.MaxValue : int.Parse(strTakeMaybe))
            };
        }

        public void _Cleanup()
        {
            if (!(_strm is null))
            {
                _strm.Dispose();
                _strm = null;
            }
            _xmlDoc = null;
        }
    }
}
