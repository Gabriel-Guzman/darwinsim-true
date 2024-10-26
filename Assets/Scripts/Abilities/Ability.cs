using Entities;
using Unity.VisualScripting;

namespace Abilities
{
    public interface IAbility
    {
        public int NodesRequired();
        public void Perform(Living player, int offset);
    }
}