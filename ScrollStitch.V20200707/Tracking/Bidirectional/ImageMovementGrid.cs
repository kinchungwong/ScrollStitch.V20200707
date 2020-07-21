using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ======
//
// About this class
// This class is supposed to replace the set of classes ("ImagePairMovement", "ImagePairGridMovement") and related classes.
//
// ======

namespace ScrollStitch.V20200707.Tracking.Bidirectional
{
    using Data;
    using ScrollStitch.V20200707.Collections;
    using Spatial;

    public class ImageMovementGrid
    {
        /// <summary>
        /// The current image key, used as reference.
        /// </summary>
        public int ImageKey { get; }

        /// <summary>
        /// The other image keys with which overlapped content has been detected.
        /// </summary>
        public UniqueList<int> PeerImageKeys { get; }

        /// <summary>
        /// The grid used with the current image.
        /// </summary>
        public Grid Grid { get; }

        /// <summary>
        /// Current image size.
        /// </summary>
        public Size ImageSize => Grid.InputSize;

        /// <summary>
        /// 
        /// </summary>
        public GridArray<Movement[]> GridOfMovements { get; private set; }

        public ImageMovementGrid(int imageKey, IEnumerable<int> peerImageKeys, Grid grid)
        {
            ImageKey = imageKey;
            PeerImageKeys = new UniqueList<int>(peerImageKeys);
            Grid = grid;
            GridOfMovements = new GridArray<Movement[]>(grid);
        }

        /// <summary>
        /// Iterates through the list of movements from the current image to the peer image.
        /// </summary>
        /// <param name="peerImageKey"></param>
        /// <param name="rectMovementFunc"></param>
        public void ForEach(int peerImageKey, Action<Rect, Movement> rectMovementFunc)
        {
            GridOfMovements.ForEach(
                (CellIndex ci, Movement[] ms) =>
                {
                    if (ms is null) return;
                    Rect cr = Grid.GetCellRect(ci);
                    foreach (Movement m in ms)
                    {
                        rectMovementFunc(cr, m);
                    }
                });
        }
    }
}
