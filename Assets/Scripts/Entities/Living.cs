using System;
using System.Collections.Generic;
using Abilities;
using Sensors;
using UnityEngine;
using UnityEngine.UI;
using UnitySharpNEAT.SharpNEAT.Genomes.Neat;
using UnitySharpNEAT.SharpNEAT.Phenomes.NeuralNets.AcyclicNetwork;
using UnitySharpNEAT.SharpNEAT.Utility;

namespace Entities
{
    // Represents a living entity
    public class Living : Entity<Living>
    {
        private FastRandom _fr = new();

        #region unity component references

        public HealthBarBehavior healthBarBehavior;
        public Slider healthBar;
        [NonSerialized] public CircleCollider2D C;

        public Rigidbody2D Rb { get; private set; }
        public SpriteRenderer S { get; private set; }

        #endregion unity component references

        #region team-provided properties

        private Color _color;
        [NonSerialized] public readonly List<ISense> Senses = new();
        [NonSerialized] public readonly List<IAbility> Abilities = new();
        [NonSerialized] public readonly List<ILivingCollisionHandler> CollisionHandlers = new();
        public EntityManager<TreeBehavior> TreeManager;

        #endregion team-provided properties

        #region SharpNEAT properties

        public FastAcyclicNetwork BlackBox;
        [NonSerialized] public NeatGenome Genome;

        #endregion SharpNEAT properties

        #region settings

        [Header("Movement")] public float speed = 5f;
        public float turnSpeed = 180f;

        [Header("Passive Factors")] [SerializeField]
        private float hungerDamagePerSecond = 1 / 40f;

        [SerializeField] private float eatingHealthPerSecond = 1 / 5f;

        #endregion settings

        #region state properties

        private bool _initialized;
        private bool _isActive = true;
        [NonSerialized] public float Health = 1;
        [NonSerialized] public bool TouchedThisTick;
        [NonSerialized] public bool AteThisTick;
        [NonSerialized] public float Score;

        #endregion state properties


        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                if (S)
                {
                    S.color = value;
                }
            }
        }

        // public float Score
        // {
        //     get => _score;
        // }

        public float Age { get; private set; }

        public void Start()
        {
            Rb = GetComponent<Rigidbody2D>();
            C = GetComponent<CircleCollider2D>();
            S = GetComponent<SpriteRenderer>();
            S.color = Color;
            _initialized = true;
        }

        public void FixedUpdate()
        {
            if (!_isActive || !_initialized) return;
            if (Health <= 0)
            {
                Deactivate();
                return;
            }

            Age += Time.fixedDeltaTime;
            Health -= hungerDamagePerSecond * Time.fixedDeltaTime;
            Score += Time.fixedDeltaTime;
            AteThisTick = false;

            // handle collisions first, since they will update state properties for use in senses
            var contacts = new Collider2D[500];
            var num = Rb.GetContacts(contacts);

            foreach (var handler in CollisionHandlers)
            {
                handler.HandleCollisions(this, contacts, num);
            }

            // store inputs in first NN layer
            var offset = 0;
            foreach (var sense in Senses)
            {
                sense.Report(this, offset);
                offset += sense.NodesRequired();
            }

            // feed-forward inputs to NN
            BlackBox.Activate();

            // perform abilities using neural network outputs
            offset = 0;
            foreach (var ability in Abilities)
            {
                ability.Perform(this, offset);
                offset += ability.NodesRequired();
            }


            // if (num > 0) TouchedThisTick = true;
            // for (var i = 0; i < num; i++)
            // {
            //     var c = contacts[i];
            //     // if c is a tree, eat it
            //     var isTree = treeManager.HasEntity(c.name);
            //     if (isTree)
            //     {
            //         var tree = treeManager.GetEntity(c.name);
            //         tree.Bite();
            //         AteThisTick = true;
            //         _health += eatingHealthPerSecond * Time.fixedDeltaTime;
            //         if (_health > 1) _health = 1;
            //         _score += 10;
            //     }
            // }

            healthBar.value = Health;
        }

        public float GetScore()
        {
            return Score;
        }

        public void Deactivate()
        {
            Rb.simulated = C.enabled = S.enabled = _isActive = false;
            healthBarBehavior.Disable();
        }

        public void Activate()
        {
            Rb.simulated = C.enabled = S.enabled = _isActive = true;
        }

        public void Reset()
        {
            Score = 0;
            Health = 1;
        }
    }
}