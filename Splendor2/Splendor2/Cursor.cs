using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using XNACS1Lib;

namespace Splendor
{
    public class Cursor
    {
        private MouseState oMouseState; // the mouse pixel position

        public XNACS1Circle oHitbox;
        private const float fCursorRadius = 0.1f;

        private Vector2 oPointingVector; // mouse position in world coordinates

        private bool bResetLeft = true;
        private bool bResetRight = true;
        private ButtonState oLastStateLeft;
        private ButtonState oLastStateRight;
        private float fWorldWidth;
        private float fWorldHeight;

        public Vector2 oDragPosition = new Vector2();

        public Cursor()
        {
            oHitbox = new XNACS1Circle(new Vector2(), fCursorRadius);
            oHitbox.Color = Color.Red;
            oHitbox.Visible = true;
            oMouseState = Mouse.GetState();

            float x;
            float y;

            if (MyLibrary.nWindowPixelWidth > MyLibrary.nWindowPixelHeight)
            {
                fWorldWidth = 50f;
                fWorldHeight = (float)MyLibrary.nWindowPixelHeight / (float)MyLibrary.nWindowPixelWidth * 50;
            }
            else
            {
                fWorldHeight = 50f;
                fWorldWidth = (float)MyLibrary.nWindowPixelWidth / (float)MyLibrary.nWindowPixelHeight * 50;
            }

            //fWorldWidth = (float)(MyLibrary.nWindowPixelWidth / 2) / (float)(MyLibrary.nWindowPixelWidth / 2) * (x / 2);
            //fWorldHeight = (float)(MyLibrary.nWindowPixelHeight / 2) / (float)(MyLibrary.nWindowPixelHeight / 2) * (y / 2);
        }

        public void Update()
        {
            //XNACS1Base.SetBottomEchoColor(Color.Red);
            //XNACS1Base.EchoToTopStatus("Mouse:" + oHitbox.Center);

            #region Get Mouse Pixel Position
            oMouseState = Mouse.GetState();
            #endregion

            CalculatePosition();

            #region Reset Left Mouse Click
            if (oMouseState.LeftButton != oLastStateLeft && oMouseState.LeftButton == ButtonState.Pressed)
                bResetLeft = true;
            else
                bResetLeft = false;

            oLastStateLeft = oMouseState.LeftButton;
            #endregion

            #region Reset Right Mouse Click
            if (oMouseState.RightButton != oLastStateRight && oMouseState.RightButton == ButtonState.Pressed)
                bResetRight = true;
            else
                bResetRight = false;

            oLastStateRight = oMouseState.RightButton;
            #endregion
        }

        public bool Collided(XNACS1Primitive oObject)
        {
            return oHitbox.Collided(oObject);
        }

        public void CalculatePosition()
        {
            // hard coded fix later
            //oPointingVector.X = oMouseState.X - fWorldWidth;
            //oPointingVector.Y = -(oMouseState.Y - fWorldHeight);
            oPointingVector.X = (oMouseState.X - (float)(MyLibrary.nWindowPixelWidth / 2)) / (float)(MyLibrary.nWindowPixelWidth / 2) * fWorldWidth;
            oPointingVector.Y = -(oMouseState.Y - (float)(MyLibrary.nWindowPixelHeight / 2)) / (float)(MyLibrary.nWindowPixelHeight / 2) * fWorldHeight;
            oHitbox.Center = oPointingVector;
            oHitbox.CenterX += XNACS1Base.World.WorldDimension.X / 2;
            oHitbox.CenterY += XNACS1Base.World.WorldDimension.Y / 2;
        }

        public bool IsLeftMouseDown()
        {
            if (oMouseState.LeftButton == ButtonState.Pressed)
                return true;
            else
                return false;
        }

        public bool IsRightMouseDown()
        {
            if (oMouseState.RightButton == ButtonState.Pressed)
                return true;
            else
                return false;
        }

        public bool LeftClick()
        {
            if (oMouseState.LeftButton == ButtonState.Pressed && bResetLeft == true)
                return true;
            else
                return false;
        }

        public bool RightClick()
        {
            if (oMouseState.RightButton == ButtonState.Pressed && bResetRight == true)
                return true;
            else
                return false;
        }
    }
}
