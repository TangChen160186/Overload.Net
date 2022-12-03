using System.Runtime.InteropServices;
using Assimp;
using BulletSharp;
using Microsoft.VisualBasic;
using OpenTK.Mathematics;
using OvAudio.OvAudio.Core;
using OvAudio.OvAudio.Entities;
using OvAudio.OvAudio.Resources;
using OvCore.OvCore.Api;
using OvCore.OvCore.Ecs;
using OvCore.OvCore.Ecs.Components;
using OvDebug;
using OvMath;
using OvPhysics.Entities;
using OvRendering.OvRendering.Resources;
using OvRendering.OvRendering.Resources.Loaders;
using OvTest;
using Quaternion = OpenTK.Mathematics.Quaternion;


class A
{
    public int Num;
}

class B
{
    public A  ClassA { get; set; }
}
class Program
{
    public static void Main(string[] args)
    {
        A a = new A();
        a.Num = 10;
        B b = new B();
        b.ClassA = a;
        Console.WriteLine(b.ClassA == a);
        Console.WriteLine(b.ClassA.Num);



        var light = new CDirectionalLight(new Actor());

        SerializeHelper.Serialize(light, @"C:\Users\16018\Desktop\a.xml");
        var c = SerializeHelper.DeSerialize<CDirectionalLight>(@"C:\Users\16018\Desktop\a.xml");
        using Game game = new Game(800, 600, "LearnOpenTK");
        //Run takes a double, which is how many frames per second it should strive to reach.
        //You can leave that out and it'll just update as fast as the hardware will allow it.
        game.RenderFrequency = 60;
        game.Run();
    }
}
