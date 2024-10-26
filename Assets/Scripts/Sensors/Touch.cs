using Entities;
using UnityEngine;

namespace Sensors
{
    public class Touch : ISense
    {
        public int NodesRequired()
        {
            return 1;
        }

        public void Report(Living player, int offset)
        {
            var contacts = new Collider2D[20];
            var num = player.Rb.GetContacts(contacts);

            if (num > 0) player.BlackBox.InputSignalArray[offset] = 1;
            else player.BlackBox.InputSignalArray[offset] = 0;
        }
    }
}