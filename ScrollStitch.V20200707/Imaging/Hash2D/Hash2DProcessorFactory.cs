using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Hash2D
{
    using TestClassConfig = Config.TestClassConfig;
    using ConfigData = Config.Data;

    public static class Hash2DProcessorFactory
    {
        public static Hash2DProcessor CreateFromConfigByName(string specName)
        {
            var cfgSpec = TestClassConfig.DefaultInstance.Hash2DSpecs.Find(
                (argSpec) => specName.Equals(argSpec.Name));
            return CreateFromConfig(cfgSpec);
        }

        public static Hash2DProcessor CreateFromConfig(ConfigData.Hash2DSpec spec)
        {
            Hash2DProcessor processor = new Hash2DProcessor();
            foreach (var cfgPass in spec.Passes)
            {
                processor.AddStage(
                    direction: _ParseDirection(cfgPass.Direction), 
                    windowSize: cfgPass.WindowSize, 
                    skipStep: cfgPass.SkipStep, 
                    fillValue: cfgPass.FillValue);
            }
            return processor;
        }

        private static Direction _ParseDirection(string s)
        {
            return (Direction)Enum.Parse(typeof(Direction), s);
        }
    }
}
