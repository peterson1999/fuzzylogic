using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace fuzzylogic
{
    class Car
    {
        public Boolean Animating;

        public Double xPos, yPos, angle;
        public Double xDuration, yDuration;

        public Double xDestination, yDestination;

        public void Initialize(Double xPosition, Double yPosition, Double Duration)
        {
            if (Animating) { return; }

            angle = 0;

            xPos = xPosition;
            yPos = yPosition;

            xDestination = xPos;
            yDestination = yPos;

            xDuration = Duration;
            yDuration = Duration;

            Animating = true;
        }

        public void SetDestination(Double x, Double y)
        {
            xDestination = x;
            yDestination = y;
        }

        public void PointTowardsDestination()
        {
            Double x = xDestination - xPos;
            Double y = yDestination - yPos;

            Double lAngle = Math.Atan2(y, x);

            lAngle = Math.Abs(lAngle * 180 / Math.PI);

            if ((x >= 0) && (y >= 0)) { lAngle = 360 - lAngle; }
            if ((x < 0) && (y >= 0)) { lAngle = 270 - (lAngle - 90); }

            angle = Math.Abs(lAngle);
        }

        public void MoveInDirection(Double pAngle)
        {
            Double x = Math.Cos((pAngle * Math.PI) / 180);
            Double y = Math.Sqrt((1 - (x * x)));

            xPos += x;
            if (pAngle <= 180)
            {
                yPos -= y;
            }
            else
            {
                yPos += y;
            }
        }

        public bool CheckCollsions(ref Double dist, Point sprite, ref Double pAngle)
        {
            Double a = sprite.X - xPos;
            Double b = sprite.Y - yPos;
            Double c, temp;

            c = (a * a) + (b * b);
            c = Math.Sqrt(c);

            pAngle = Math.Atan2(b, a);
            pAngle = pAngle * 180 / Math.PI;
            temp = Math.Abs(pAngle);
            if ((angle >= 0) && (angle <= 90))
            {
                pAngle = Math.Abs(angle + pAngle);
                if (pAngle > 180) { pAngle = 360 - pAngle; }
            }
            else if ((angle > 90) && (angle <= 180))
            {
                pAngle = Math.Abs(angle + pAngle);
                if (pAngle > 180) { pAngle = 360 - pAngle; }
            }
            else if ((angle > 180) && (angle <= 270))
            {
                pAngle = Math.Abs(angle + pAngle);
                if (pAngle > 180) { pAngle = 360 - pAngle; }
                pAngle = Math.Abs(pAngle);
            }
            else if ((angle > 270) && (angle <= 360))
            {
                pAngle = Math.Abs(angle + pAngle);
                if (pAngle > 180) { pAngle = 360 - pAngle; }
                pAngle = Math.Abs(pAngle);
            }

            if ((a < 0) && (b > 0))
            {
                if ((angle < 180 - temp) || (angle > 180 - temp + 180)) { pAngle *= -1; }
            }
            else if ((a > 0) && (b > 0))
            {
                if ((angle < 180 - temp) || (angle > 180 - temp + 180)) { pAngle *= -1; }
            }
            else if ((a > 0) && (b < 0))
            {
                Double x = sprite.X - xPos;
                Double y = yPos - sprite.Y;
                temp = Math.Atan2(y, x);
                temp = temp * 180 / Math.PI;
                temp = Math.Abs(temp);
                if ((angle < temp + 180) && (angle > temp)) { pAngle *= -1; }
            }
            else if ((a < 0) && (b < 0))
            {
                Double x = sprite.X - xPos;
                Double y = yPos - sprite.Y;
                temp = Math.Atan2(y, x);
                temp = temp * 180 / Math.PI;
                temp = Math.Abs(temp);
                if ((angle < temp + 180) && (angle > temp)) { pAngle *= -1; }
            }

            dist = c;
            //if (c < 120) { return true; } else { return false; }                       
            return true;
        }
    }
}
