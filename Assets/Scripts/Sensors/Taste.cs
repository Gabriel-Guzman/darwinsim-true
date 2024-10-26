using Entities;

namespace Sensors
{
    public class Taste : ISense
    {
        public int NodesRequired()
        {
            return 1;
        }

        public void Report(Living player, int offset)
        {
            if (player.AteThisTick)
            {
                player.BlackBox.InputSignalArray[offset] = 1;
            }
            else
            {
                player.BlackBox.InputSignalArray[offset] = 0;
            }
        }
    }
}