﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
        public List<Hash2DSpec> Hash2DSpecs { get; set; }

        public Dictionary<string, ClassParallelPermission> ClassParallelPermissions { get; set; }

        public List<KeyValuePair<string, string>> DevelopmentSwitches { get; set; }

        public TestClassConfig()
        {
            _LoadXml();
            _ParseTestSets();
            _ParseCurrentTestSet();
            _ParseHash2DSpecs();
            _ParseClassParallelPermissions();
            _ParseDevelopmentSwitches();
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
            string currentTestSetName = currentTestSetNode.SelectSingleNode("TestSetName").InnerText;
            string itemsTextMaybe = currentTestSetNode.SelectSingleNode("Items")?.InnerText;
            IntegerExpandList items = null;
            if (!string.IsNullOrEmpty(itemsTextMaybe))
            {
                bool parseOk = IntegerExpandList.TryParse(itemsTextMaybe, out items);
                if (!parseOk)
                {
                    items = null;
                }
                if (items.Items?.Count == 0)
                {
                    items = null;
                }
            }
            CurrentTestSet = new CurrentTestSet()
            {
                TestSetName = currentTestSetName,
                Items = items
            };
        }

        public void _ParseHash2DSpecs()
        {
            Hash2DSpecs = new List<Hash2DSpec>();
            XmlNode specsNode = _xmlDoc.SelectSingleNode("//Hash2DSpecs");
            XmlNodeList specNodes = specsNode.SelectNodes("Hash2DSpec");
            foreach (XmlNode specNode in specNodes)
            {
                string name = specNode.SelectSingleNode("Name").InnerText;
                XmlNodeList passNodes = specNode.SelectNodes("Hash2DPasses/Hash2DPass");
                Hash2DSpec spec = new Hash2DSpec(name);
                foreach (XmlNode passNode in passNodes)
                {
                    string strDirection = passNode.Attributes["Direction"].Value;
                    int windowSize = int.Parse(passNode.Attributes["WindowSize"].Value);
                    int skipStep = int.Parse(passNode.Attributes["SkipStep"].Value);
                    int fillValue = int.Parse(passNode.Attributes["FillValue"].Value);
                    Hash2DPass pass = new Hash2DPass()
                    {
                        Direction = strDirection,
                        WindowSize = windowSize,
                        SkipStep = skipStep,
                        FillValue = fillValue
                    };
                    spec.Passes.Add(pass);
                }
                Hash2DSpecs.Add(spec);
            }
        }

        public void _ParseClassParallelPermissions()
        {
            ClassParallelPermissions = new Dictionary<string, ClassParallelPermission>();
            XmlNodeList permNodeList = _xmlDoc.SelectNodes("//ClassParallelPermissions/ClassParallelPermission");
            foreach (XmlNode permNode in permNodeList)
            {
                if (ClassParallelPermission.TryParseXml(permNode, out var perm))
                {
                    ClassParallelPermissions.Add(perm.ClassName, perm);
                }
                else
                {
                    string strProblem = Text.CharArrayFilterUtility.RemoveControlAndHighChars(permNode.InnerText);
                    Logging.Sinks.LogMemorySink.DefaultInstance.Add(DateTime.Now,
                        "CONFIG WARNING: Cannot parse " + strProblem);
                }
            }
        }

        /// <summary>
        /// Gets the parallel permission profile for the specified class name and the current thread.
        /// 
        /// <para>
        /// Important: thread-specific behavior. <br/>
        /// This method returns one of the several profiles depending on the caller's thread.
        /// </para>
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public ClassParallelPermissionProfile GetParallelPermissionProfileForCurrentClassAndThread(string className)
        {
            if (!ClassParallelPermissions.TryGetValue(className, out var perm))
            {
                return null;
            }
            return Thread.CurrentThread.IsThreadPoolThread ? perm.ThreadPoolProfile : perm.NormalProfile;
        }

        public void _ParseDevelopmentSwitches()
        {
            DevelopmentSwitches = new List<KeyValuePair<string, string>>();
            XmlNodeList nodeList = _xmlDoc.SelectNodes("//DevelopmentSwitches/KeyValuePair");
            foreach (XmlNode node in nodeList)
            {
                var keyAttr = node.Attributes["Key"];
                var valueAttr = node.Attributes["Value"];
                if (keyAttr is null ||
                    valueAttr is null)
                {
                    string strProblem = Text.CharArrayFilterUtility.RemoveControlAndHighChars(node.InnerText);
                    Logging.Sinks.LogMemorySink.DefaultInstance.Add(DateTime.Now,
                        "CONFIG WARNING: Cannot parse " + strProblem);
                }
                else
                {
                    DevelopmentSwitches.Add(new KeyValuePair<string, string>(keyAttr.Value, valueAttr.Value));
                }
            }
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
