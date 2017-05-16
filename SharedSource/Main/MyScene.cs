#region Using Statements
using System;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Resources;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
#endregion

namespace TFG
{
    public class MyScene : Scene
    {
        // Los diferentes estados del juego
        public enum State
        {
            Init,
            Finish,
            Game,
            GameOver,
            Change,
            Win
        }
        public State CurrentState = State.Init;
        public State lastState = State.Game;

        public TextBlock textblockInit;
        public TextBlock textblockLife;



        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.MyScene);

            int offsetTop = 50;

            //Texto para mostrar el tiempo de cada turno
            textblockInit = new TextBlock("TextInit")
            {
                Margin = new Thickness((WaveServices.Platform.ScreenWidth) - 150, offsetTop, 0, 0),
                // FontPath = "Content/Font/Calisto MT.wpk",
                Text = "",
                Height = 130,
                IsVisible = false,
            };

            //Texto para mostrar la vida del personaje
            textblockLife = new TextBlock("TextLife")
            {
                Margin = new Thickness((WaveServices.Platform.ScreenWidth ) - WaveServices.Platform.ScreenWidth +150, offsetTop, 0, 0),
                Text = "",
                Height = 130,
                IsVisible = false,
            };

            var a = EntityManager.FindAllByTag("worm");
            EntityManager.Add(textblockInit);
            EntityManager.Add(textblockLife);

            //Añadimos el manejador de escenas
            AddSceneBehavior(new MySceneBehavior(), SceneBehavior.Order.PostUpdate);
        }

       
    }
}
