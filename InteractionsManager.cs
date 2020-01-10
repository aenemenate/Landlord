using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    static class InteractionsManager
    {
        // block vs block interactions

        /* I plan to have a function here called void HitBlock(this Block block, Block otherBlock);
         * if either block breaks, it will have a property called "flaggedForDeletion" set to true
         * setting that property true will also set DeletionManager.BlocksFlagged to true. When this 
         * is set, DeletionManager will go through the map and delete every block marked flaggedForDeletion.
         * When blocks are updated to have drops, it will also trigger drops for every block.
         * For now, it will drop ore for ore walls, stones for stone walls, and 
         */
    }
}
