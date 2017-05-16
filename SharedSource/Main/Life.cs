using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace TFG
{
    [DataContract]
    class Life : Behavior
    {
        public bool destroyOnDead;
        public bool deadPrefab;
        private float maxLife;
        private float currentLife;
        private int count = 0;

        protected override void Initialize()
        {
            base.Initialize();
            maxLife = 100;
            currentLife = 30;
            destroyOnDead = true;
            deadPrefab = true;

        }
        protected override void Update(TimeSpan gameTime)
        {
            
        }

        //Método para obtener la vida actual del personaje
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
                    Dead();
                }
            }
        }

        //Método para inicializar la vida
        public void SetInitialLife(float _life)
        {
            maxLife = _life;
            currentLife = _life;
        }

        //Método para dañar con el máximo daño
        public void SetDamage()
        {
            SetDamage(maxLife);
        }

        //Método para dañar con una cantidad concreta
        public void SetDamage(float damage)
        {
            CurrentLife -= damage;
        }

        //Método para curar completamente
        public void SetTreat()
        {
            SetTreat(maxLife);
        }

        //Método para curar con con una cantidad concreta
        public void SetTreat(float treat)
        {
            CurrentLife += treat;
        }

        //Método para eliminar a un personaje cuando muere
        public void Dead()
        {
            //Creación del prefab cuando muere un personaje
            if (deadPrefab == true)
            {
                  var newEntity = this.EntityManager.Instantiate(WaveContent.Assets.Prefast.Tombstone);
                newEntity.Name = newEntity.Name + this.Owner.Name;
                newEntity.FindComponent<Transform3D>().Position = this.Owner.FindComponent<Transform3D>().Position;
                EntityManager.Add(newEntity);
                deadPrefab = false;
                count = count + 1;
               
            }
            if (destroyOnDead)
            {
                //Eliminamos al personaje al morir
                if(this.Owner.Name != null)
                {
                    EntityManager.Remove(this.Owner.Name);
                    //this.Owner.Enabled = false;
                }
                
            }
        }

    }
}
