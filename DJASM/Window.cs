using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DJASM;
using Raylib_cs;

namespace DJASM
{
    class WindowCreation
    {
         static Color[] pallete = new Color[32];

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


            Raylib.InitWindow(1792, 1280, "DJASM PROGRAM");
            Raylib.SetTargetFPS(1000);
        }

        static public void UpdateScreen()
        {
            
            int winw = 224;
            int winh = 160;
            Raylib.BeginDrawing();


            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            for (int y = 0; y < winh; y++)
            {
                for (int x = 0; x < winw; x++)
                {
                    int addr = y * winw + x; // map (x,y) → RAM index
                    byte colorIndex = Program.RAM[addr];
                    Raylib.DrawRectangle(x * 8, y * 8, 8, 8, pallete[colorIndex]);
                }
            }

            Raylib.EndDrawing();
        }
    }
}
