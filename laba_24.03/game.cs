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

using StbImageSharp;

namespace laba_24._03
{
    internal class game : GameWindow
    {

        bool click = false;
        int speed = 1;
        int xzSpeed = 5;
        float buffer = 0f;
        float angle = 0.25f;

        Vector3 topPosition = new Vector3(0f, 0f, 0f);
        Vector2 lastMoveMousePos;
        bool firstMoveInput = true;
        float moveSensitivity = 0.01f;

        float yRot = 0f;
        float xRot = 0f;
        int width, height;

        //параметры для отрисовки треуголки
        int VAO, VBO, EBO, textureVBO;
        Shader shaderProgram;
        Camera camera;
        int textureID;
        
         uint[] indices =
        {
            0, 1, 2,
            0, 2, 3, 
            0, 3, 4,
            0, 4, 5,

            0, 6, 7,
            0, 7, 8,
            0, 8, 9,
            0, 9, 5,

            0, 6, 10,
            0, 10, 11,
            0, 11, 12,
            0, 12, 13,

            0, 13, 16, 
            0, 16, 15,
            0, 15, 14,
            0, 14, 1,
            
            17, 1,2,
            17, 2,3,
            17, 3, 4,

            17, 4, 5,
            17, 5, 6,
            17, 6, 7,

            17, 7, 8,
            17, 8, 9,
            17, 9, 5,

            17, 6, 10,
            17, 10, 11,
            17, 11, 12,
            17, 12, 13,

            17, 13, 16,
            17, 16, 15,
            17, 15, 14,
            17, 14, 1,

        };
        
        List<Vector3> vertices = new List<Vector3>() 
        {
            new Vector3(0f,  -0.5f, 0f),

            //1 square 
            new Vector3( 0f,  18f, 20f), //on z (1)
            new Vector3( 9f,  18f, 18f),
            new Vector3( 14.5f,  18f, 14.5f), 
            new Vector3( 18f,  18f, 9f), 
            new Vector3( 20f, 18f, 0f), // on x

            // 2 square
            new Vector3( 0f,  18f, -20f), //on z (6)
            new Vector3( 9f,  18f, -18f),
            new Vector3( 14.5f,  18f, -14.5f),
            new Vector3( 18f,  18f, -9f), 
             // 3 square
            new Vector3( -9f,  18f, -18f), //(10)
            new Vector3( -14.5f,  18f, -14.5f),
            new Vector3( -18f,  18f, -9f),
            new Vector3( -20f, 18f, 0f), // on x

            // 4 square
            new Vector3( -9f,  18f, 18f), //(14)
            new Vector3( -14.5f,  18f, 14.5f),
            new Vector3( -18f,  18f, 9f),     
            
            //
            new Vector3(0f,  28f, 0f), //(17)
        };

        List<Vector2> texCoords = new List<Vector2>()
        {
            new Vector2(0.5f, 0.0f), //текстуры загружаются
        
            new Vector2(0.0f / 16.0f, 1.0f),    // Vertex 1
            new Vector2(1.0f / 16.0f, 1.0f),    // Vertex 2
            new Vector2(2.0f / 16.0f, 1.0f),    // Vertex 3
            new Vector2(3.0f / 16.0f, 1.0f),    // Vertex 4
            new Vector2(4.0f / 16.0f, 1.0f),    // Vertex 5
        
            new Vector2(5.0f / 16.0f, 1.0f),    // Vertex 6
            new Vector2(6.0f / 16.0f, 1.0f),    // Vertex 7
            new Vector2(7.0f / 16.0f, 1.0f),    // Vertex 8
            new Vector2(8.0f / 16.0f, 1.0f),    // Vertex 9
        
            new Vector2(9.0f / 16.0f, 1.0f),    // Vertex 10
            new Vector2(10.0f / 16.0f, 1.0f),   // Vertex 11
            new Vector2(11.0f / 16.0f, 1.0f),   // Vertex 12
            new Vector2(12.0f / 16.0f, 1.0f),   // Vertex 13
        
            new Vector2(13.0f / 16.0f, 1.0f),   // Vertex 14
            new Vector2(14.0f / 16.0f, 1.0f),   // Vertex 15
            new Vector2(15.0f / 16.0f, 1.0f),   // Vertex 16

            new Vector2(0.5f, 2f)
        };



        int VAO2, VBO2, EBO2, textureVBO2;
        int[] skyboxTextureIDs = new int[6];
        List<Vector3> vertices2 = new List<Vector3>
        {
            // Лицевая грань (+Z) скайбокс
            new Vector3(-800f, -800f,  800f), 
            new Vector3( 800f, -800f,  800f), 
            new Vector3( 800f,  800f,  800f), 
            new Vector3(-800f,  800f,  800f), 

            // Правая грань (+X)
            new Vector3( 800f, -800f,  800f), 
            new Vector3( 800f, -800f, -800f), 
            new Vector3( 800f,  800f, -800f), 
            new Vector3( 800f,  800f,  800f), 

            // Задняя грань (-Z)
            new Vector3( 800f, -800f, -800f), 
            new Vector3(-800f, -800f, -800f), 
            new Vector3(-800f,  800f, -800f), 
            new Vector3( 800f,  800f, -800f), 

            // Левая грань (-X)
            new Vector3(-800f, -800f, -800f), 
            new Vector3(-800f, -800f,  800f), 
            new Vector3(-800f,  800f,  800f), 
            new Vector3(-800f,  800f, -800f), 

            // Нижняя грань (-Y) 
            new Vector3(-800f, -0.5f,  800f), 
            new Vector3( 800f, -0.5f,  800f), 
            new Vector3( 800f, -0.5f, -800f), 
            new Vector3(-800f, -0.5f, -800f), 

            // Верхняя грань (+Y)
            new Vector3(-800f,  800f, -800f), 
            new Vector3( 800f,  800f, -800f), 
            new Vector3( 800f,  800f,  800f),
            new Vector3(-800f,  800f,  800f)  
        };

        List<Vector2> texCoords2 = new List<Vector2>()
        {
			new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),

			new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),

			new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),

			new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),

            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),

            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),
        };

        uint[] indices2 =
        {
            0, 1, 2,
            0, 2, 3,
            4, 5, 6,
            4, 6, 7,
            8, 9, 10,
            8, 10, 11,
            12, 13, 14,
            12, 14, 15,
            16, 17, 18,
            16, 18, 19,
            20, 21, 22, 
            20, 22, 23
        };

        public game(int width, int height) : base (GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            this.CenterWindow(new Vector2i(width, height));
            this.height = height;
            this.width = width;
        }
        protected override void OnLoad()
        {
            base.OnLoad();
            //ula
            VAO = GL.GenVertexArray();//Create VAO
            VBO = GL.GenBuffer();            //Create VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);//Bind the VBO 
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * Vector3.SizeInBytes, vertices.ToArray(), BufferUsageHint.StaticDraw);//Copy vertices data to the buffer
            GL.BindVertexArray(VAO); //Bind the VAO
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0); //Point a slot number 0
            GL.EnableVertexArrayAttrib(VAO, 0); //Enable the slot

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //Unbind the VBO

            EBO = GL.GenBuffer(); //Create EBO 
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            textureVBO = GL.GenBuffer();//Create, bind texture
            GL.BindBuffer(BufferTarget.ArrayBuffer, textureVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, texCoords.Count * Vector3.SizeInBytes, texCoords.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);            //Point a slot number 1
            GL.EnableVertexArrayAttrib(VAO, 1);            //Enable the slot

            GL.BindVertexArray(0);            //Delete everything

            // Texture Loading
            textureID = GL.GenTexture(); //Generate empty texture
            GL.ActiveTexture(TextureUnit.Texture0); //Activate the texture in the unit
            GL.BindTexture(TextureTarget.Texture2D, textureID); //Bind texture

            //Texture parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            //Load image
            StbImage.stbi_set_flip_vertically_on_load(1);
            ImageResult boxTexture = ImageResult.FromStream(File.OpenRead("../../../Textures/i.jpg"), ColorComponents.RedGreenBlueAlpha);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, boxTexture.Width, boxTexture.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, boxTexture.Data);

            //Unbind the texture
            GL.BindTexture(TextureTarget.Texture2D, 0);

            //skybox
            VAO2 = GL.GenVertexArray();//Create VAO
            VBO2 = GL.GenBuffer(); //Create VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO2);//Bind the VBO 
            GL.BufferData(BufferTarget.ArrayBuffer, vertices2.Count * Vector3.SizeInBytes, vertices2.ToArray(), BufferUsageHint.StaticDraw);//Copy vertices data to the buffer
            GL.BindVertexArray(VAO2); //Bind the VAO
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0); //Point a slot number 0
            GL.EnableVertexArrayAttrib(VAO2, 0); //Enable the slot

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //Unbind the VBO

            EBO2 = GL.GenBuffer(); //Create EBO 
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO2);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices2.Length * sizeof(uint), indices2, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            textureVBO2 = GL.GenBuffer(); //Create, bind texture
            GL.BindBuffer(BufferTarget.ArrayBuffer, textureVBO2);
            GL.BufferData(BufferTarget.ArrayBuffer, texCoords2.Count * Vector3.SizeInBytes, texCoords2.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);            //Point a slot number 1
            GL.EnableVertexArrayAttrib(VAO2, 1);            //Enable the slot

            GL.BindVertexArray(0);            //Delete everything

            shaderProgram = new Shader();
            shaderProgram.LoadShader();

            // Texture Loading
            GL.GenTextures(skyboxTextureIDs.Length, skyboxTextureIDs); //Generate empty texture

            string[] texturePaths = {
                "../../../Textures/skybox/posz.bmp",// +X
                "../../../Textures/skybox/negx.bmp",// -X
                "../../../Textures/skybox/negz.bmp",// +Z
                "../../../Textures/skybox/posx.bmp",// +X
                "../../../Textures/skybox/fl.jpg",// -Y
                "../../../Textures/skybox/posy.bmp",// +Y
            };

            StbImage.stbi_set_flip_vertically_on_load(0);

            for (int i = 0; i < skyboxTextureIDs.Length; i++)
            {
                GL.BindTexture(TextureTarget.Texture2D, skyboxTextureIDs[i]);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);                                                                                                                
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear); 

                ImageResult boxTexture2 = ImageResult.FromStream(File.OpenRead(texturePaths[i]), ColorComponents.RedGreenBlueAlpha);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, boxTexture2.Width, boxTexture2.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, boxTexture2.Data);

                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.Enable(EnableCap.DepthTest);

            camera = new Camera(width, height, (-1f,100f,-180f));
            CursorState = CursorState.Grabbed;

        }
        protected override void OnUnload()
        {
            base.OnUnload();
            GL.DeleteBuffer(VAO);
            GL.DeleteBuffer(VBO);
            GL.DeleteBuffer(EBO);
            GL.DeleteTexture(textureID);
            GL.DeleteBuffer(VAO2);
            GL.DeleteBuffer(VBO2);
            GL.DeleteBuffer(EBO2);
            for(int i = 0; i < skyboxTextureIDs.Length; i++)
                GL.DeleteTexture(skyboxTextureIDs[i]);
            shaderProgram.DeleteShader();
            
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.ClearColor(0.5f, 0.5f, 1f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            shaderProgram.UseShader();
            GL.BindTexture(TextureTarget.Texture2D, textureID);
            //Transformation 
            Matrix4 model1 = Matrix4.Identity; 
            
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjection();

            Matrix4 rotationY = Matrix4.CreateRotationY(yRot);
            Matrix4 rotationX = Matrix4.CreateRotationX(xRot);
            Matrix4 rotationZ = Matrix4.CreateRotationZ(xRot);

            Matrix4 translation = Matrix4.CreateTranslation(topPosition);
            model1 = rotationY * rotationX * rotationZ * translation;
            //великий путь на GPU
            int modelLocation = GL.GetUniformLocation(shaderProgram.shaderHandle, "model");
            int viewLocation = GL.GetUniformLocation(shaderProgram.shaderHandle, "view");
            int projectionLocation = GL.GetUniformLocation(shaderProgram.shaderHandle, "projection");

            GL.UniformMatrix4(modelLocation, true, ref model1);
            GL.UniformMatrix4(viewLocation, true, ref view);
            GL.UniformMatrix4(projectionLocation, true, ref projection);

            
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            //skybox
            GL.DepthFunc(DepthFunction.Lequal);

            Matrix4 model2 = Matrix4.Identity;
            Matrix4 translation2 = Matrix4.CreateTranslation(1.0f, 0f, -3f);
            model2 *= translation2;
            GL.UniformMatrix4(modelLocation, true, ref model2);
            GL.UniformMatrix4(projectionLocation, true, ref projection);

            GL.BindVertexArray(VAO2);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO2);
            for (int i = 0; i < skyboxTextureIDs.Length; i++) 
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, skyboxTextureIDs[i]);
                GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, i * 6 * sizeof(uint));
            }
            GL.DepthFunc(DepthFunction.Less);
            GL.BindVertexArray(0);

            Context.SwapBuffers();
            base.OnRenderFrame(args);
        }
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            MouseState mouse = MouseState;
            KeyboardState input = KeyboardState;

            if (KeyboardState.IsKeyDown(Keys.LeftAlt) && click)
            {
                if (firstMoveInput)
                {
                    firstMoveInput = false;
                
                    lastMoveMousePos = new Vector2(1f, 1f);
                    lastMoveMousePos = new Vector2(mouse.X, mouse.Y);
                }
                else
                {
                    var deltaX = mouse.X - lastMoveMousePos.X;
                    var deltaY = mouse.Y - lastMoveMousePos.Y;
                    lastMoveMousePos = new Vector2(mouse.X, mouse.Y);
                
                    topPosition.X -= deltaX * moveSensitivity;
                    topPosition.Z -= deltaY * moveSensitivity;
                }
            }
            else
            {
                firstMoveInput = true;
            }

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }
            if (KeyboardState.IsKeyPressed(Keys.M))
            {
                click = !click;
            }
            switch (click)
            {
                case true:
                    yRot += speed * (float)args.Time;
                    buffer += (float)args.Time;
                    xRot = (float)Math.Sin(buffer * xzSpeed) * angle;
                    break;
                case false:
                    yRot += 0;
                    xRot += 0;
                    break;
                default:
            }
            if (KeyboardState.IsKeyPressed(Keys.Left))
            {
                angle -= 0.05f;
            }
            if (KeyboardState.IsKeyPressed(Keys.Right))
            {
                angle += 0.05f;
            }
            if (KeyboardState.IsKeyPressed(Keys.Up))
            {
                speed += 2;
            }
            if (KeyboardState.IsKeyPressed(Keys.Down))
            {
                speed -= 2;
            }
            if (KeyboardState.IsKeyDown(Keys.Tab))
            {
                CursorState = CursorState.Normal;
            }
            if (MouseState.IsButtonDown(MouseButton.Left))
            {
                CursorState = CursorState.Grabbed;
            }
            if (KeyboardState.IsKeyDown(Keys.Z))
            {
                this.topPosition = new Vector3(0f, 0f, 0f);
            }
            base.OnUpdateFrame(args);
            camera.Update(input, mouse, args);
        }
        protected override void OnResize(ResizeEventArgs e)
        {   
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            this.width = e.Width;
            this.height = e.Height;
        }
    }
}
