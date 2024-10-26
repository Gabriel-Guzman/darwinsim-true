using Entities;

namespace Sensors
{
    public interface ISense
    {
        public int NodesRequired();
        public void Report(Living player, int offset);
    }
}