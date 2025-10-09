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
        static Color[] pallete = new Color[64];
        static Color[] rawframebuffer;
        static Texture2D framebuffertexture;

        public static int winw = 128;
        public static int winh = 96;

        static public void CreateWindow()
        {
            // gray / white
            pallete[0] = new Color(0, 0, 0, 255);
            pallete[1] = new Color(32, 32, 32, 255);
            pallete[2] = new Color(64, 64, 64, 255);
            pallete[3] = new Color(96, 96, 96, 255);
            pallete[4] = new Color(128, 128, 128, 255);
            pallete[5] = new Color(160, 160, 160, 255);
            pallete[6] = new Color(200, 200, 200, 255);
            pallete[7] = new Color(255, 255, 255, 255);

            // red
            pallete[8] = new Color(16, 0, 0, 255);
            pallete[9] = new Color(32, 0, 0, 255);
            pallete[10] = new Color(64, 0, 0, 255);
            pallete[11] = new Color(96, 0, 0, 255);
            pallete[12] = new Color(128, 0, 0, 255);
            pallete[13] = new Color(160, 0, 0, 255);
            pallete[14] = new Color(200, 0, 0, 255);
            pallete[15] = new Color(255, 0, 0, 255);

            // green
            pallete[16] = new Color(0, 16, 0, 255);
            pallete[17] = new Color(0, 32, 0, 255);
            pallete[18] = new Color(0, 64, 0, 255);
            pallete[19] = new Color(0, 96, 0, 255);
            pallete[20] = new Color(0, 128, 0, 255);
            pallete[21] = new Color(0, 160, 0, 255);
            pallete[22] = new Color(0, 200, 0, 255);
            pallete[23] = new Color(0, 255, 0, 255);

            // blue
            pallete[24] = new Color(0, 0, 16, 255);
            pallete[25] = new Color(0, 0, 32, 255);
            pallete[26] = new Color(0, 0, 64, 255);
            pallete[27] = new Color(0, 0, 96, 255);
            pallete[28] = new Color(0, 0, 128, 255);
            pallete[29] = new Color(0, 0, 160, 255);
            pallete[30] = new Color(0, 0, 200, 255);
            pallete[31] = new Color(0, 0, 255, 255);

            // yellow
            pallete[32] = new Color(16, 16, 0, 255);
            pallete[33] = new Color(32, 32, 0, 255);
            pallete[34] = new Color(64, 64, 0, 255);
            pallete[35] = new Color(96, 96, 0, 255);
            pallete[36] = new Color(128, 128, 0, 255);
            pallete[37] = new Color(160, 160, 0, 255);
            pallete[38] = new Color(200, 200, 0, 255);
            pallete[39] = new Color(255, 255, 0, 255);

            // magenta / purple
            pallete[40] = new Color(16, 0, 16, 255);
            pallete[41] = new Color(32, 0, 32, 255);
            pallete[42] = new Color(64, 0, 64, 255);
            pallete[43] = new Color(96, 0, 96, 255);
            pallete[44] = new Color(128, 0, 128, 255);
            pallete[45] = new Color(160, 0, 160, 255);
            pallete[46] = new Color(200, 0, 200, 255);
            pallete[47] = new Color(255, 0, 255, 255);

            // cyan / teal
            pallete[48] = new Color(0, 16, 16, 255);
            pallete[49] = new Color(0, 32, 32, 255);
            pallete[50] = new Color(0, 64, 64, 255);
            pallete[51] = new Color(0, 96, 96, 255);
            pallete[52] = new Color(0, 128, 128, 255);
            pallete[53] = new Color(0, 160, 160, 255);
            pallete[54] = new Color(0, 200, 200, 255);
            pallete[55] = new Color(0, 255, 255, 255);

            // orange (new!)
            pallete[56] = new Color(16, 8, 0, 255);
            pallete[57] = new Color(48, 16, 0, 255);
            pallete[58] = new Color(80, 32, 0, 255);
            pallete[59] = new Color(112, 48, 0, 255);
            pallete[60] = new Color(160, 64, 0, 255);
            pallete[61] = new Color(192, 80, 0, 255);
            pallete[62] = new Color(224, 96, 0, 255);
            pallete[63] = new Color(255, 128, 0, 255);



            Raylib.InitWindow(winw * 8, winh * 8, "DJASM PROGRAM");
            rawframebuffer = new Color[winw * winh];


            framebuffertexture = Raylib.LoadTextureFromImage(Raylib.GenImageColor(winw, winh, Color.Black));
        }

        static public void UpdateScreen()
        {

            if (!Raylib.WindowShouldClose())
            {
                for (int i = 0; i < winw * winh; i++)
                {
                    byte colorIndex = (byte)Program.RAM[i];
                    rawframebuffer[i] = pallete[colorIndex];
                }



                

                unsafe
                {
                    if (rawframebuffer.Length != WindowCreation.winw * WindowCreation.winh)
                    {
                        rawframebuffer = new Color[WindowCreation.winw * WindowCreation.winh];
                    }
                    
                    fixed (Color* ptr = rawframebuffer)
                    {
                        Raylib.UpdateTexture(framebuffertexture, ptr);
                    }
                }
                




                Raylib.UpdateTexture(framebuffertexture, rawframebuffer);

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                Raylib.DrawTexturePro(framebuffertexture, new Rectangle(0, 0, winw, winh), new Rectangle(0, 0, winw * 8, winh * 8), new System.Numerics.Vector2(0, 0), 0f, Color.White);

                Raylib.EndDrawing();
            }
            else
            {
                Raylib.UnloadTexture(framebuffertexture);
                Raylib.CloseWindow();
            }
        }


    }

    
}
