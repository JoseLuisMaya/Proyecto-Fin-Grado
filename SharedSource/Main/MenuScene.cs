using System;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Resources;
using WaveEngine.Framework.Services;
using WaveEngine.Common.Math;
using WaveEngine.Components.Cameras;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics3D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Materials;
using WaveEngine.Components.UI;
using WaveEngine.Framework.UI;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Components.Gestures;

namespace TFG
{
    public class MenuScene : Scene
    {
        public TextBlock textblockDescription;
        protected override void CreateScene()
        {


            //Título cabecera del menú
            textblockDescription = new TextBlock()
            {
                Margin = new Thickness((WaveServices.Platform.ScreenWidth) / 2 - 70, 150, 0, 0),
                FontPath = "Content/Fonts/Playbill_48.TTF.wpk",
                Foreground = Color.Gold,
                Text = "Game Win",

                Height = 250,
                Width = 250,
                IsVisible = true,
            };
            EntityManager.Add(textblockDescription);

            //Botón para iniciar el juego
            Button button = new Button("Init Game")
            {
                Margin = new Thickness((WaveServices.Platform.ScreenWidth) / 2 - 150, (WaveServices.Platform.ScreenHeight) / 2, 0, 0),
                Text = "Start",
                BackgroundImage = "Content/Assets/Textures/button.png",
                PressedBackgroundImage = "Content/Assets/Textures/buttonPress.jpg",
                Height = 50,
                Width = 100,

            };

            //Iniciar el juego al pulsar el botón
            button.Click += new EventHandler(InitGame_TouchPressed);

            EntityManager.Add(button.Entity);


            //Botón para salir del juego
            Button buttonExit = new Button("Exit Game")
            {

                Margin = new Thickness((WaveServices.Platform.ScreenWidth) / 2 + 70, (WaveServices.Platform.ScreenHeight) / 2, 0, 0),

                Text = "Exit",
                BackgroundImage = "Content/Assets/Textures/button.png",
                PressedBackgroundImage = "Content/Assets/Textures/buttonPress.jpg",
                Height = 50,
                Width = 100,
            };

            //Salir del juego al pulsar el botón
            buttonExit.Click += new EventHandler(ExitGame_TouchPressed);

            EntityManager.Add(buttonExit.Entity);
        }

        //Método para reiniciar el juego
        private void InitGame_TouchPressed(object sender, EventArgs e)
        {
            ScreenContext MyScene = new ScreenContext(new MyScene());
            WaveServices.ScreenContextManager.To(MyScene);
        }

        //Método para salir del juego
        private void ExitGame_TouchPressed(object sender, EventArgs e)
        {
            WaveServices.Platform.Exit();
        }
    }
}
