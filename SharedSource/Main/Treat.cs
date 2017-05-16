using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Framework;
using WaveEngine.Framework.Physics3D;

namespace TFG
{
    [DataContract]
    class Treat : Behavior
    {
        public float treat;
        private Entity treatObject;

        protected override void Initialize()
        {
            base.Initialize();
            if(this.Owner.Name != null)
            {
                treatObject = this.EntityManager.Find(this.Owner.Name);

            }
            treat = 30;

        }
        protected override void Update(TimeSpan gameTime)
        {
            if (treatObject==null)
            {
                return;
            }
            
           
            //Guardar el delineado del colider del objeto de curación
            var collider = this.treatObject.FindComponent<BoxCollider3D>().BoundingBox;

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
                    //En caso de que colisione y tenga un componente de vida, curar al personaje
                    if (collider.Intersects(ref wormBounding))
                    {
                        var Life = tagi.FindComponent<Life>();
                        if (Life == null)
                        {
                            return;
                        }
                        else
                        {
                            Life.SetTreat(this.treat);
                            EntityManager.Remove(treatObject);
                            return;
                        }

                    }

                }

            }

        }
    }
}
