using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        class Point
        {
            public int X { get; set; }
            public int Y { get; set; }

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }


            public void Draw_Point(object sender, PaintEventArgs e)
            {
                Pen pen = new Pen(Color.Black);
                Rectangle rect = new Rectangle(X - 3, Y - 3, 6, 6);
                e.Graphics.DrawEllipse(pen, rect);
            }
        }


        class Line
        {
            public Point P1 { get; set; }
            public Point P2 { get; set; }
            int A_par, B_par, C_par;
            int R, G, B;

        };

        class Triangle
        {
            public bool crossed { get; set; }
            public Pen pen { get; set; }
            public Point A { get; set; }
            public Point B { get; set; }
            public Point C { get; set; }
            public Line lineA { get; set; }
            public Line lineB { get; set; }
            public Line lineC { get; set; }

            public Triangle(Point a, Point b, Point c)
            {
                A = a;
                B = b;
                C = c;
                lineA= new Line();
                lineB=new Line();
                lineC=  new Line();

                lineA.P1 = a;
                lineA.P2 = b;

                lineB.P1 = b;
                lineB.P2 = c;

                lineC.P1 = c;
                lineC.P2 = a;

            }

            double Get_Length(Point P1, Point P2)
            {
                double dx = P1.X - P2.X;
                double dy = P1.Y - P2.Y;
                double len = Math.Sqrt(dx * dx + dy * dy);
                return len;
            }

            // Calculate the area of the triangle using Heron's formula
            public double GetArea()
            {
                double s = (A.X + B.X + C.X) / 2;
                double area = Math.Sqrt(s * (s - A.X) * (s - B.X) * (s - C.X));
                return area;
            }

            // Check if the triangle is obtuse
            public bool IsObtuse()
            {
                double a = Math.Sqrt(Math.Pow(B.X - C.X, 2) + Math.Pow(B.Y - C.Y, 2));
                double b = Math.Sqrt(Math.Pow(A.X - C.X, 2) + Math.Pow(A.Y - C.Y, 2));
                double c = Math.Sqrt(Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2));

                if (Math.Pow(a, 2) + Math.Pow(b, 2) < Math.Pow(c, 2) ||
                    Math.Pow(a, 2) + Math.Pow(c, 2) < Math.Pow(b, 2) ||
                    Math.Pow(b, 2) + Math.Pow(c, 2) < Math.Pow(a, 2))
                {
                    return true;
                }
                return false;
            }

            //checking if the triangle is obtuse
            public bool checkIsTupokutnyi(Triangle t)
            {
                Get_Length(t.A, t.B);

                if (Math.Pow(Get_Length(t.A, t.B), 2) + Math.Pow(Get_Length(t.B, t.C), 2) < Math.Pow(Get_Length(t.C, t.A), 2))
                {
                    return true;
                }
                else return false;
            }
        }







        private void Form1_Paint(object sender, PaintEventArgs e)
        {

            List<Point> points = new List<Point>();
            List<Point> usedPointss = new List<Point>();
            List<Triangle> crossed = new List<Triangle>();
            List<Triangle> NOTcrossed = new List<Triangle>();
            List<Triangle> obtuseTriangles = new List<Triangle>();

            Pen blackPen = new Pen(Color.Black);
            Pen greenPen = new Pen(Color.Green);

            int countertuptriangls = 0;

            // Read coordinates from file 
            using (StreamReader reader = new StreamReader("points.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] coordinates = line.Split(',');
                    int x = int.Parse(coordinates[0]);
                    int y = int.Parse(coordinates[1]);
                    points.Add(new Point(x, y));

                }
            }

            void writeUsedpoints(Point point1, Point point2, Point point3)
            {
                usedPointss.Add(point1);
                usedPointss.Add(point2);
                usedPointss.Add(point3);

            }

            bool isUsedPoint(Point point1, Point point2, Point point3)
            {

                if (usedPointss.Contains(point1) || usedPointss.Contains(point2) || usedPointss.Contains(point3))
                {
                    return true;
                }
                return false;
            }


            void writeCrossedTriangels(Triangle triangle)
            {
                if (!(crossed.Contains(triangle)) && triangle.crossed == true) { crossed.Add(triangle); }

            }

            void writeNotCrossedTriangels(Triangle triangle)
            {
                if (triangle.crossed) { NOTcrossed.Add(triangle); }
                NOTcrossed.Add(triangle);
            }

            void writeNotCrossedTriangelsToFILE()
            {
                using (StreamWriter writer = new StreamWriter(".NOTcrossesd.txt"))

                    foreach (Triangle t in obtuseTriangles)
                    {
                        if (t.crossed == false)
                        {
                            writer.Write(t.A.X + ",");
                            writer.WriteLine(t.A.Y);
                            writer.Write(t.B.X + ",");
                            writer.WriteLine(t.B.Y);
                            writer.Write(t.C.X + ",");
                            writer.WriteLine(t.C.Y);
                        }
                    }
            }


            void writeCrossedTriangelstoFile()
            {
                using (StreamWriter writer = new StreamWriter(".crossesd.txt"))
                {
                    foreach (Triangle t in crossed)
                    {
                        if (t.crossed == true)
                        {
                            writer.Write(t.A.X + ",");
                            writer.WriteLine(t.A.Y);
                            writer.Write(t.B.X + ",");
                            writer.WriteLine(t.B.Y);
                            writer.Write(t.C.X + ",");
                            writer.WriteLine(t.C.Y);
                        }

                    }
                }

            }
            Line lineA = new Line();
            Line lineB = new Line();
            Line lineC = new Line();
            Line line4 = new Line();
            Line line5 = new Line();
            Line line6 = new Line();



          bool  calculatecrosedline(Line line1, Line line2)
            {

                float x1 = line1.P1.X, x2 = line1.P2.X, x3 = line2.P1.X, x4 = line2.P2.X;

                float y1 = line1.P1.Y, y2 = line1.P2.Y, y3 = line2.P1.Y, y4 = line2.P2.Y;

                float d = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

                // If it is zero, there is no intersection
                if (d == 0)
                {
                    //Trace.WriteLine("D=" + d);
                     Trace.WriteLine("NOT_CROSED");
                    return  false;
                    
                }

                else
                {
                    //Trace.WriteLine("D=" + d);
                     Trace.WriteLine("CROSED");
                    return  true;
                    
                }

            }










          //  List<Line> lines2triangls = new List<Line>() { line1,line2,line3,line4,line5,line6,};
           
            //checking if the triangles are crossed
            bool intersection(Triangle triangle1, Triangle triangle2)
            {
                





                if (calculatecrosedline(triangle1.lineA, triangle2.lineA)==false||
                    calculatecrosedline(triangle1.lineA, triangle2.lineB) == false ||
                    calculatecrosedline(triangle1.lineA, triangle2.lineC) == false ||
                    calculatecrosedline(triangle1.lineB, triangle2.lineB) == false ||
                    calculatecrosedline(triangle1.lineB, triangle2.lineC) == false ||
                    calculatecrosedline(triangle1.lineC, triangle2.lineB) == false ||
                    calculatecrosedline(triangle1.lineC, triangle2.lineC) == false  )
                {
                    triangle2.pen = blackPen;
                    triangle2.crossed = false;
                   // triangle2.pen = blackPen;
                   // triangle2.crossed = false;

                    writeNotCrossedTriangels(triangle2);
                   // Trace.WriteLine("NOT_CROSED");
                    return false;

                }

                else {
                   // Trace.WriteLine("CROSED");
                    triangle2.pen = greenPen;
                  //  triangle2.pen = greenPen;

                    writeCrossedTriangels(triangle1);
                    triangle2.crossed = true;
                  //  triangle2.crossed = true;
                    return true;
                }



                
            } 


            // Construction of all possible obtuse triangles except triangles with the same vertices     

            for (int i = 0; i < points.Count - 2; i++)
            {
                for (int j = i + 1; j < points.Count - 1; j++)
                {
                    for (int k = j + 1; k < points.Count; k++)
                    {
                        Triangle t = new Triangle(points[i], points[j], points[k]);
                        if (t.checkIsTupokutnyi(t))
                        {
                            countertuptriangls++;

                            if (isUsedPoint(points[i], points[j], points[k])) { break; }

                            obtuseTriangles.Add(t);

                            writeUsedpoints(points[i], points[j], points[k]);

                        }
                    }
                }
            }
            int count = 0;
            foreach (Triangle t in obtuseTriangles)
            {
                t.pen = new Pen(Color.Yellow);
                foreach (Triangle t2 in obtuseTriangles)
                {
                    count++;
                    Trace.WriteLine(count);
                    intersection(t, t2);
                }
            }

            // Draw the triangles on the screen
            foreach (Triangle t in obtuseTriangles)
            {
                //Draw points
                t.A.Draw_Point(sender, e);
                t.B.Draw_Point(sender, e);
                t.C.Draw_Point(sender, e);

                // Draw the lines between the points
                e.Graphics.DrawLine(t.pen, (float)t.A.X, (float)t.A.Y, (float)t.B.X, (float)t.B.Y);
                e.Graphics.DrawLine(t.pen, (float)t.B.X, (float)t.B.Y, (float)t.C.X, (float)t.C.Y);
                e.Graphics.DrawLine(t.pen, (float)t.C.X, (float)t.C.Y, (float)t.A.X, (float)t.A.Y);
            }

            writeCrossedTriangelstoFile();
            writeNotCrossedTriangelsToFILE();
            Trace.WriteLine("END program");
        }
    }
}