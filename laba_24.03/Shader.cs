using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace laba_24._03
{
    internal class Shader
    {
        public int shaderHandle;

        public Shader() { }

        public void LoadShader()
        {
            shaderHandle = GL.CreateProgram();
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, LoadShaderSource("shader.vert"));
            GL.CompileShader(vertexShader);


            //////////////////////
            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int success1);
            if (success1 == 0)
            {
                string infoLog = GL.GetShaderInfoLog(vertexShader);
                Console.WriteLine(infoLog);
            }
            //////////////////////

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, LoadShaderSource("shader.frag"));
            GL.CompileShader(fragmentShader);

            //////////////////////

            GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out int success2);
            if (success2 == 0)
            {
                string infoLog = GL.GetShaderInfoLog(fragmentShader);
                Console.WriteLine(infoLog);
            }
            ///////////////////////


            GL.AttachShader(shaderHandle, vertexShader);
            GL.AttachShader(shaderHandle, fragmentShader);

            GL.LinkProgram(shaderHandle);
        }

        public static string LoadShaderSource(string filepath)
        {
            string shadSour = "";
            try
            {
                using (StreamReader reader = new StreamReader("../../../Shaders/" + filepath))
                {
                    shadSour = reader.ReadToEnd();

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to load shader source file: " + ex.Message);
            }
            return shadSour;
        }

        public void UseShader()
        {
            GL.UseProgram(shaderHandle);
        }
        
        public void DeleteShader()
        {
            GL.DeleteProgram(shaderHandle);
        }
    }
}
