﻿using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laba_24._03
{
    internal class Camera
    {
        private float SPEED = 15f;
        private int SCREENWIDTH;
        private int SCREENHEIGHT;
        private float SENSITIVITY = 100f;

        public Vector3 position;

        Vector3 up = Vector3.UnitY;
        Vector3 front = -Vector3.UnitZ;
        Vector3 right = Vector3.UnitX;

        private float pitch;
        private float yaw = -270.0f;
        private bool firstMove = true;
        private Vector2 lastPos;

        public Camera(int width, int height, Vector3 position)
        {
            SCREENWIDTH = width;
            SCREENHEIGHT = height;
            this.position = position;
        }
        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(position, position + front, up);
        }
        public Matrix4 GetProjection()
        {
            return
           Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60f), SCREENWIDTH / SCREENHEIGHT, 0.1f, 2000f);
        }
        public void InputController(KeyboardState input, MouseState mouse, FrameEventArgs e)
        {
            
            if (input.IsKeyDown(Keys.W))
            {
                position += front * SPEED * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.A))
            {
                position -= right * SPEED * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.S))
            {
                position -= front * SPEED * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.D))
            {
                position += right * SPEED * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.Space))
            {
                position += up * SPEED * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.LeftControl))
            {
                position += -up * SPEED * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                SPEED = 80f;
            }
            else if (input.IsKeyReleased(Keys.LeftShift))
            {
                SPEED = 8f;
            }
            if(position.Y < 0.00001f)
            {
                position.Y = 0.00001f;
            }
            if (input.IsKeyDown(Keys.LeftAlt))
            {
                firstMove = true;
            }
            else
            {
                if (firstMove)
                {
                    lastPos = new Vector2(position.X, position.Y);
                    lastPos = new Vector2(mouse.X, mouse.Y);
                    firstMove = false;
                }
                else
                {
                    var deltaX = mouse.X - lastPos.X;
                    var deltaY = mouse.Y - lastPos.Y;
                    lastPos = new Vector2(mouse.X, mouse.Y);
                    yaw += deltaX * SENSITIVITY * (float)e.Time;
                    pitch -= deltaY * SENSITIVITY * (float)e.Time;
                } 
            }
            UpdateVectors();
        }
        public void Update(KeyboardState input, MouseState mouse,
        FrameEventArgs e)
        {
            InputController(input, mouse, e);
        }
        private void UpdateVectors()
        {
            if (pitch > 89.0f)
            {
                pitch = 89.0f;
            }
            if (pitch < -89.0f)
            {
                pitch = -89.0f;
            }
            front.X = MathF.Cos(MathHelper.DegreesToRadians(pitch)) *
            MathF.Cos(MathHelper.DegreesToRadians(yaw));
            front.Y = MathF.Sin(MathHelper.DegreesToRadians(pitch));
            front.Z = MathF.Cos(MathHelper.DegreesToRadians(pitch)) *
            MathF.Sin(MathHelper.DegreesToRadians(yaw));
            front = Vector3.Normalize(front);
            right = Vector3.Normalize(Vector3.Cross(front,
            Vector3.UnitY));
            up = Vector3.Normalize(Vector3.Cross(right, front));
        }

    }
}
