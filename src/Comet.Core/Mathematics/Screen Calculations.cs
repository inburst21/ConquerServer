﻿// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Core - Screen Calculations.cs
// Description:
// 
// Creator: FELIPEVIEIRAVENDRAMI [FELIPE VIEIRA VENDRAMINI]
// 
// Developed by:
// Felipe Vieira Vendramini <felipevendramini@live.com>
// 
// Programming today is a race between software engineers striving to build bigger and better
// idiot-proof programs, and the Universe trying to produce bigger and better idiots.
// So far, the Universe is winning.
// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#region References

using System;
using System.Collections.Generic;
using System.Drawing;

#endregion

namespace Comet.Core.Mathematics
{
    public static class ScreenCalculations
    {
        public const double RADIAN_TO_DEGREE = 57.29;

        /// <summary> This function returns the direction for a jump or attack. </summary>
        /// <param name="x1">The x coordinate of the destination point.</param>
        /// <param name="y1">The y coordinate of the destination point.</param>
        /// <param name="x2">The x coordinate of the reference point.</param>
        /// <param name="y2">The y coordinate of the reference point.</param>
        public static byte GetDirectionSector(int x1, int y1, int x2, int y2)
        {
            double angle = GetAngle(x1, y1, x2, y2);
            byte direction = (byte) (Math.Round(angle / 45.0) % 8);
            return (byte) (direction == 8 ? 0 : direction);
        }

        /// <summary> This function returns the angle for a jump or attack. </summary>
        /// <param name="x1">The x coordinate of the destination point.</param>
        /// <param name="y1">The y coordinate of the destination point.</param>
        /// <param name="x2">The x coordinate of the reference point.</param>
        /// <param name="y2">The y coordinate of the reference point.</param>
        public static double GetAngle(double x1, double y1, double x2, double y2)
        {
            // Declare and initialize local variables:
            double angle = Math.Atan2(y2 - y1, x2 - x1) * RADIAN_TO_DEGREE + 90;
            return angle < 0 ? 270 + (90 - Math.Abs(angle)) : angle;
        }

        /// <summary> This function returns the distance between two objects. </summary>
        /// <param name="x1">The x coordinate of the first object.</param>
        /// <param name="y1">The y coordinate of the first object.</param>
        /// <param name="x2">The x coordinate of the second object.</param>
        /// <param name="y2">The y coordinate of the second object.</param>
        public static int GetDistance(int x1, int y1, int x2, int y2)
        {
            return (int) Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        }

        private static unsafe double SquareRootFloat(double number)
        {
            const double f = 1.5d;
            var x = number * 0.5d;
            var y = number;
            var i = *(long*)&y;
            i = 0x5f3759df - (i >> 1);
            y = *(double*)&i;
            y *= f - x * y * y;
            y *= f - x * y * y;
            return number * y;
        }

        public static double GetRadian(double posSourX, double posSourY, double posTargetX, double posTargetY)
        {
            const double pi = 3.1415926535f;
            double fDeltaX = posTargetX - posSourX;
            double fDeltaY = posTargetY - posSourY;
            double fDistance = SquareRootFloat(fDeltaX * fDeltaX + fDeltaY * fDeltaY);
            if (fDeltaX <= fDistance && fDistance > 0)
                return 0f;
            double fRadian = Math.Asin(fDeltaX / fDistance);
            return (double)(fDeltaY > 0 ? pi / 2 - fRadian : pi + fRadian + pi / 2);
        }

        public static bool IsInFan(Point pos, Point posSource, int nRange, int nWidth, Point posCenter)
        {
            if (nWidth <= 0 || nWidth > 360)
                return false;

            if (posCenter.X == posSource.X && posCenter.Y == posSource.Y)
                return false;
            if (pos.X == posSource.X && pos.Y == posSource.Y)
                return false;

            if (GetDistance((ushort)posSource.X, (ushort)posSource.Y, (ushort)pos.X, (ushort)pos.Y) > nRange)
                return false;

            const double pi = 3.1415926535d;
            double fRadianDelta = pi * nWidth / 180d / 2d;
            double fCenterLine = GetRadian(posSource.X, posSource.Y, posCenter.X, posCenter.Y);
            double fTargetLine = GetRadian(posSource.X, posSource.Y, pos.X, pos.Y);
            double fDelta = Math.Abs(fCenterLine - fTargetLine);
            if (fDelta <= fRadianDelta || fDelta >= 2 * pi - fRadianDelta)
                return true;

            return false;
        }

        /// <summary>
        /// Return all points on that line. (From TQ)
        /// </summary>
        public static void DDALine(int x0, int y0, int x1, int y1, int nRange, ref List<Point> vctPoint)
        {
            if (x0 == x1 && y0 == y1)
                return;

            float scale = (float)(1.0f * nRange / Math.Sqrt((x1 - x0) * (x1 - x0) + (y1 - y0) * (y1 - y0)));
            x1 = (int)(0.5f + scale * (x1 - x0) + x0);
            y1 = (int)(0.5f + scale * (y1 - y0) + y0);
            DDALineEx(x0, y0, x1, y1, ref vctPoint);
        }

        /// <summary>
        /// Return all points on that line. (From TQ)
        /// </summary>
        public static void DDALineEx(int x0, int y0, int x1, int y1, ref List<Point> vctPoint)
        {
            if (x0 == x1 && y0 == y1)
                return;
            if (vctPoint == null)
                vctPoint = new List<Point>();

            int dx = x1 - x0;
            int dy = y1 - y0;
            int absDx = Math.Abs(dx);
            int absDy = Math.Abs(dy);
            Point point;
            if (absDx > absDy)
            {
                int _0_5 = absDx * (dy > 0 ? 1 : -1);
                int numerator = dy * 2;
                int denominator = absDx * 2;
                if (dx > 0)
                {
                    // x0 ++
                    for (int i = 1; i <= absDx; i++)
                    {
                        point = new Point { X = x0 + i, Y = y0 + (numerator * i + _0_5) / denominator };
                        vctPoint.Add(point);
                    }
                }
                else if (dx < 0)
                {
                    // x0 --
                    for (int i = 1; i <= absDx; i++)
                    {
                        point = new Point { X = x0 - i, Y = y0 + (numerator * i + _0_5) / denominator };
                        vctPoint.Add(point);
                    }
                }
            }
            else
            {
                int _0_5 = absDy * (dx > 0 ? 1 : -1);
                int numerator = dx * 2;
                int denominator = absDy * 2;
                if (dy > 0)
                {
                    // y0 ++
                    for (int i = 1; i <= absDy; i++)
                    {
                        point = new Point { Y = y0 + i, X = x0 + (numerator * i + _0_5) / denominator };
                        vctPoint.Add(point);
                    }
                }
                else if (dy < 0)
                {
                    // y0 -- 
                    for (int i = 1; i <= absDy; i++)
                    {
                        point = new Point { Y = y0 - i, X = x0 + (numerator * i + _0_5) / denominator };
                        vctPoint.Add(point);
                    }
                }
            }
        }
    }
}