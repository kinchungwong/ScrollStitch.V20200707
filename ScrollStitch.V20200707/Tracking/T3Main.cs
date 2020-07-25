using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Tracking
{
    using ScrollStitch.V20200707.Collections;
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Spatial;

    /// <summary>
    /// The main algorithm for three-image trajectory segmentation.
    /// 
    /// <para>
    /// This class implements the classification and filtering so that only the relevant trajectories 
    /// are retained.
    /// </para>
    /// </summary>
    public class T3Main
    {
        public ImageManager ImageManager { get; }

        public UniqueList<int> ImageKeys { get; }

        public int MainImageKey { get; }

        public T3ClassifierThreshold FilterThreshold { get; }

        public Size ApproxCellSize { get; }

        public T3HashPoints FirstStageHashPoints { get; private set; }

        public T3Movements FirstStageMovements { get; private set; }

        public Size MainImageSize { get; private set; }

        public Grid MainGrid { get; private set; }

        public T3CellLabels FirstStageCellLabels { get; private set; }

        public T3Classifier Classifier { get; private set; }

        public T3Filter Filter { get; private set; }

        public T3HashPoints SecondStageHashPoints { get; private set; }

        public T3Movements SecondStageMovements { get; private set; }

        public T3CellLabels SecondStageCellLabels { get; private set; }

        public T3Main(ImageManager imageManager, UniqueList<int> imageKeys, int mainImageKey, T3ClassifierThreshold threshold, Size approxCellSize)
        {
            _CtorValidateImageKeys(imageKeys, mainImageKey);
            ImageManager = imageManager;
            ImageKeys = imageKeys;
            MainImageKey = mainImageKey;
            FilterThreshold = threshold;
            ApproxCellSize = approxCellSize;
        }

        private static void _CtorValidateImageKeys(UniqueList<int> imageKeys, int mainImageKey)
        {
            if (imageKeys.Count != 3)
            {
                throw new ArgumentException(nameof(imageKeys));
            }
            if (imageKeys.IndexOf(mainImageKey) < 0)
            {
                throw new ArgumentException(nameof(mainImageKey));
            }
        }

        public void Process()
        {
            _PopulateImageInfo();
            _ProcessFirstStage();
            _ProcessSecondStage();
        }

        private void _PopulateImageInfo()
        {
            MainImageSize = ImageManager.ImageSizes[MainImageKey];
            MainGrid = Grid.Factory.CreateApproxCellSize(MainImageSize, ApproxCellSize);
        }

        private void _ProcessFirstStage()
        {
            FirstStageHashPoints = T3HashPoints.Factory.Create(ImageManager, ImageKeys);
            FirstStageMovements = T3Movements.Factory.Create(FirstStageHashPoints);
            FirstStageCellLabels = T3CellLabels.Factory.Create(FirstStageMovements, MainImageKey, MainGrid);
        }

        private void _ProcessSecondStage()
        {
            Classifier = new T3Classifier(FirstStageMovements, FirstStageCellLabels, FilterThreshold);
            Filter = new T3Filter(FirstStageHashPoints, FirstStageMovements, Classifier);
            Filter.Process();
            SecondStageHashPoints = Filter.NewHashPoints;
            SecondStageMovements = Filter.NewMovements;
            SecondStageCellLabels = T3CellLabels.Factory.Create(SecondStageMovements, MainImageKey, MainGrid);
        }
    }
}
