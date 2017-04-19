using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Framework;

namespace TFG
{
    [DataContract]
    class Life : Behavior
    {
        public bool destroyOnDead;
        public Entity deadPrefab;
        private float maxLife;
        private float currentLife;

        protected override void Initialize()
        {
            base.Initialize();
            maxLife = 100;
            currentLife = 100;

        }
        protected override void Update(TimeSpan gameTime)
        {
            
        }

        public float CurrentLife {
            get
            {
                return currentLife;
            }
            set
            {
                currentLife = value;
                if(currentLife > maxLife)
                {
                    currentLife = maxLife;
                }
                else if (currentLife <= 0)
                {
 //                   Dead();
                }
            }
        }

        public void SetInitialLife(float _life)
        {
            maxLife = _life;
            currentLife = _life;
        }

        public void SetDamage()
        {
            SetDamage(maxLife);
        }

        public void SetDamage(float damage)
        {
            CurrentLife -= damage;
        }

        public void SetTreat()
        {
            SetTreat(maxLife);
        }

        public void SetTreat(float treat)
        {
            CurrentLife += treat;
        }

       
    }
}
