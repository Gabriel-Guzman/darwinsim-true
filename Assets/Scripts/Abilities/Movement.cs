using Entities;
using UnityEngine;
using UnitySharpNEAT.SharpNEAT.Phenomes;

namespace Abilities
{
    public class Movement : IAbility
    {
        public int NodesRequired()
        {
            return 2;
        }

        public void Perform(Living player, int offset)
        {
            var outputSignalArray = player.BlackBox.OutputSignalArray;

            var turnSpeed = player.turnSpeed;

            var steer = (float)outputSignalArray[offset];
            var gas = (float)outputSignalArray[offset + 1];

            var speed = player.speed;
            var t = player.transform;
            var rb = player.Rb;
            var moveForce = t.right * (speed * gas);
            if (rb.velocity.magnitude > speed) moveForce = Vector3.zero;

            player.Health -= 0;//gas * Time.fixedDeltaTime * 0.1f;
            
            if (moveForce.magnitude < 0.1f) rb.velocity = Vector3.zero;
            else rb.AddForce(moveForce);
            
            // Calculate the target rotation based on the steer value
            var targetRotation = rb.transform.rotation * Quaternion.Euler(0, 0, (steer*2 - 1) * 180);

            // Smoothly interpolate between the current rotation and the target rotation or stop the rotation
            if (steer is < 0.6f and > 0.4f) rb.angularVelocity = 0;
            else rb.MoveRotation(Quaternion.RotateTowards(t.rotation, targetRotation, turnSpeed * Time.deltaTime));
        }
    }
}