using ScrollStitch.V20200707.Collections;
using ScrollStitch.V20200707.Data;
using ScrollStitch.V20200707.Spatial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Tracking
{
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

        public T3GridStats_OneVotePerCell FirstStageCellVotes { get; private set; }

        public T3Classifier Classifier { get; private set; }

        public T3Filter Filter { get; private set; }

        public T3HashPoints SecondStageHashPoints { get; private set; }

        public T3Movements SecondStageMovements { get; private set; }

        public T3GridStats_OneVotePerCell SecondStageCellVotes { get; private set; }

        public T3Main(ImageManager imageManager, IEnumerable<int> imageKeys, int mainImageKey, T3ClassifierThreshold threshold, Size approxCellSize)
        {
            ImageManager = imageManager;
            ImageKeys = new UniqueList<int>(imageKeys);
            if (ImageKeys.Count != 3)
            {
                throw new ArgumentException(nameof(imageKeys));
            }
            MainImageKey = mainImageKey;
            if (ImageKeys.IndexOf(MainImageKey) < 0)
            {
                throw new ArgumentException(nameof(mainImageKey));
            }
            FilterThreshold = threshold;
            ApproxCellSize = approxCellSize;
        }

        public void Process()
        {
            _ProcessFirstStage();
            _ProcessSecondStage();
        }

        private void _ProcessFirstStage()
        {
            FirstStageHashPoints = T3HashPoints.Factory.Create(ImageManager, ImageKeys);
            FirstStageMovements = T3Movements.Factory.Create(FirstStageHashPoints);
            MainImageSize = ImageManager.ImageSizes[MainImageKey];
            MainGrid = Grid.Factory.CreateApproxCellSize(MainImageSize, ApproxCellSize);
            var tempGridStat = new T3GridStats(FirstStageMovements, MainImageKey, MainGrid);
            FirstStageCellVotes = new T3GridStats_OneVotePerCell(tempGridStat);
            tempGridStat.Add(FirstStageCellVotes);
            tempGridStat.Process();
        }

        private void _ProcessSecondStage()
        {
            Classifier = new T3Classifier()
            { 
                MovementsClass = FirstStageMovements,
                LabelCellCountsClass = FirstStageCellVotes
            };
            Filter = new T3Filter(FirstStageHashPoints, FirstStageMovements, Classifier);
            Filter.Process();
            SecondStageHashPoints = Filter.NewHashPoints;
            SecondStageMovements = Filter.NewMovements;
            var tempGridStat = new T3GridStats(SecondStageMovements, MainImageKey, MainGrid);
            SecondStageCellVotes = new T3GridStats_OneVotePerCell(tempGridStat);
            tempGridStat.Add(SecondStageCellVotes);
            tempGridStat.Process();
        }
    }
}
