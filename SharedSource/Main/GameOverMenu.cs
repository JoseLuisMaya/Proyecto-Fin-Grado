using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;

namespace TFG
{
    public class GameOverMenu : Scene
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
                Text = "Game Over",

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

        //Método para iniciar el juego
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

