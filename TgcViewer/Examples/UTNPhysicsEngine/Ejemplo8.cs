using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcGeometry;
using Examples.UTNPhysicsEngine.physics.body;

namespace Examples
{
    /// <summary>
    /// UTNPhysicsEngine Main example
    /// </summary>
    public class Ejemplo8 : TgcExample
    {

        public override string getCategory()
        {
            return "UTNPhysicsEngine";
        }

        public override string getName()
        {
            return "08 ejemplo del UTNPhysicsEngine";
        }

        public override string getDescription()
        {
            return "Octavo ejemplo del UTNPhysicsEngine";
        }

        private Prototipos proto = new Prototipos();
        
        private const float ZLocation = -40.0f;
                    
        public enum Density
        {
            Low = 5,
            Medium = 20,
            High = 250,
            Heavy = 200
        }

        public void createBodys()
        {
            float locationY = -80.0f;
            float locationX = 0.0f;
            AddMultipleCollisionSpheres(locationX, locationY, Density.Low, Density.Low, Density.Low);

            locationY = -45.0f;
            locationX = 30.0f;
            AddMultipleCollisionSpheres(locationX, locationY, Density.Medium, Density.Low, Density.Low);

            locationY = -10.0f;
            locationX = 0.0f;
            AddMultipleCollisionSpheres(locationX, locationY, Density.Low, Density.Medium, Density.Low);

            locationY = 25.0f;
            locationX = 30.0f;
            AddMultipleCollisionSpheres(locationX, locationY, Density.Low, Density.Medium, Density.High);

            locationY = 55f;
            const float radius = 7.0f;            
            const float startingLocationXStationary = 0.0f;
            const float startingLocationXMoving = -50.0f;
            const int numberOfStationarySpheres = 4;
            const int numberOfMovingSpheres = 2;

            for (int i = 0; i < numberOfMovingSpheres; ++i)
            {
               this.AddBody(Density.Medium,
                    new Vector3(startingLocationXMoving + (i * 2.0f * radius), locationY, ZLocation) * 2,
                    new Vector3(2.0f, 0.0f, 0.0f) * 2,
                    radius*2);
            }

            for (int i = 0; i < numberOfStationarySpheres; ++i)
            {
                this.AddBody(Density.Medium,
                    new Vector3(startingLocationXStationary + (i * 2.0f * radius), locationY, ZLocation) * 2,
                    new Vector3(), radius * 2);
            }
        }

        private void AddMultipleCollisionSpheres(float locationX, float locationY, Density densityMoving, Density densityStationaryTop,
            Density densityStationaryBottom)
        {
            const float stationarySpheresSeparation = 0.1f;
            const float radius = 7.0f;
            const float initialXLocationMoving = -20.0f;
            const float initialXLocationStationary = 20.0f;

            const float movingVelocityX = 2.0f;
            this.AddBody(densityMoving,
                new Vector3(locationX + initialXLocationMoving, locationY, ZLocation) * 2,
                new Vector3(movingVelocityX, 0.0f, 0.0f) * 2,
                radius * 2);

            this.AddBody(densityStationaryTop,
                new Vector3(locationX + initialXLocationStationary, locationY + (radius + (stationarySpheresSeparation) / 2.0f), ZLocation) * 2,
                new Vector3(), radius * 2);

            this.AddBody(densityStationaryBottom,
                new Vector3(locationX + initialXLocationStationary, locationY - (radius + (stationarySpheresSeparation) / 2.0f), ZLocation) * 2,
                new Vector3(), radius * 2);
        }

        private void AddBody(Density density, Vector3 initialLocation, Vector3 initialVelocity, float radius)
        {
            float densityValue = (int)density;

            float mass = densityValue * (1.33333f) * FastMath.PI * (radius * radius * radius);
            
            SphereBody sphere = new SphereBody(radius, initialLocation, initialVelocity, new Vector3(), mass, false);
            proto.Bodys.Add(sphere);            
        }

        public override void init()
        {
            proto.init();
            this.createBodys();
            proto.optimize();
        }

        public override void close()
        {
            proto.close();
        }

        public override void render(float elapsedTime)
        {
            proto.render(elapsedTime);
        }
    }
}
