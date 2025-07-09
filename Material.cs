using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    public class Material
    {
        public RayColour colour;
        public double ambient;
        public double diffuse;
        public double specular;
        public double shininess;
        public double reflectivity;
        public double transparency = 0.0;

        //examples RIs
        //Vacuum = 1
        //Air = 1.00029
        //Water = 1.333
        //Glass = 1.52
        //Diamond = 2.417

        public double refractiveIndex = 1.0;
        public Pattern pattern = null;

        public Material(RayColour colour, double ambient, double diffuse, double specular, double shininess, double reflectivity, double transparency, double refractiveIndex, Pattern pattern)
        {
            this.colour = colour;
            this.ambient = ambient;
            this.diffuse = diffuse;
            this.specular = specular;
            this.shininess = shininess;
            this.reflectivity = reflectivity;
            this.transparency = transparency;
            this.refractiveIndex = refractiveIndex;
            this.pattern = pattern;
        }

        public Material(RayColour colour, double ambient, double diffuse, double specular, double shininess, double reflectivity, double transparency, double refractiveIndex) : this(colour, ambient, diffuse, specular, shininess, reflectivity, transparency, refractiveIndex, null)
        {
            
        }
}
}
