/**
Fonte: https://github.com/mono/opentk/blob/master/Source/Examples/OpenGL/1.x/Textures.cs
 */
using System;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace textura
{
  class Render : GameWindow
  {
    //FIXME: precisei instalar $ brew install mono-libgdiplus
    int texture;

    Esfera sol = new Esfera("Sol");
     Esfera terra = new Esfera("Terra");
     Esfera lua = new Esfera("Lua");
     double xMin, xMax, yMin, yMax, zMin, zMax;

    public void resetCamera()
    {
      xMin = -0.7;
      xMax = 0.7;
      yMin = -0.7;
      yMax = 0.7;
      zMin = 1;
      zMax = 0;
    }

    public Render(int width, int height) : base(width, height) { }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      GL.ClearColor(Color.Black);
      GL.Enable(EnableCap.DepthTest);
      GL.Enable(EnableCap.CullFace);

      sol.CarregaTextura("sun.jpg");
      terra.CarregaTextura("earth.bmp");
      lua.CarregaTextura("moon.png");

      sol.EscalaXYZ(0.5,0.5,0.5);
      terra.EscalaXYZ(0.5,0.5, 0.5);
      lua.EscalaXYZ(0.5,0.5, 0.5);

      sol.FilhoAdicionar(terra);
      terra.FilhoAdicionar(lua);
      terra.TranslacaoXYZ(6,0,1);
      lua.TranslacaoXYZ(2,0,1);
      resetCamera();
    }

    protected override void OnUnload(EventArgs e)
    {
      sol.RemoveTextura();
      terra.RemoveTextura();
      lua.RemoveTextura();
    }

    protected override void OnResize(EventArgs e)
    {
      base.OnResize(e);
      GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
      Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 50.0f);
      GL.MatrixMode(MatrixMode.Projection);
      GL.LoadMatrix(ref projection);
    }
    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      base.OnUpdateFrame(e);
      GL.Ortho(xMin,xMax,yMin,yMax,zMin,zMax);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
      Matrix4 modelview = Matrix4.LookAt(eye: new Vector3(5, 5, 5), target: new Vector3(0, 0, 0), up: Vector3.UnitY);
      GL.MatrixMode(MatrixMode.Modelview);
      GL.LoadMatrix(ref modelview);

      SRU3D();

      Animacao();
      sol.Desenhar();
      SwapBuffers();
    }

    public void Animacao()
    {
      terra.RotacaoY(0.3);   // em torno do sol
      terra.RotacaoYBBox(15); // rotacao da terra
      lua.RotacaoY(-12);
    }

    protected override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
    {
      if (e.Key == Key.Escape)
        this.Exit();
      else
        if (e.Key == Key.F)
        GL.CullFace(CullFaceMode.Front);
      if (e.Key == Key.B)
        GL.CullFace(CullFaceMode.Back);
      if (e.Key == Key.A)
      //FIXME: aqui deveria aplicar a textura no lado de fora e dentro, mas não aparece nada
        GL.CullFace(CullFaceMode.FrontAndBack);
    }


    private void SRU3D()
    {
      GL.LineWidth(3);
      GL.Begin(PrimitiveType.Lines);
      GL.Color3(Color.Red);
      GL.Vertex3(0, 0, 0); GL.Vertex3(200, 0, 0);
      GL.Color3(Color.Green);
      GL.Vertex3(0, 0, 0); GL.Vertex3(0, 200, 0);
      GL.Color3(Color.Blue);
      GL.Vertex3(0, 0, 0); GL.Vertex3(0, 0, 200);
      GL.End();
    }

  }

  class Program
  {
    static void Main(string[] args)
    {
      Render window = new Render(600, 600);
      window.Run(1.0 / 60.0);
    }
  }

}