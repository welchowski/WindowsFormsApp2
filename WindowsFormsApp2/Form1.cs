using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {

            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {


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
                lineA = new Line();
                lineB = new Line();
                lineC = new Line();

                lineA.P1 = a;
                lineA.P2 = b;

                lineB.P1 = b;
                lineB.P2 = c;

                lineC.P1 = c;
                lineC.P2 = a;
                pen = new Pen(Color.Magenta);

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
            List<Point> noDupes =new List<Point>();
            noDupes = points;

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

            bool isUsedPoint(Point point)
            {


                for (int i = 0; i < usedPointss.Count(); i++)
                {
                    

                        if (point.X == usedPointss[i].X && point.Y == usedPointss[i].Y)
                        {

                            return true;



                        }



                    
                }



               

               // Trace.WriteLine(" false used point OR" + " 1 x" + point1.X + " y " + point1.Y + " 2 x" + point2.X + " y " + point2.Y + " 3 x" + point3.X + " y " + point3.Y);
                return false;
                
            }





            void writeNotCrossedTriangels(Triangle triangle)
            {
                if (triangle.crossed == false) { NOTcrossed.Add(triangle); }

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



            bool calculatecrosedline(Line line1, Line line2)
            {

                float x1 = line1.P1.X, x2 = line1.P2.X, x3 = line2.P1.X, x4 = line2.P2.X;

                float y1 = line1.P1.Y, y2 = line1.P2.Y, y3 = line2.P1.Y, y4 = line2.P2.Y;

                float d = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

                // If it is zero, there is no intersection
                if (d == 0)
                {
                    //Trace.WriteLine("D=" + d);
                    //  Trace.WriteLine("NOT_CROSED");
                    return false;

                }

                // Get the x and y
                float pre = (x1 * y2 - y1 * x2), post = (x3 * y4 - y3 * x4);
                float x = (pre * (x3 - x4) - (x1 - x2) * post) / d;
                float y = (pre * (y3 - y4) - (y1 - y2) * post) / d;

                // Check if the x and y coordinates are within both lines
                if (x < Math.Min(x1, x2) || x > Math.Max(x1, x2) ||
                x < Math.Min(x3, x4) || x > Math.Max(x3, x4))
                {// Trace.WriteLine("NOT_CROSED");
                    return false;
                }

                if (y < Math.Min(y1, y2) || y > Math.Max(y1, y2) ||
                y < Math.Min(y3, y4) || y > Math.Max(y3, y4))
                {// Trace.WriteLine("NOT_CROSED");
                    return false;
                }



                //Trace.WriteLine("D=" + d);
                // Trace.WriteLine("CROSED");
                return true;



            }


            //  List<Line> lines2triangls = new List<Line>() { line1,line2,line3,line4,line5,line6,};

            //checking if the triangles are crossed
            bool intersection(Triangle triangle1, Triangle triangle2)
            {

                if (calculatecrosedline(triangle1.lineA, triangle2.lineA) == true ||
                    calculatecrosedline(triangle1.lineA, triangle2.lineB) == true ||
                    calculatecrosedline(triangle1.lineA, triangle2.lineC) == true ||
                    calculatecrosedline(triangle1.lineB, triangle2.lineB) == true ||
                    calculatecrosedline(triangle1.lineB, triangle2.lineC) == true ||
                    calculatecrosedline(triangle1.lineC, triangle2.lineC) == true)
                {
                    Trace.WriteLine("CROSED");
                    //triangle2.pen = greenPen;
                    triangle1.pen = greenPen;


                    // triangle2.crossed = true;
                    triangle1.crossed = true;
                    //   writeCrossedTriangels(triangle1);
                    return true;

                }

                else
                {

                    //  triangle2.pen = blackPen;
                    //  triangle2.crossed = false;
                    triangle1.pen = blackPen;
                    triangle1.crossed = false;

                    // writeNotCrossedTriangels(triangle2);
                    Trace.WriteLine("NOT_CROSED");
                    return false;

                }
            }


            // Construction of all possible obtuse triangles except triangles with the same vertices     
            Triangle t__;
            int connn = 0;


        


            for (int i = 0; i < noDupes.Count(); i++)
            {
                for (int j = 1; j < noDupes.Count(); j++)
                {

                    if (points[i].X == noDupes[j].X && points[i].Y == noDupes[j].Y) {

                        noDupes.RemoveAt(i);
                        


                    }



                }
            }





            for (int i = 0; i < noDupes.Count - 2; i++)
            {
               

                for (int j = i + 1; j < noDupes.Count - 1; j++)
                {
                    


                    for (int k = j + 1; k < noDupes.Count; k++)
                    {

                       
                        t__ = new Triangle(noDupes[i], noDupes[j], noDupes[k]);

                        if (isUsedPoint(t__.A)|| isUsedPoint(t__.B) || isUsedPoint(t__.C) ) { break; }
                        countertuptriangls++;

                           

                            Trace.WriteLine("triangle " + connn);
                        writeUsedpoints(noDupes[i], noDupes[j], noDupes[k]);
                        obtuseTriangles.Add(t__);
                            connn++;

                        // Trace.WriteLine("writed used points");

                        
                        
                    }

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


            Triangle t1;
            Triangle t2;

            int counterOfobtuseTriangles = obtuseTriangles.Count;
            int count = 0;


            for (int i = 0; i < counterOfobtuseTriangles; i++)
            {
                t1 = obtuseTriangles[i];

                for (int j = 0; j < counterOfobtuseTriangles; j++)
                {                
                    if (j != i)
                    {
                        t2 = obtuseTriangles[j];
                        Trace.WriteLine(" i= " + i + " j= " + j);

                    }


                    else
                    {
                        if (countertuptriangls == 1)
                        {
                            break;
                        }

                        if (i == 0 && j == i)
                        {
                            //  t1 = obtuseTriangles[1];
                            t2 = obtuseTriangles[1];

                            Trace.WriteLine(" i= " + i + " j= " + 1);
                        }

                        else if (j == i)
                        {
                            if (j + 1 != counterOfobtuseTriangles)
                            {
                                t2 = obtuseTriangles[j + 1];

                                Trace.WriteLine(" i= " + i + " j= " + j + 1);
                            }

                            else
                            {
                                t2 = obtuseTriangles[j - 1];

                                Trace.WriteLine(" i= " + i + " j= " + (j - 1));
                            }

                        }

                        else
                        {
                            t2 = obtuseTriangles[j];
                            Trace.WriteLine(" i= " + i + " j= " + j);
                        }
                    }

                    count++;
                    Trace.WriteLine(count);
                    if (intersection(t1, t2)) { break; }
                }

            }

            
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

            foreach (Triangle t in obtuseTriangles)
            {
                if (t.crossed)
                {

                    crossed.Add(t);
                }
                else
                {
                    NOTcrossed.Add(t);
                }

            }


            writeCrossedTriangelstoFile();
            writeNotCrossedTriangelsToFILE();
            foreach (var item in noDupes)
            {
                Trace.WriteLine(item.X +" "+item.Y);
            }
           
            Trace.WriteLine("END program");
        }

    }
}