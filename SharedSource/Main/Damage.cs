using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Framework;
using WaveEngine.Framework.Physics3D;

namespace TFG
{
    [DataContract]
    class Damage : Behavior
    {
        private Entity ballEntity;
        public float damage = 10;

        protected override void Update(TimeSpan gameTime)
        {
            this.ballEntity = this.EntityManager.Find("ball1");

            //En caso de que ball1 no exista comprobar si existe ballFrag1 (Proyectil fragmentado)
            if (this.ballEntity == null)
            {
                this.ballEntity = EntityManager.Find("ballFrag1");
                if (ballEntity == null)
                {
                    return;
                }

            }
            //Guardar el delineado del colider de el proyectil
            var collider = this.ballEntity.FindComponent<SphereCollider3D>().BoundingSphere;

            //Obtener todos los objetos con un tag definido
            var tags = EntityManager.FindAllByTag("worm");

            //Para cada objeto con dicho tag, comprobar si ha colisionado el proyectil con dicho objeto
            foreach (Entity tagi in tags)
            {
                var wormBounding = tagi.FindComponent<BoxCollider3D>().BoundingBox;
                if (wormBounding == null)
                {
                    return;
                }
                else
                {
                    //En caso de que colisione y tenga un componente de vida dañar al personaje
                    if (wormBounding.Intersects(ref collider))
                    {
                        var Life = tagi.FindComponent<Life>();
                        if (Life == null)
                        {
                            return;
                        }
                        else
                        {
                            Life.SetDamage(damage);
                        }

                    }

                }

            }
        }
    }
}
