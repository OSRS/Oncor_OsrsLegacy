using Osrs.Numerics.Spatial.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Fact: "+(GeometryFactory2Double.Instance!=null));
            GeometryFactory2Base<double> factory = GeometryFactory2Double.Instance;
            Console.WriteLine("Factory is " + (factory == null ? "null" : "not null"));

            if (factory!=null)
            {
                Point2<double> pt = factory.ConstructPoint(43, 22);
                Console.WriteLine("Created point: " + pt.ToString());
                Ring2<double> rg = factory.ConstructRing(new Point2<double>[] { factory.ConstructPoint(22.45, 33.33), factory.ConstructPoint(23.45, 33.33), factory.ConstructPoint(22.45, 35.33) });
                Polygon2<double> pl = factory.ConstructPolygon(rg);

                if (rg!=null)
                {
                    Console.WriteLine("Created ring: " + rg.ToString());
                    object o = Osrs.Numerics.Spatial.Postgres.NpgSpatialUtils.ToPGis(rg);
                    Console.WriteLine("Created PG Geometry: " + (o!=null).ToString());
                }

                if (pl!=null)
                {
                    Console.WriteLine(GeoJsonUtils.ToGeoJson(pl).ToString());
                }
            }

            Console.WriteLine("ALL DONE");
            Console.ReadLine();
        }
    }
}
