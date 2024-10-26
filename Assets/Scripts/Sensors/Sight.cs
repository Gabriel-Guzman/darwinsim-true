using Entities;
using UnityEngine;
using UnitySharpNEAT.SharpNEAT.Phenomes;
using UnitySharpNEAT.SharpNEAT.Phenomes.NeuralNets.AcyclicNetwork;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;

namespace Sensors
{
    public class Sight : ISense
    {
        private readonly float _fieldOfView;
        private readonly float _distance;
        private readonly int _rays;

        public Sight(float fov, float distance, int rays)
        {
            _fieldOfView = fov;
            _distance = distance;
            _rays = rays;
        }

        public int NodesRequired()
        {
            return _rays * 4;
        }

        private Vector2 GetWorldSpacePosition(Living player)
        {
            var c = player.C;
            var t = c.transform;
            return t.TransformPoint(c.offset) + t.right * (c.radius * t.lossyScale.z);
        }

        public void Report(Living player, int offset)
        {
            var p = player;
            var fov = _fieldOfView;
            var rays = _rays;
            var raySeparationAngle = fov / rays;
            var initialOffset = raySeparationAngle * (rays - 1) / 2;

            var blackBox = player.BlackBox;
            var inputSignalArray = blackBox.InputSignalArray;

            for (var i = 0; i < rays; i++)
            {
                var rotation = Quaternion.Euler(0, 0, -initialOffset + raySeparationAngle * i);

                var cTransform = p.C.transform;
                Vector2 direction = rotation * cTransform.right; 

                var results = new RaycastHit2D[10];
                var hits = p.C.Raycast(direction, results, _distance);
                
                if (hits < 0) continue;
                var hit = results[0];
                if (!hit.collider) continue;

                var endPoint = hit.point;
                Debug.DrawLine(p.C.transform.position, endPoint, Color.magenta, Time.fixedDeltaTime);

                var adjustedI = offset + i * 4;
                inputSignalArray[adjustedI] = hit.fraction;

                Color c;
                var go = hit.collider.gameObject;
                if (go.CompareTag("Tree"))
                {
                    c = Color.green;
                }
                else if (go.CompareTag("Player"))
                {
                    c = player.Manager.GetEntity(go.name).Color;
                }
                else
                {
                    c = Color.blue;
                }

                inputSignalArray[adjustedI + 1] = c.r;
                inputSignalArray[adjustedI + 2] = c.g;
                inputSignalArray[adjustedI + 3] = c.b;
            }
        }
    }
}