using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using DJASM;
using Raylib_cs;

namespace DJASM
{
    class WindowCreation
    {
        static Color[] pallete = new Color[32];
        static Color[] rawframebuffer;
        static Texture2D framebuffertexture;

        public static int winw = 128;
        static int winh = 96;

        static public void CreateWindow()
        {
            //gray or "white" colors
            pallete[0] = new Color(0, 0, 0, 255);
            pallete[1] = new Color(64, 64, 64, 255);
            pallete[2] = new Color(96, 96, 96, 255);
            pallete[3] = new Color(255, 255, 255, 255);
            //red colors
            pallete[4] = new Color(32, 0, 0, 255);
            pallete[5] = new Color(64, 0, 0, 255);
            pallete[6] = new Color(96, 0, 0, 255);
            pallete[7] = new Color(255, 0, 0, 255);
            //blue colors
            pallete[8] = new Color(0, 32, 0, 255);
            pallete[9] = new Color(0, 64, 0, 255);
            pallete[10] = new Color(0, 96, 0, 255);
            pallete[11] = new Color(0, 255, 0, 255);
            //green colors
            pallete[12] = new Color(0, 0, 32, 255);
            pallete[13] = new Color(0, 0, 64, 255);
            pallete[14] = new Color(0, 0, 96, 255);
            pallete[15] = new Color(0, 0, 255, 255);
            //yellow colors
            pallete[16] = new Color(32, 0, 32, 255);
            pallete[17] = new Color(64, 0, 64, 255);
            pallete[18] = new Color(96, 0, 96, 255);
            pallete[19] = new Color(255, 0, 255, 255);
            //magenta colors
            pallete[20] = new Color(32, 32, 0, 255);
            pallete[21] = new Color(64, 64, 0, 255);
            pallete[22] = new Color(96, 96, 0, 255);
            pallete[23] = new Color(255, 255, 0, 255);
            //teal colors
            pallete[24] = new Color(0, 64, 64, 255);
            pallete[25] = new Color(0, 64, 64, 255);
            pallete[26] = new Color(0, 96, 96, 255);
            pallete[27] = new Color(0, 255, 255, 255);
            //placeholder
            pallete[28] = new Color(0, 0, 0, 255);
            pallete[29] = new Color(64, 64, 64, 255);
            pallete[30] = new Color(96, 96, 96, 255);
            pallete[31] = new Color(255, 255, 255, 255);


            Raylib.InitWindow(winw * 8, winh * 8, "DJASM PROGRAM");
            rawframebuffer = new Color[winw * winh];


            framebuffertexture = Raylib.LoadTextureFromImage(Raylib.GenImageColor(winw, winh, Color.Black));
        }

        static public void UpdateScreen()
        {

            for(int i = 0; i < winw * winh; i++)
            {
                byte colorIndex = (byte)(Program.RAM[i] & 0b00011111);
                rawframebuffer[i] = pallete[colorIndex];
            }



            Color[] ptr = rawframebuffer;
            Raylib.UpdateTexture(framebuffertexture, ptr);

            


            Raylib.UpdateTexture(framebuffertexture, rawframebuffer);

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            Raylib.DrawTexturePro(framebuffertexture, new Rectangle(0, 0, winw, winh), new Rectangle(0, 0, winw * 8, winh * 8), new System.Numerics.Vector2(0, 0), 0f, Color.White);

            Raylib.EndDrawing();
        }


    }

    
}
